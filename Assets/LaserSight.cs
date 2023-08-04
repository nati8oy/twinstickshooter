using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))] 
public class LaserSight : MonoBehaviour
{
    
    private LineRenderer lineRenderer;

    [SerializeField] private Transform startPoint;
    [SerializeField] private float sightLength = 3f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
       // lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    private void LateUpdate()
    {

        //use the linerenderer to draw a line from the start point to the end point
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, gameObject.transform.forward * sightLength);

        }
       
    }

}
