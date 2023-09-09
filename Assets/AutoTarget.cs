using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{
    [SerializeField] private bool closestTarget;
    [SerializeField] private bool furthestTarget;
    [SerializeField] private GameObject[] targets;
    //[SerializeField] private GameObject currentTarget;
    [SerializeField] private float attackRange = 10f;
    public Vector3 attackDirection;

    [SerializeField] private GameObject closestObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

            //this shows the direction that will be used by another script for auto targeting
            attackDirection = (closestObject.transform.position - transform.position).normalized;

            //drag a debug line to the closest enemy
            Debug.DrawLine(transform.position, closestObject.transform.position, Color.green);
        }



       


    }

}
