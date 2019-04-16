using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class Ui_Utilety : MonoBehaviour
{
	public static Ui_Utilety instance;
	public ToggleGroup[] allGroups;
	UiLineGenerator2 uiLineGenerator;
	bool toggle;
	public bool tagSelectorOn , mapSelectorOn;
	ActorManager actorManager;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		allGroups = GetComponentsInChildren<ToggleGroup>();
		uiLineGenerator = GetComponent<UiLineGenerator2>();
		actorManager = ActorManager.instance;
		ActorManager.changeActorAfterEvent += ChargeActor;
	}

	public void ToggleApply(bool b)
	{
		if (!allGroups[0].AnyTogglesOn())
		{
			foreach (var item in allGroups)
			{
				item.SetAllTogglesOff();
			}
			return;
		}

		if (!allGroups[1].AnyTogglesOn())
		{
			allGroups[2].SetAllTogglesOff();
			allGroups[3].SetAllTogglesOff();
		}

		if (!allGroups[2].AnyTogglesOn())
		{
			allGroups[4].SetAllTogglesOff();
			allGroups[5].SetAllTogglesOff();

		}

		MoveType moveType = actorManager.actorSelected.moveType;
		foreach (ToggleGroup item in allGroups)
		{
			if(item.GetActive() != null)
			{
				if (item.GetActive().name == "M_Ui_static")
				{
					allGroups[1].SetAllTogglesOff();
					allGroups[2].SetAllTogglesOff();
					allGroups[3].SetAllTogglesOff();
					allGroups[4].SetAllTogglesOff();
					allGroups[5].SetAllTogglesOff();
					moveType = MoveType.PASSIVE;
				}
				if (item.GetActive().name == "M_Ui_dynamic")
				{
					allGroups[6].SetAllTogglesOff();
				}
				if (item.GetActive().name == "M_Ui_directional")
				{
					allGroups[3].SetAllTogglesOff();
				}
				if (item.GetActive().name == "M_Ui_Patrol")
				{
					allGroups[2].SetAllTogglesOff();
					allGroups[4].SetAllTogglesOff();
					allGroups[5].SetAllTogglesOff();
					allGroups[6].SetAllTogglesOff();
				}
				if (item.GetActive().name == "M_Ui_Random")
				{
					allGroups[5].SetAllTogglesOff();
					allGroups[6].SetAllTogglesOff();
					moveType = MoveType.RANDOM_DIRECTION;
				}
				if (item.GetActive().name == "M_Ui_foreword")
				{
					allGroups[4].SetAllTogglesOff();
					allGroups[6].SetAllTogglesOff();
					moveType = MoveType.CONSTANT_FORWARD;
				}
				if (item.GetActive().name == "M_Ui_object")
				{
					tagSelectorOn = true;
				}
				else
				{
					tagSelectorOn = false;
				}
				if (item.GetActive().name == "M_UI_waypoint")
				{
					mapSelectorOn = true;
					moveType = MoveType.PATROL_WAYPOINT;
				}
				else
				{
					mapSelectorOn = false;
				}
			}
		}
		if (GameServer.instance.Connected())
			actorManager.actorSelected.moveType = moveType;
		else
			GameClient.instance.SendMoveType(actorManager.actorSelected, moveType);
	}

	public void ChargeActor(Actor actor)
	{
		// if (!UiManager.instance.ui1.activeSelf)
		// 	return ;
		List<string> targetName = new List<string>();
		if (actor.moveType == MoveType.RANDOM_DIRECTION)
		{
			targetName.Add("M_Ui_dynamic");
			targetName.Add("M_Ui_directional");
			targetName.Add("M_Ui_Random");
		}
		else if (actor.moveType == MoveType.CONSTANT_FORWARD)
		{
			targetName.Add("M_Ui_dynamic");
			targetName.Add("M_Ui_directional");
			targetName.Add("M_Ui_foreword");
		}
		else if (actor.moveType == MoveType.PASSIVE)
		{
			targetName.Add("M_Ui_static");
		}
		else if (actor.moveType == MoveType.PATROL_WAYPOINT)
		{
			targetName.Add("M_Ui_dynamic");
			targetName.Add("M_Ui_Patrol");
			targetName.Add("M_UI_waypoint");
		}
		Toggle[] toggles = GetComponentsInChildren<Toggle>();
		foreach (Toggle item in toggles)
		{
			if (targetName.Contains(item.name))
				item.isOn = true;
			else
				item.isOn = false;
		}
	}
}