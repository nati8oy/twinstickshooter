using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] LineRenderer lr;


    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
       // lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    private void LateUpdate()
    {

        //use the linerenderer to draw a line from the start point to the end point
        if (lr != null)
        {
            lr.SetPosition(0, startPoint.position);
            lr.SetPosition(1, endPoint.position);

        }
       
    }

}
