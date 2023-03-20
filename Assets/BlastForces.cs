using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastForces : MonoBehaviour
{
    public float pushForce = 10f; // The force with which to push objects

    public LayerMask layerMask;
    public float detectionRange = 5f;
    public float operationInterval;
    public bool canPush; 

    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
    void FixedUpdate()
    {

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, detectionRange, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * detectionRange, Color.yellow);
            Debug.Log("Did Hit");


            Vector3 pushDirection = (hit.transform.position - transform.position).normalized;
            GameObject hitObject = hit.collider.gameObject;


            if (canPush)
            {
                hitObject.GetComponent<Rigidbody>().AddForce(pushDirection * pushForce, ForceMode.Impulse);
                Debug.Log("pushing");
                StartCoroutine(PushCooldown());

            }
        }
        
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * detectionRange, Color.white);
            Debug.Log("Did not Hit");
        }
    }

    // Coroutine for timer
    private IEnumerator PushCooldown()
    {
        Debug.Log("Cooldown complete");
        canPush = false;
        yield return new WaitForSeconds(operationInterval);
        canPush = true;
    }
}
