using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    [Header("Field of View")]
    //public GameObject targetRefObject;
    public float angle;
    public float radius;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;
    public bool canSeeTarget;


    void Start()
    {
        /*
        if(GetComponent<AutoTarget>() != null)
        {
            targetRef = GetComponent<AutoTarget>().closestObject;
        } else
        {
            //targetRef = null;
            Debug.LogError("No AutoTarget script found on this object");
        }*/

        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        //check it's not null

        var targetRef = GetComponent<AutoTarget>().closestTarget;
        
        /*
        if (GetComponent<AutoTarget>() != null)
        {
            targetRef = GetComponent<AutoTarget>().closestObject;

        }
      */

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);


        if (rangeChecks.Length != 0)
        {
            if(targetRef != null) { 
            Transform targetObject = targetRef.transform;

                Vector3 directionToTarget = (targetObject.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetObject.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                       // Debug.Log("Target in range");
                        canSeeTarget = true;

                    }
                    else
                    {
                       // Debug.Log("Target out of range");
                        canSeeTarget = false;
                    }
                }
                else
                {
                    //Debug.Log("Target out of range");
                    canSeeTarget = false;
                }
            }

        }
        else
        {
            canSeeTarget = false;
        }
    }
}
