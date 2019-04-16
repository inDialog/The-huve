using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiNode : MonoBehaviour
{
    public UiGraphics uiGraphics;
    public Text text;
    Action<Actor,Actor> action;
    RuleManager ruleManager;
    ActorManager actorManager;
    Rule currentRule;

    public void Awake()
    {
    }

    public void Start()
    {
        ruleManager = RuleManager.instance;
        actorManager = ActorManager.instance;
    }

    public void SetName(string name)
    {
        text.text = name;
    }

    public void SetAction(Action<Actor,Actor> newAction)
    {
        action = newAction;
    }

	public string Tag
    {
        get {return tag;}
		set {tag = value;}
    }

    public void ToggleActivate(bool state)
    {
        if (state)
        {
            if (uiGraphics.textDistance)
            {
                RuleDistance rule = new RuleDistance();
                rule.from = ActorManager.instance.actorSelected;
                rule.action = action;
                rule.distance = float.Parse(uiGraphics.textDistance.text);
                rule.numEventActivate = uiGraphics.NumTrigger;
                ruleManager.AddRuleByTag(rule, uiGraphics.Tag);
                currentRule = rule;
            }
            else if (uiGraphics.textCount)
            {
                RuleCount rule = new RuleCount();
                rule.from = actorManager.actorSelected;
                rule.action = action;
                rule.count = float.Parse(uiGraphics.textCount.text);
                rule.type = uiGraphics.Type;
                rule.numEventActivate = uiGraphics.NumTrigger;
                ruleManager.AddRuleByTag(rule, uiGraphics.Tag);
                currentRule = rule;
            }
            else
            {
                RuleCollision rule = new RuleCollision();
                rule.from = actorManager.actorSelected;
                rule.action = action;
                rule.numEventActivate = uiGraphics.NumTrigger;
                ruleManager.AddRuleByTag(rule, uiGraphics.Tag);
                currentRule = rule;
            }
        }
        else
        {
            ruleManager.DeleteRule(currentRule, uiGraphics.Tag);
        }
    }
}