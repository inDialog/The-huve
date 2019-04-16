using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

[CustomEditor(typeof(CamCustom))]
public class CamCustomEditor : Editor
{
	CamCustom camCustom;

	void OnEnable()
	{
		camCustom = (CamCustom)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Next Actor"))
			camCustom.NextActor();
		if (GUILayout.Button("Prev Actor"))
			camCustom.PrevActor();
	}
}