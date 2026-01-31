using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{
    [SerializeField] private bool lockToClosestTarget;
    [SerializeField] private bool furthestTarget;
    [SerializeField] private bool hookshotAutoTargeting;

    [SerializeField] private GameObject[] targets;
    [SerializeField] private GameObject[] grapplePoints;
    //[SerializeField] private GameObject currentTarget;
    [SerializeField] private float attackRange = 10f;
    public Vector3 attackDirection;
    public Vector3 grappleDirection;

     public GameObject closestTarget;
    public GameObject closestGrapplePoint;


    private void Start()
    {
        FindClosestTarget();

        //run this as a coroutine to save a bit of processing
        StartCoroutine(FindClosestTargetRoutine());
    }


    private IEnumerator FindClosestTargetRoutine()
    {

        //if the closest object is not null then run this coroutine
        if (closestTarget)
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while (true)
            {
                yield return wait;
                FindClosestTarget();
            }
        }
        
    }


    private void FindClosestTarget()
    {
        //fill the arrawy with the enemy game objects
        targets = GameObject.FindGameObjectsWithTag("enemy");

        float closestDistance = Mathf.Infinity;

        foreach (GameObject targetEnemy in targets)
        {

            if (targetEnemy != null)
            {
                // Calculate the distance between the player and the current object.
                float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

                // Check if this object is closer than the previously closest object.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = targetEnemy;
                    //draw a debug from the player to the current target
                }
               
            }
            else
            {
                closestTarget = null;
            }
        }


        if (targets.Length > 0)
        {
            //this shows the direction that will be used by another script for auto targeting
            attackDirection = (closestTarget.transform.position - transform.position).normalized;


            //drag a debug line to the closest enemy
            Debug.DrawLine(transform.position, closestTarget.transform.position, Color.green);

        }
        else
        {
            attackDirection = transform.forward;
        }
    }

    public void FindClosestGrapplePoint()
    {


        //get the layer the grapple point object is on
        //fill the arrawy with the enemy game objects
        grapplePoints = GameObject.FindGameObjectsWithTag("grapple point");

        float closestDistanceToGrapplePoint = Mathf.Infinity;

        foreach (GameObject grapplePoint in grapplePoints)
        {

            if (grapplePoint != null)
            {
                // Calculate the distance between the player and the current object.
                float distance = Vector3.Distance(transform.position, grapplePoint.transform.position);

                // Check if this object is closer than the previously closest object.
                if (distance < closestDistanceToGrapplePoint)
                {
                    closestDistanceToGrapplePoint = distance;
                    closestGrapplePoint = grapplePoint;
                    Debug.Log("closest grapple point is " + closestGrapplePoint);
                    //draw a debug from the player to the current target
                }

            }
            else
            {
                closestGrapplePoint = null;
            }
        }


        if (grapplePoints.Length > 0)
        {
            //this shows the direction that will be used by another script for auto targeting
            grappleDirection = (closestTarget.transform.position - transform.position).normalized;


            //drag a debug line to the closest enemy
            Debug.DrawLine(transform.position, closestTarget.transform.position, Color.magenta);

        }
        else
        {
            //grappleDirection = transform.forward;
        }
    }



}
