using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointGraphics : MonoBehaviour
{
    LineRenderer lineRenderer;
    Actor actor;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        actor = this.gameObject.GetComponentInParent<Actor>();
        Color color = new Color(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2), 1);
        lineRenderer.SetColors(color,color);
    }

    // Update is called once per frame
    void Update()
    {
        if (actor.waypoint.Count == 0) return;
        List<Vector3> line = actor.waypoint;
        List<Vector3> line2 = new List<Vector3>();
        lineRenderer.positionCount = actor.waypoint.Count-1;
        for (int i = 0; i < actor.waypoint.Count; i++)
        {
            line2.Add(new Vector3(line[i].x, line[i].y + 1, line[i].z));
        }
        lineRenderer.SetPositions(line2.ToArray());
    }
    
}
