using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorManager : MonoBehaviour
{
	public static event Action<Actor> changeActorAfterEvent;
	public static event Action<Actor> changeActorBeforeEvent;
	public static ActorManager instance;
	public Actor actorSelected;
	public List<Actor> listActors;
	public List<Actor> listActorsToDelete;
	public List<Actor> listActorsToSpawn;
	public List<Actor> listActorsOrigin;

	void Awake()
	{
		instance = this;
		Actor[] actors = (Actor[])FindObjectsOfType(typeof(Actor));
		listActors = new List<Actor>(actors);
		if (listActors.Count > 0)
			actorSelected = listActors[0];
		listActors.Sort((x, y) => {return string.Compare(x.name, y.name);});
		listActorsOrigin = new List<Actor>();
		foreach (Actor actor in listActors)
		{
			Actor res = Instantiate(actor);
			listActorsOrigin.Add(res);
			actor.origin = res;
			res.name += " - Origin";
			res.gameObject.SetActive(false);
		}
		listActorsToDelete = new List<Actor>();
		listActorsToSpawn = new List<Actor>();
	}

	void Start()
	{
		CamCustom.changeActorAfterEvent += ChangeActorSelected;
	}

	void Update()
	{
		foreach (Actor actor in listActorsToSpawn)
		{
			Actor get = Instantiate(actor);
			get.navMeshAgent.enabled = false;
			get.gameObject.SetActive(true);
			listActors.Add(get);
		}
		listActorsToSpawn.Clear();
		foreach (Actor actor in listActorsToDelete)
		{
			Destroy(actor.gameObject);
			listActors.Remove(actor);
		}
		listActorsToDelete.Clear();
	}

	public void RemoveActor(int index)
	{
		listActorsToDelete.Add(listActorsOrigin[index]);
	}

	public void SpawnActor(int index)
	{
		listActorsToSpawn.Add(listActorsOrigin[index]);
	}

	void ChangeActorSelected(Actor actor)
	{
		if (changeActorBeforeEvent != null)
			changeActorBeforeEvent(actorSelected);
		actorSelected = actor;
		if (changeActorAfterEvent != null)
			changeActorAfterEvent(actor);
	}

	public List<string> GetAllTags()
	{
		List<string> listTags = new List<string>();
		foreach (Actor actor in listActors)
		{
			foreach (string tag in actor.tags)
			{
				if (!listTags.Contains(tag))
					listTags.Add(tag);
			}
		}
		return (listTags);
	}
}
