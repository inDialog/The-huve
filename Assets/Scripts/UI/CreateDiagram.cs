using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class CreateDiagram : MonoBehaviour
{
    public UILineRenderer LineRenderer; 
    public UIPolygon uIPolygon;
    public Material mat;
    public List<Vector2> positions = new List<Vector2>();
    public Vector2 [] positionArr;
    bool active;

    private void LateUpdate()
    {
        positionArr  = uIPolygon.positions.ToArray();

        if (active == false)
        {

            for (int i = 0; i < positionArr.Length; i++)
            {
                if (positionArr[i] != new Vector2(0, 0))
                    positions.Add(new Vector2(0, 0));
                    positions.Add(positionArr[i]);
                if(i==positionArr.Length-1)
                    active = true;

            }
        }
        LineRenderer.gameObject.SetActive(false);
        LineRenderer.gameObject.SetActive(true);

        LineRenderer.Points = positions.ToArray(); 
    }

}