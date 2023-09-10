using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{
    [SerializeField] private bool closestTarget;
    [SerializeField] private bool furthestTarget;
    [SerializeField] private bool hookshotAutoTargeting;

    [SerializeField] private GameObject[] targets;
    //[SerializeField] private GameObject currentTarget;
    [SerializeField] private float attackRange = 10f;
    public Vector3 attackDirection;

     public GameObject closestObject;


    private void Start()
    {
        FindClosestTarget();

        //run this as a coroutine to save a bit of processing
        StartCoroutine(FindClosestTargetRoutine());
    }


    private IEnumerator FindClosestTargetRoutine()
    {

        //if the closest object is not null then run this coroutine
        if (closestObject)
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
                    closestObject = targetEnemy;
                    //draw a debug from the player to the current target
                }
               
            }
            else
            {
                closestObject = null;
            }
        }


        if (targets.Length > 0)
        {
            //this shows the direction that will be used by another script for auto targeting
            attackDirection = (closestObject.transform.position - transform.position).normalized;


            //drag a debug line to the closest enemy
            Debug.DrawLine(transform.position, closestObject.transform.position, Color.green);

        }
        else
        {
            attackDirection = transform.forward;
        }
    }


    
}
