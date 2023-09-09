using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MomentumHookshot : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;


    private bool  hasFired = false;

    [SerializeField] private LayerMask layerMask;

    public Transform shotPoint;

    private Vector3 hookshotPosition;
    public Vector3 characterVelocityMomentum;

    [Header("Hookshot")]
    [SerializeField] private Transform hook;
    [SerializeField] private Transform hookHolder;

    private bool hitSomething = false;

    private RaycastHit raycastHit;

    [SerializeField] private SpringJoint joint;

    [SerializeField] private float hookshotMaxRange = 10f;

    public bool isGrappling = false;


    //used to check which arm is being used to grapple
    //0 means first arm

    [Header("Targeting")]
    public GameObject hitTarget;
    [SerializeField] private bool autoTarget;
    [SerializeField] GameObject[] targets;


    [Header("Hookshot Speed")]

    [SerializeField] private float hookshotSpeedMin = 10f;
    [SerializeField] private float hookshotSpeedMax = 40f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Physics.Raycast(shotPoint.position, shotPoint.forward, out raycastHit, hookshotMaxRange);

        // Check if the left mouse button is pressed and the fire rate cooldown has passed
        if (Mouse.current.rightButton.isPressed && !hasFired)
        {
            hasFired = true;
        }

        else if (!Mouse.current.rightButton.isPressed)
        {
            // Reset the flag when the button is released
            hasFired = false;
        }

        //set the hook transform to the hookshotMaxRange distance from the player when the right mouse button is held down
        if (hasFired)
        {
            hook.position = shotPoint.position + shotPoint.forward * hookshotMaxRange;
            //fire the hook from the shotpoint position to the hookshotMaxRange distance
            lr.SetPosition(0, shotPoint.position);
            lr.SetPosition(1, hook.position);

        } 
        else
        {
            hook.position = hookHolder.position;
        }
    }

    private void Shot()
    {
        if (Physics.Raycast(shotPoint.position, shotPoint.forward, out raycastHit, hookshotMaxRange))
        {

            //check the layer of the object that was hit
            //if the layer is grappleable, then set the hookshot position to the hit point
            //if not, then set the hookshot position to the maximum distance of the hookshot
            if (raycastHit.collider.gameObject.layer == 10)
            {
                // Debug.Log("hit grappleable object");
                hookshotPosition = raycastHit.point;
                //state = State.HookshotFlyingPlayer;

            }


            //this is the pullable object layer
            if (raycastHit.collider.gameObject.layer == 11)
            {
                //Debug.Log("hit pullable object");
                hookshotPosition = raycastHit.point;
                //state = State.HookshotAttached;
            }

            //this is the layer for collectibles that you can pull towards yourself
            if (raycastHit.collider.gameObject.layer == 12)
            {
                // Debug.Log("hit collectible object");
                hookshotPosition = raycastHit.point;
                //state = State.HookshotPull;

            }


        }

        //set the hit target to whatever the raycast has hit
        hitTarget = raycastHit.collider.gameObject;
 
    }

    //visualise the raycast in the scene view using gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(shotPoint.position, shotPoint.position + shotPoint.forward * hookshotMaxRange);
    }
}
