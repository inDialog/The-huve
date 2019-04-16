using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MoveType
{
	PASSIVE,
	RANDOM_DIRECTION,
	CONSTANT_FORWARD,
	FOLLOW_ACTOR,
	PATROL_WAYPOINT
}

public class Actor : MonoBehaviour
{
	public List<string> tags;
	public bool waitWaypoint;
	public MoveType moveType;
	public Actor actorFollowed;
	public float distanceWalked;
	public float totalDistanceWalked;
	public float totalTimeElapse;
	public int power;
	Vector3 destination;
	public Vector3 syncPos;
	public Quaternion syncRot;
	public NavMeshAgent navMeshAgent;
	public List<Vector3> waypoint;
	public Actor origin;
	int indexWaypoint;
	RuleManager ruleManager;
	Vector3 startPosition;
	List<Vector3> listLastPosition;
	Vector3 lastPosition;
	public float syncTime = 2.0f;
	public List<GameObject> listNodes;

	void Awake()
	{
		waypoint = new List<Vector3>();
		listLastPosition = new List<Vector3>();
		listNodes = new List<GameObject>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	void Start()
	{
		ruleManager = RuleManager.instance;
		indexWaypoint = 0;
		destination = transform.position;
		startPosition = transform.position;
		lastPosition = transform.position;
		distanceWalked = 0;
		totalDistanceWalked = 0;
		totalTimeElapse = 0;
		power = 0;
	}

	void Update()
	{
		if (!GameServer.instance.Connected())
		{
			transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * syncTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, Time.deltaTime * syncTime);
			return ;
		}
		if (waitWaypoint && Input.GetMouseButtonDown(0))
		{
			if (AddWaypoint())
				waitWaypoint = false;
		}
		MoveProcess();
		totalTimeElapse += Time.deltaTime;
		distanceWalked = Vector3.Distance(transform.position, lastPosition);
		totalDistanceWalked += distanceWalked;
		lastPosition = transform.position;
		listLastPosition.Add(lastPosition);
		if (listLastPosition.Count > 30)
			listLastPosition.RemoveAt(0);
	}

	void MoveProcess()
	{
		if (ruleManager.HaveMoveRule(this))
		{
			navMeshAgent.updateRotation = false;
			return ;
		}
		navMeshAgent.updateRotation = true;
		if (moveType == MoveType.PASSIVE)
		{
			navMeshAgent.SetDestination(transform.position);
		}
		else
		{
			if (moveType == MoveType.PATROL_WAYPOINT)
				MoveToWaypoint();
			else if (moveType == MoveType.CONSTANT_FORWARD)
				MoveConstantForward();
			else if (moveType == MoveType.RANDOM_DIRECTION)
				MoveRandomDirection();
			else if (moveType == MoveType.FOLLOW_ACTOR)
				MoveFollowActor();
		}
	}

	void MoveFollowActor()
	{
		if (actorFollowed)
			navMeshAgent.SetDestination(actorFollowed.transform.position);
	}

	void MoveRandomDirection()
	{
		Vector3 vecToTarget = transform.position - destination;
		vecToTarget.y = 0;
		float dist = vecToTarget.magnitude;

		if (dist < 1.5f || IsStopped())
		{
			float roamRadius = 50.0f;
			Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
			randomDirection += startPosition;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
			destination = hit.position;
			navMeshAgent.SetDestination(destination);
		}
	}

	void MoveConstantForward()
	{
		Vector3 vecToTarget = transform.position - destination;
		vecToTarget.y = 0;
		float dist = vecToTarget.magnitude;
		if (dist < 1.5f)
			destination = transform.position + transform.forward * 10.0f;
		navMeshAgent.SetDestination(destination);
	}

	void MoveToWaypoint()
	{
		if (waypoint.Count == 0)
			return ;
		if (Vector3.Distance(transform.position, waypoint[indexWaypoint]) < 1.5f)
		{
			if (waypoint.Count > 1)
				indexWaypoint = (indexWaypoint + 1) % waypoint.Count;
			else
				return ;
		}
		navMeshAgent.SetDestination(waypoint[indexWaypoint]);
	}

	public bool AddWaypoint()
	{
		RaycastHit hit;
		Ray ray = CamCustom.instance.camMap.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			if (GameServer.instance.Connected())
				waypoint.Add(hit.point);
			else
				GameClient.instance.SendWayPoint(this, hit.point);
			return (true);
		}
		return (false);
	}

	bool IsStopped()
	{
		if (listLastPosition.Count < 30)
			return false;
		for (int i = 0 ; i < listLastPosition.Count; i++)
		{
			if (listLastPosition[i] != lastPosition)
				return false;
		}
		return true;
	}

	public float Speed
	{
		get { return navMeshAgent.speed; }
		set
		{
			navMeshAgent.speed = Mathf.Clamp(value, 0.1f, 4.0f);
		}
	}

	public float BaseOffset
	{
		get { return navMeshAgent.baseOffset; }
		set
		{
			navMeshAgent.baseOffset = Mathf.Clamp(value, 0.5f, 15.0f);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		if (waitWaypoint)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
				Gizmos.DrawSphere(hit.point, 0.3f);
		}
		if (waypoint == null)
			return ;
		Gizmos.color = Color.red;
		for (int i = 0; i < waypoint.Count; i++)
		{
			Gizmos.DrawSphere(waypoint[i], 0.3f);
			Gizmos.DrawLine(waypoint[i], waypoint[(i + 1) % waypoint.Count]);
		}
	}

	void OnTriggerEnter(Collider colliderInfo)
	{
		Actor to;
		to = colliderInfo.gameObject.GetComponent<Actor>();
		if (to)
			ruleManager.RuleCollisionSet(this, to, true);
	}

	void OnTriggerExit(Collider colliderInfo)
	{
		Actor to;
		to = colliderInfo.gameObject.GetComponent<Actor>();
		if (to)
			ruleManager.RuleCollisionSet(this, to, false);
	}
}