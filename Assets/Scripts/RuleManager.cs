using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RuleManager : MonoBehaviour
{
	public static RuleManager instance;
	public Transform actorsTransform;
	public List<Rule> listRules;
	public List<Rule> listRulesToDelete;
	public List<RuleByTag> listRulesByTag;
	public List<Action<Actor, Actor>> listActions;
	public List<string> listActionsName;
	List<Actor> actorToDelete;
	List<Rule> actorToSpawn;

	void Awake()
	{
		instance = this;
		listRules = new List<Rule>();
		listRulesToDelete = new List<Rule>();
		listRulesByTag = new List<RuleByTag>();
		listActions = new List<Action<Actor, Actor>>();
		listActionsName = new List<string>();
		actorToDelete = new List<Actor>();
		actorToSpawn = new List<Rule>();

		listActions.Add(LookActor);
		listActions.Add(LookAwayActor);
		listActions.Add(DestroyActor);
		listActions.Add(SpawnActor);
		listActions.Add(FollowActor);
		listActions.Add(CopyDirectionActor);
		listActions.Add(CopyOppositeDirectionActor);
		listActions.Add(IncreaseSpeedActor);
		listActions.Add(DecreaseSpeedActor);
		listActions.Add(IncreaseScaleActor);
		listActions.Add(DecreaseScaleActor);
		listActions.Add(AscendActor);
		listActions.Add(DescendActor);
		foreach (Action<Actor,Actor> action in listActions)
			listActionsName.Add(action.Method.Name);
	}

	void Update()
	{
		if (!GameServer.instance.Connected())
			return ;
		foreach (Rule rule in listRules)
		{
			if (rule as RuleDistance != null)
			{
				RuleDistance r = (RuleDistance)rule;
				rule.Check(Vector3.Distance(rule.from.transform.position, rule.to.transform.position) <= r.distance);
			}
			else if (rule as RuleCount != null)
			{
				RuleCount r = (RuleCount)rule;
				if (r.type == RuleCountType.DISTANCE)
				{
					r.tmp += r.from.distanceWalked;
					rule.Check(r.tmp >= r.count);
					if (rule.activate)
						r.tmp = 0.0f;
				}
				else if (r.type == RuleCountType.TIME_BIRTH)
				{
					if (rule.numEvents == 0)
						rule.Check(r.from.totalTimeElapse >= r.count);
					else
						rule.activate = false;
				}
				else if (r.type == RuleCountType.TIME_BETWEEN_ACTION)
				{
					r.tmp += Time.deltaTime;
					rule.Check(r.tmp >= r.count);
					if (rule.activate)
						r.tmp = 0.0f;
				}
				else if (r.type == RuleCountType.NUM_ACTIONS)
				{
					if (r.from.power > 0)
						rule.Check((r.from.power % r.count) == 0);
				}
			}
		}
		foreach (Rule rule in listRules)
		{
			if (rule.IsActivate())
				rule.action(rule.from, rule.to);
		}
		SpawnActors();
		DeleteActor();
		DeleteRule();
	}

	public void SpawnActors()
	{
		foreach (Rule rule in actorToSpawn)
		{
			Actor newActor = Instantiate(rule.to, actorsTransform);
			ActorManager.instance.listActors.Add(newActor);
			int index = ActorManager.instance.listActorsOrigin.IndexOf(rule.to.origin);
			GameServer.instance.SendSpawnActor(index);
			newActor.origin = ActorManager.instance.listActorsOrigin[index];
			foreach (RuleByTag ruleByTag in listRulesByTag)
			{
				if (newActor.tags.Contains(ruleByTag.tag) && ruleByTag.rule.action != SpawnActor)
				{
					ruleByTag.rule.to = newActor;
					AddRule(ruleByTag.rule);
				}
			}
		}
		actorToSpawn.Clear();
	}

	public void DeleteActor()
	{
		foreach (Actor actor in actorToDelete)
		{
			listRules.RemoveAll(u => u.from == actor);
			listRules.RemoveAll(u => u.to == actor);
			Destroy(actor.gameObject);
		}
		actorToDelete.Clear();
	}

	public void DeleteRule()
	{
		foreach (Rule r in listRulesToDelete)
		{
			listRules.Remove(r);
		}
		listRulesToDelete.Clear();
	}

	public void DeleteRule(Rule rule, string tag)
	{
		if (!GameServer.instance.Connected())
		{
			GameClient.instance.SendRuleTagDelete(rule, tag);
			return ;
		}
		RuleByTag ruleByTag;

		ruleByTag = null;
		foreach (RuleByTag r in listRulesByTag)
		{
			if (r.rule.Equals(rule) && r.tag == tag)
				ruleByTag = r;
		}
		if (ruleByTag == null)
			return ;
		foreach(Rule r in listRules)
		{
			if (r.GetType() == ruleByTag.rule.GetType())
			{
				if (r.Equals(ruleByTag.rule))
				{
					listRulesToDelete.Add(r);
				}
			}
		}
		listRulesByTag.Remove(ruleByTag);
	}

	public void AddRule(Rule rule)
	{
		if (rule as RuleDistance != null)
			listRules.Add(new RuleDistance((RuleDistance)rule));
		else if (rule as RuleCollision != null)
			listRules.Add(new RuleCollision((RuleCollision)rule));
		else if (rule as RuleCount != null)
			listRules.Add(new RuleCount((RuleCount)rule));
	}

	public void AddRuleByTag(Rule rule, string tag)
	{
		if (!GameServer.instance.Connected())
		{
			GameClient.instance.SendRuleTag(rule, tag);
			return ;
		}
		Actor[] actors;

		RuleByTag ruleByTag = new RuleByTag();
		ruleByTag.rule = rule;
		ruleByTag.tag = tag;
		listRulesByTag.Add(ruleByTag);
		actors = (Actor[])FindObjectsOfType(typeof(Actor));
		foreach (Actor actor in actors)
		{
			if (actor.tags.Contains(tag))
			{
				rule.to = actor;
				AddRule(rule);
			}
		}
	}

	public void RuleCollisionSet(Actor from, Actor to, bool activate)
	{
		foreach (Rule rule in listRules)
		{
			if (rule as RuleCollision != null)
			{
				if (rule.from == from && rule.to == to)
				{
					if (activate)
						rule.numEvents++;
					rule.activate = activate;
				}
			}
		}
	}

	public bool HaveMoveRule(Actor actor)
	{
		foreach (Rule rule in listRules)
		{
			if (rule.from == actor && rule.IsActivate() && rule.action == FollowActor)
				return true;
		}
		return false;
	}

# region ACTION

	public void LookActor(Actor from, Actor to)
	{
		Vector3 targetPos = new Vector3(to.transform.position.x, from.transform.position.y, to.transform.position.z);
		from.transform.LookAt(targetPos);
	}

	public void DestroyActor(Actor from, Actor to)
	{
		int index = (ActorManager.instance.listActors.IndexOf(to));
		GameServer.instance.SendRemoveActor(index);
		actorToDelete.Add(to);
	}

	public void SpawnActor(Actor from, Actor to)
	{
		if (ActorManager.instance.listActors.Count > 50)
			return ;
		Rule rule = new Rule();
		rule.from = from;
		rule.to = to;
		actorToSpawn.Add(rule);
	}

	public void FollowActor(Actor from, Actor to)
	{
		from.navMeshAgent.SetDestination(to.transform.position);
	}

	public void LookAwayActor(Actor from, Actor to)
	{
		Vector3 targetPos = new Vector3(to.transform.position.x, from.transform.position.y, to.transform.position.z);
		from.transform.LookAt(targetPos);

		Vector3 rot = from.transform.rotation.eulerAngles;
		rot = new Vector3(rot.x, rot.y + 180, rot.z);
		from.transform.rotation = Quaternion.Euler(rot);
	}

	public void CopyDirectionActor(Actor from, Actor to)
	{
		from.transform.rotation = to.transform.rotation;
	}

	public void CopyOppositeDirectionActor(Actor from, Actor to)
	{
		from.transform.rotation = to.transform.rotation;

		Vector3 rot = from.transform.rotation.eulerAngles;
		rot = new Vector3(rot.x, rot.y + 180, rot.z);
		from.transform.rotation = Quaternion.Euler(rot);
	}

	public void AscendActor(Actor from, Actor to)
	{
		to.BaseOffset += 0.1f;
	}

	public void DescendActor(Actor from, Actor to)
	{
		to.BaseOffset -= 0.1f;
	}

	public void IncreaseSpeedActor(Actor from, Actor to)
	{
		to.Speed += 0.1f;
	}

	public void DecreaseSpeedActor(Actor from, Actor to)
	{
		to.Speed -= 0.1f;
	}

	public void IncreaseScaleActor(Actor from, Actor to)
	{
		float scale = 0.1f;
		to.transform.localScale += new Vector3(scale, scale, scale);
		to.transform.localScale = VectorClamp(to.transform.localScale, 0.3f, 10.0f);
	}

	public void DecreaseScaleActor(Actor from, Actor to)
	{
		float scale = 0.1f;
		to.transform.localScale -= new Vector3(scale, scale, scale);
		to.transform.localScale = VectorClamp(to.transform.localScale, 0.3f, 10.0f);
	}

#endregion

	Vector3 VectorClamp(Vector3 v, float min, float max)
	{
		v.x = Mathf.Clamp(v.x, min, max);
		v.y = Mathf.Clamp(v.y, min, max);
		v.z = Mathf.Clamp(v.z, min, max);
		return v;
	}
}