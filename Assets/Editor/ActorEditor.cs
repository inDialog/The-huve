using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

[CustomEditor(typeof(Actor))]
public class ActorEditor : Editor
{
	Actor actor;

	void OnEnable()
	{
		actor = (Actor)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Set Waypoint"))
			actor.waitWaypoint = true;
	}
}