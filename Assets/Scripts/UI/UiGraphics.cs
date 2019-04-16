using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class UiGraphics : MonoBehaviour
{
    public Dropdown dropdownTag;
    public Dropdown dropdownCountType;
    public Text textDistance;
    public Text textCount;
    public Text textNumTrigger;
    public List<GameObject> actions = new List<GameObject>();

    public UnityEngine.UI.Extensions.UILineRenderer LineRenderer; // Assign Line Renderer in editor
    int lengthOfLineRenderer;

    void Start()
    {
        dropdownTag.ClearOptions();
        dropdownTag.AddOptions(ActorManager.instance.GetAllTags());
    }

    void Update()
    {
        if (GetComponentInParent<moveUi>().itemSelected)
        {
            if (actions.Count == 0)
            {
                LineRenderer.Points = new Vector2[0];
                return;
            }
            LineRenderer.Points = BuildList().ToArray();
        }
    }




   public  List<Vector2> BuildList()
    {
        RectTransform rct = this.gameObject.GetComponent<RectTransform>();
        List<Vector2> positions = new List<Vector2>();
        Vector2 originalPosition = rct.anchoredPosition;
        Vector2 startPosition = new Vector2(originalPosition.x - rct.rect.width / 2.9f, originalPosition.y);
        Vector2 midPosition = new Vector2(startPosition.x - rct.rect.width / 25f, startPosition.y);


        for (int i = 0; i < actions.Count; i++)
        {
            if (!actions[i].activeInHierarchy)
                continue;
            Vector2 temp = new Vector2(0, 0);
            RectTransform tempRct = actions[i].GetComponent<RectTransform>();

            Vector2 end = new Vector2(tempRct.localPosition.x + tempRct.rect.width / 2.2f, tempRct.localPosition.y);
            Vector2 beforeEnd = new Vector2(end.x + tempRct.rect.width / 15f, end.y);
            positions.Add(startPosition);
            positions.Add(midPosition);
            positions.Add(beforeEnd);
            positions.Add(end);
            positions.Add(beforeEnd);
            positions.Add(midPosition);
            positions.Add(startPosition);
        }


        return positions;
    }

    public string Tag
    {
        get { return dropdownTag.options[dropdownTag.value].text; }
    }

    public int NumTrigger
    {
        get {
            int res;
            if (int.TryParse(textNumTrigger.text, out res))
                return res;
            return 1;
        }
    }

    public RuleCountType Type
    {
        get
        {
            int value = dropdownCountType.value;
            if (value == 1)
                return RuleCountType.DISTANCE;
            if (value == 2)
                return RuleCountType.TIME_BIRTH;
            if (value == 3)
                return RuleCountType.NUM_ACTIONS;
            if (value == 4)
                return RuleCountType.TIME_BETWEEN_ACTION;
            return RuleCountType.DISTANCE;
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!actions.Contains(col.gameObject))
            actions.Add(col.gameObject);
        UiNode node =  col.gameObject.GetComponent<UiNode>();
        if (node)
            node.uiGraphics = this;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        actions.Remove(col.gameObject);
        // UiNode node = col.GetComponent<UiNode>();
        // if (node)
        //     node.uiGraphics = null;
    }
}