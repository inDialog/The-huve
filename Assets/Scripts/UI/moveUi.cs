using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class moveUi : MonoBehaviour
{
    public Canvas myCanvas;
   public bool itemSelected;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    GameObject target;

    void Start()
    {
        CamCustom.toggleCamMapEvent += ToggleCamMap;
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();


            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count != 0 & itemSelected==false)
            {
                if (results[0].gameObject.GetComponent<UiNode>())
                {
                    target = results[0].gameObject;
                    itemSelected = true;
                }
            }

            if (target == null) return;
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, Camera.main, out pos);
            target.transform.position = transform.TransformPoint(pos);

        }
        else
        {
            target = null;
            itemSelected = false;
        }
    }

    void ToggleCamMap(bool state)
    {
        myCanvas.gameObject.SetActive(!state);
    }
}
