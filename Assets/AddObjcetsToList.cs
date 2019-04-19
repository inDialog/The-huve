using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AddObjcetsToList : MonoBehaviour
{
    Dropdown m_Dropdown;
    public GameObject actorsManage;
    ActorManager manager;
    public List<Actor> actors = new List<Actor>();
    public List<string> actorsNames = new List<string>();
    public Actor toFallow;

    // Start is called before the first frame update
    void Start()
    {
        actors = actorsManage.GetComponent<ActorManager>().listActors;
        if (actors.Count == 0) return;
        for (int i = 0; i < actors.Count; i++)
        {
            actorsNames.Add(actors[i].ToString());
        }
        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.ClearOptions();
        m_Dropdown.AddOptions(actorsNames);
        m_Dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(m_Dropdown);
        });
    }

    // Update is called once per frame
    void Update()
    {
        //m_DropdownValue = m_Dropdown.value;
    }

    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log(m_Dropdown.value);
        Debug.Log(actors[m_Dropdown.value]);
        //Debug.Log(manager.actorSelected);
        toFallow = actors[m_Dropdown.value];
        //manager.actorSelected.actorFollowed = 

    }
}