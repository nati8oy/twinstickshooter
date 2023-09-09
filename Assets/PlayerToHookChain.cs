using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToHookChain : MonoBehaviour
{
    [SerializeField] private SpringJoint joint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform chainStartPoint;
    [SerializeField] private Transform chainEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.enabled = true;

        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        //set the line renderer's first position to the chain start point
        lineRenderer.SetPosition(0, chainStartPoint.position);
        //set line renderer's last position to the chain end point
        lineRenderer.SetPosition(positions.Length-1, chainEndPoint.position);

        foreach(Vector3 position in positions)
        {
            
        }
    }
}
