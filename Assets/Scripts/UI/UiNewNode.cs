using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiNewNode : MonoBehaviour
{
    public GameObject nodePrefab;
    public Transform nodes;
    public Transform spawnPoint;
    Dropdown dropdown;
    RuleManager ruleManager;
    ActorManager actorManager;
    List<String> actionString;

    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        actionString = new List<string>();
    }

    void Start()
    {
        actorManager = ActorManager.instance;
        ruleManager = RuleManager.instance;
        foreach (Action<Actor,Actor> action in ruleManager.listActions)
            actionString.Add(action.Method.Name);
        dropdown.AddOptions(actionString);

        ActorManager.changeActorAfterEvent += LoadActor;
        ActorManager.changeActorBeforeEvent += BeforeChangeActor;
    }

    public void CreateNewNode()
    {
        GameObject goNode = Instantiate(nodePrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, nodes);

        UiNode node = goNode.GetComponent<UiNode>();
        node.SetName(actionString[dropdown.value]);
        node.SetAction(ruleManager.listActions[dropdown.value]);
    }

    public void BeforeChangeActor(Actor actor)
    {
        foreach (Transform node in nodes)
        {
            if (node.gameObject.activeSelf)
            {
                actor.listNodes.Add(node.gameObject);
                node.gameObject.SetActive(false);
            }
        }
    }

    public void LoadActor(Actor actor)
    {
        foreach (GameObject node in actor.listNodes)
            node.gameObject.SetActive(true);
    }
}