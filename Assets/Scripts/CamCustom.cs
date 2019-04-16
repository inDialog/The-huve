using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CamCustom : MonoBehaviour
{
	public static CamCustom instance;
	public static event Action<Actor> changeActorAfterEvent;
	public static event Action<Actor> changeActorBeforeEvent;
	public static event Action<bool> toggleCamMapEvent;
	public Camera camMap;
	public Camera camUser;
	public ActorManager actorManager;
	public bool inCamMap;
	public float cameraSensitivity = 90;
	public float climbSpeed = 4;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;
	public bool actorFPS;
    public bool mainMenu;
    float rotationX = 0.0f;
	float rotationY = 0.0f;
	public Actor actorView;

	void Awake()
	{
		instance = this;
		inCamMap = false;
	}

	void Start()
	{
        actorManager = ActorManager.instance;
		actorFPS = true;
		NextActor();
	}

	void Update()
	{
            if (inCamMap)
            {
                if (Input.GetMouseButtonDown(0))
                    actorManager.actorSelected.AddWaypoint();
                if (Input.GetKeyDown(KeyCode.Escape))
                    ToggleCamMap();
            }
            if (actorFPS)
                ActorCam();
            else
                FreeCam();
        }
	
	void ActorCam()
	{
		if (actorView == null)
			return ;
		transform.position = actorView.transform.position;
		transform.rotation = actorView.transform.rotation;
	}

	void FreeCam()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
		}
		if (Cursor.lockState == CursorLockMode.None)
			return ;
		rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
		rotationY = Mathf.Clamp (rotationY, -90, 90);

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

	 	if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
	 	{
			transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
	 	else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
	 	{
			transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
	 	else
	 	{
	 		transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
	 	}
	}

	public void NextActor()
	{
		if (changeActorBeforeEvent != null)
			changeActorBeforeEvent(actorView);
		Actor[] actors;
		actors = (Actor[])FindObjectsOfType(typeof(Actor));
		if (actors.Length == 0)
			return;
		if (actorView == null)
			actorView = actors[0];
		else
		{
			int index = Array.IndexOf(actors, actorView);
			actorView = actors[(index + 1) % actors.Length];
		}
		if (changeActorAfterEvent != null)
			changeActorAfterEvent(actorView);
	}

	public void PrevActor()
	{
		Actor[] actors;
		actors = (Actor[])FindObjectsOfType(typeof(Actor));
		if (actors.Length == 0)
			return;
		if (actorView == null)
			actorView = actors[0];
		else
		{
			int index = Array.IndexOf(actors, actorView);
			if (index == 0)
				index = actors.Length;
			actorView = actors[index - 1];
		}
		if (changeActorAfterEvent != null)
			changeActorAfterEvent(actorView);
	}

	public void ToggleCamMap()
	{
		inCamMap = !inCamMap;
		if (toggleCamMapEvent != null)
			toggleCamMapEvent(inCamMap);
	}
}