using System.Collections;
using System.Collections.Generic;
using System;

public enum RuleType
{
	DISTANCE,
	COLLISION,
	COUNT
}

public enum RuleCountType
{
	DISTANCE,
	TIME_BIRTH,
	NUM_ACTIONS,
	TIME_BETWEEN_ACTION
}

public class RuleByTag
{
	public Rule rule;
	public string tag;
}

public class Rule
{
	public Actor from;
	public Actor to;
	public Action<Actor,Actor> action;
	public bool activate = false;
	public int numEvents;
	public int numEventActivate;

	public Rule()
	{

	}

	public Rule(Rule rule)
	{
		this.from = rule.from;
		this.to = rule.to;
		this.action = rule.action;
		this.activate = rule.activate;
		this.numEvents = rule.numEvents;
		this.numEventActivate = rule.numEventActivate;
	}

	public bool IsActivate()
	{
		return (activate && (numEventActivate == 0 || (numEvents % numEventActivate) == 0));
	}

	public void Check(bool needActivate)
	{
		if (!activate && needActivate)
		{
			numEvents++;
			if (numEventActivate == 0 || (numEvents % numEventActivate) == 0)
				from.power++;
		}
		activate = needActivate;
	}

	public override bool Equals(Object obj)
   {
      if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
      {
         return false;
      }
      else {
         Rule p = (Rule) obj;
         return (from == p.from) && (action == p.action);
      }
   }
}

public class RuleDistance : Rule
{
	public float distance;

	public RuleDistance() : base()
	{
	}

	public RuleDistance(RuleDistance rule) : base(rule)
	{
		this.distance = rule.distance;
	}

	public override bool Equals(Object obj)
    {
        RuleDistance rd = obj as RuleDistance;
        if (rd == null)
            return false;
        else
            return base.Equals((RuleDistance)obj) && distance == rd.distance;
    }
}

public class RuleCollision : Rule
{
	public RuleCollision() : base()
	{
	}

	public RuleCollision(RuleCollision rule) : base(rule)
	{
	}
}

public class RuleCount : Rule
{
	public float tmp;
	public float count;
	public RuleCountType type;

	public RuleCount() : base()
	{
	}

	public RuleCount(RuleCount rule) : base(rule)
	{
		this.tmp = rule.tmp;
		this.count = rule.count;
		this.type = rule.type;
	}

	public override bool Equals(Object obj)
    {
        RuleCount rd = obj as RuleCount;
        if (rd == null)
            return false;
        else
            return base.Equals((RuleCount)obj) && count == rd.count && type == rd.type;
    }
}