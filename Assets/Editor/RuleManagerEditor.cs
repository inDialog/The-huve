using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

enum TriggerType
{ DISTANCE, COLLISION, COUNT };

[CustomEditor(typeof(RuleManager))]
public class RuleManagerEditor : Editor
{
	RuleManager ruleManager;
	TriggerType trigerType;
	RuleDistance ruleDistance;
	RuleCollision ruleCollision;
	RuleCount ruleCount;
	string[] actionName;
	int numAction;
	string tag;
	bool self;

	void OnEnable()
	{
		ruleManager = (RuleManager)target;
		ruleDistance = new RuleDistance();
		ruleCollision = new RuleCollision();
		ruleCount = new RuleCount();
		List<Action<Actor, Actor>> listActions = ruleManager.listActions;

		actionName = new string[listActions.Count];
		for (int i = 0; i < listActions.Count; i++)
			actionName[i] = listActions[i].Method.Name;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		DisplayRule();
		AddRule();
	}

	void DisplayRule()
	{
		if (ruleManager.listRules == null)
			return ;
		foreach (Rule rule in ruleManager.listRules)
		{
			GUILayout.Label(String.Format("{0} | {1} -> {2} | {3} ", rule.GetType().Name, rule.from.name, rule.to.name, rule.action.Method.Name));
		}
	}

	void DefaultLayout(Rule rule)
	{
		rule.from = EditorGUILayout.ObjectField("from", rule.from, typeof(Actor), true) as Actor;
		self = EditorGUILayout.Toggle("self", self);
		if (!self)
			tag = EditorGUILayout.TextField("tag", tag);
		rule.numEventActivate = EditorGUILayout.IntField("num triggers", rule.numEventActivate);
		numAction = EditorGUILayout.Popup(numAction, actionName);
		rule.action = ruleManager.listActions[numAction];
	}

	void CreateButton(Rule rule)
	{
		if (GUILayout.Button("Create Rule"))
		{
			if (self)
			{
				rule.to = rule.from;
				ruleManager.AddRule(rule);
			}
			else
				ruleManager.AddRuleByTag(rule, tag);
		}
	}

	void AddRule()
	{
		trigerType = (TriggerType)EditorGUILayout.EnumPopup("type", trigerType);
		switch (trigerType)
		{
			case TriggerType.DISTANCE:
				DefaultLayout(ruleDistance);
				ruleDistance.distance = EditorGUILayout.FloatField("distance", ruleDistance.distance);
				CreateButton(ruleDistance);
				break ;
			case TriggerType.COLLISION:
				DefaultLayout(ruleCollision);
				CreateButton(ruleCollision);
				break ;
			case TriggerType.COUNT:
				DefaultLayout(ruleCount);
				ruleCount.type = (RuleCountType)EditorGUILayout.EnumPopup("type count", ruleCount.type);
				ruleCount.count = EditorGUILayout.FloatField("count", ruleCount.count);
				CreateButton(ruleCount);
				break ;
		}
	}
}