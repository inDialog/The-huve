using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UiLineGenerator2 : MonoBehaviour
{
    #region Private Methods
    Ui_Utilety ui_Utilety;
    ToggleGroup[] AllGroups;
    Toggle[] allToggles;
    Vector2[] position;
    #endregion
    #region Public Fields
    int listSize;
    [Header("Active functions-----call in other scripts")]
    public List<Toggle> activeTogles;
    [Header("The side options for the functions")]
    public GameObject map;
    public GameObject tagSelector;
    [Space(10)]
    public UnityEngine.UI.Extensions.UILineRenderer LineRenderer; // Assign Line Renderer in editor
    #endregion


    // Use this for initialization
    void Start()
    {
        ui_Utilety = GetComponent<Ui_Utilety>();
        AllGroups = GetComponentsInChildren<ToggleGroup>();
        allToggles = new Toggle[AllGroups.Length];
    }

    // Update is called once per frame
    void Update()
    {
        ActiveToggles();
        GenerateList();

        if (ui_Utilety.tagSelectorOn) listSize = activeTogles.Count + 1;
        else listSize = activeTogles.Count;

        if (ui_Utilety.mapSelectorOn) listSize = activeTogles.Count + 1;
        else if (!ui_Utilety.tagSelectorOn) listSize = activeTogles.Count;

        position = new Vector2[listSize];
        for (int i = 0; i < activeTogles.Count; i++)
        {
            //position[i] = activeTogles[i].gameObject.GetComponent<RectTransform>().anchoredPosition;
            if (activeTogles[i].transform.parent.tag == "ui")
            {
                Debug.Log("im selected");
                position[i] = activeTogles[i].gameObject.transform.parent.localPosition;

            }else
            position[i] = activeTogles[i].gameObject.transform.localPosition;

        }

        if (ui_Utilety.tagSelectorOn) position[listSize - 1] = tagSelector.transform.localPosition;
        if (ui_Utilety.mapSelectorOn) position[listSize - 1] = map.transform.localPosition;

        LineRenderer.Points = position.ToArray();

    }


    void ActiveToggles(){
        for (int i = 0; i < AllGroups.Length; i++)
        {
            if (AllGroups[i] != null)
            {
                allToggles[i] = AllGroups[i].GetActive();
            }
        }
    }


    public void GenerateList()
    {
        activeTogles = new List<Toggle>();
        activeTogles.AddRange(allToggles);
        List<Toggle> OnlyActive = new List<Toggle>();
        OnlyActive.AddRange(activeTogles);
        OnlyActive = OnlyActive.Where(item => item != null).ToList();
        activeTogles = OnlyActive;
    }
 }

    

 

public static class ToggleGroupExtension
{
    public static Toggle GetActive(this ToggleGroup aGroup)
    {
        return aGroup.ActiveToggles().FirstOrDefault();
    }
}