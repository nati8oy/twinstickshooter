using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEditor.PackageManager;

public class CM_Hookshot : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Inputs")]
    [SerializeField] private InputAction hookshot;
    [SerializeField] private InputAction hookshotPull;
    [SerializeField] private InputAction jump;
    [SerializeField] private InputAction pull;

    [SerializeField] private LayerMask layerMask;

    public Transform shotPoint;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform carryPoint;

    [SerializeField] private float throwForce = 2000f;

    private Vector3 hookshotPosition;

    [SerializeField] private LineRenderer lr;
    public Vector3 characterVelocityMomentum;

    [SerializeField] float hookshotSpeedMultiplier = 2f;
    private CharacterController controller;

    private bool hitSomething =false;

    private float t = 0f;
    private RaycastHit raycastHit;

    private Vector2 movement;
    private Vector3 playerVelocity;
    [SerializeField] private PlayerControls playerControls;

    [SerializeField] private SpringJoint joint;

    [SerializeField] private CharacterController characterController;

    [SerializeField] private float hookshotMaxRange = 10f;

    public bool isGrappling = false;

    [Header("Feedbacks")]
    [SerializeField] private UnityEvent onGrapple;
    [SerializeField] private UnityEvent OnHookshotHit;
    [SerializeField] private UnityEvent onGrappleDrag;
    [SerializeField] private UnityEvent onThrow;
    [SerializeField] private UnityEvent stopFeedbacks;


    [Header("Targeting")]
    public GameObject hitTarget;
    [SerializeField] GameObject[] targets;
    [SerializeField] bool targetVisible;
    [SerializeField] Transform crosshair;
    private RaycastHit targetingRaycast;
    [SerializeField] private AutoTarget autoTargetScript;


    [Header("Hookshot Speed")]

    [SerializeField] private float hookshotSpeedMin = 10f;
    [SerializeField] private float hookshotSpeedMax = 40f;

    private State state;
    private enum State
    {
        Normal, HookshotLaunched, HookshotFlyingPlayer, HookshotPull, HookshotAttached, HookshotCarry,
    }


    private void Awake()
    {
        state = State.Normal;
    }


    // Start is called before the first frame update
    void Start()
    {
       

        characterController = gameObject.GetComponent<CharacterController>();
        ///twinStick = gameObject.GetComponent<TwinStickMovement>();

        /*
        hookshot = new InputAction(binding: "<Mouse>/leftButton");
        //hookshot.performed += _ => LaunchHookshot();
        hookshot.performed += _ => HandleHookshotStart();
        hookshot.Enable();
        */
        
        hookshotPull = new InputAction(binding: "<Mouse>/rightButton");
        hookshotPull.performed += _ => HandleHookshotStart();
        hookshotPull.performed += _ => ThrowCarriedObject();
        //hookshotPull.performed += _ => HandleHookshotPull();
        hookshotPull.Enable();

        
        jump = new InputAction(binding: "<Keyboard>/space");
        jump.performed += _ => Jump();
        jump.Enable();

        pull = new InputAction(binding: "<Keyboard>/leftShift");
        pull.performed += _ => ActivateHookshotPull();
        pull.Enable();

        //set the hookshots to the first one in the array for arms and hooks
        //shotPoint = arms[0];
        //hook = hooks[0];



    }

    // Update is called once per frame
    void Update()
    {
        //visualises a target/crosshair for the hookshot
        if (targetVisible)
        {
            //set the crosshair object to a distance of hookshotMaxRange in front of the shotpoint
            crosshair.position = shotPoint.position + shotPoint.forward * hookshotMaxRange;

            
            //raycast from the shotpoint to the crosshair
            if (Physics.Raycast(shotPoint.position, shotPoint.forward, out targetingRaycast, hookshotMaxRange, layerMask))
            {
                //if the raycast hits something then set the crosshair position to the hit point
                crosshair.position = targetingRaycast.point;
                //hitTarget = targetingRaycast.collider.gameObject;
            }
            
        }



        switch (state)
        {
            default:
            case State.Normal:
            lr.enabled = false;
            isGrappling = false;
                break;
            case State.HookshotLaunched:
                LaunchHookshot();
                break;
            case State.HookshotFlyingPlayer:
                HandleHookshotMovement();
                hook.gameObject.SetActive(true);
                break;
            case State.HookshotPull:
                HandleHookshotPull();
                hook.gameObject.SetActive(true);
                break;
            case State.HookshotAttached:
                HandleHookshotPull();
//                HandleHookshotAttached();
                hook.gameObject.SetActive(true);
                break;
            case State.HookshotCarry:
                HandleHookshotCarry();
                hook.gameObject.SetActive(false);
                isGrappling = false;
                break;
        }


        //target the closest object on layer 11
        //get all of the nearby enemies and put them in an array
        targets = GameObject.FindGameObjectsWithTag("enemy");

    }

private void LateUpdate(){
   // lr.SetPosition(1, hookshotPosition);
    //set the line renderer start point to that of the shot point
    lr.SetPosition(0, shotPoint.position);
}

    private void HandleHookshotStart()
    {
        /*
        if (multiArmSwing)
        {
            currentArm = (currentArm + 1) % arms.Length;
            shotPoint = arms[currentArm];

            currentHook = (currentHook + 1) % arms.Length;
            hookshotPosition = hooks[currentHook].position;
        }
        */


        //adds the particles when moving
        //onGrapple.Invoke();

        //if you are normal or flying through the air then shoot the hookshot
        if (state == State.Normal || state == State.HookshotFlyingPlayer)
        {

            
            {

                if (Physics.Raycast(shotPoint.position, shotPoint.forward, out raycastHit, hookshotMaxRange))
                {

                    //check the layer of the object that was hit
                    //if the layer is grappleable, then set the hookshot position to the hit point
                    //if not, then set the hookshot position to the maximum distance of the hookshot
                    if (raycastHit.collider.gameObject.layer == 10)
                    {
                        // Debug.Log("hit grappleable object");
                        hook.position = raycastHit.point;
                        hookshotPosition = raycastHit.point;
                        state = State.HookshotFlyingPlayer;
                        onGrapple.Invoke();
                        OnHookshotHit.Invoke();

                    }


                    //this is the pullable object layer
                    if (raycastHit.collider.gameObject.layer == 11)
                    {
                        //Debug.Log("hit pullable object");
                        hook.position = raycastHit.point;
                        hookshotPosition = raycastHit.point;
                        state = State.HookshotAttached;
                    }

                    //this is the layer for collectibles that you can pull towards yourself
                    if (raycastHit.collider.gameObject.layer == 12)
                    {
                        // Debug.Log("hit collectible object");
                        hook.position = raycastHit.point;
                        hookshotPosition = raycastHit.point;
                        state = State.HookshotPull;

                    }


                }

                //set the hit target to whatever the raycast has hit

                hitTarget = raycastHit.collider.gameObject;
                //Debug.Log("hit target is " + hitTarget.name);
                //make the raycast ignore the hook object's collider
                Physics.IgnoreCollision(raycastHit.collider, hook.GetComponent<Collider>());


            }





        }
        
        lr.enabled = true;
        lr.SetPosition(1, hookshotPosition);
    }




    private void HandleHookshotMovement()
    {

        isGrappling = true;

        
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        // Calculate the additional movement based on the current velocity of the Character Controller
        Vector3 additionalMovement = characterController.velocity * Time.deltaTime;

        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        // Calculate the final movement vector by combining the hookshot direction, additional movement, and hookshot speed
        Vector3 move = hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime + additionalMovement;

        characterController.Move(move);

        if (Vector3.Distance(transform.position, hookshotPosition) < 2f)
        {
            // Reached hookshot position
            state = State.Normal;
            lr.enabled = false;
            // characterController.enabled = false;
        }






        //if the player has jumped while hookshotting, cancel the hookshot
        if (jump.ReadValue<float>() > 0)
        {
            
            //also maintain momentum

            float momentumExtraSpeed = 3f;


            //characterVelocityMomentum = hookshotDir * hookshotSpeed;

            CancelHookshot();


        }

        //dampen speed of momentum
        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 20f;

            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }           

        }

    }


    private void HandleHookshotAttached()
    {
        lr.enabled = true;
        lr.SetPosition(1, hookshotPosition);
        hook.gameObject.transform.position = hookshotPosition;
        hookshotPosition = hitTarget.transform.position;


        if (hitTarget == null)
        {
            CancelHookshot();
        }

        //var hitTargetRB = hitTarget.GetComponent<Rigidbody>();
        //add force to the object you've hit in an arc

        //
       // hitTargetRB.AddForce(transform.forward * 10f + transform.right * 5f);


        //deactivate the navmesh agent of the object you've hit
        if (hitTarget.GetComponent<NavMeshAgent>())
        {
            hitTarget.GetComponent<NavMeshAgent>().enabled = false;

        }


        joint.connectedBody = hitTarget.GetComponent<Rigidbody>();
      

        //if the player has jumped while attached, cancel the hookshot
        if (jump.ReadValue<float>() > 0)
        {

            //also maintain momentum

            float momentumExtraSpeed = 3f;

            //characterVelocityMomentum = hookshotDir * hookshotSpeed;

            CancelHookshot();

        }

    }

    private void HandleHookshotPull()
    {

        if (hitTarget != null)
        {

            //deactivate the navmesh agent
            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = false;
            }


            //destroy the joint for an attached object
            if (hitTarget.GetComponent<FixedJoint>())
            {
                Destroy(hitTarget.GetComponent<FixedJoint>());
            }



            //hook.gameObject.SetActive(true);

            float hookshotSpeedMin = 10f;
            float hookshotSpeedMax = 40f;

            Vector3 hookshotDir = (hookshotPosition + transform.position).normalized;

            //pull the hittarget towards the player
            hitTarget.transform.position = Vector3.MoveTowards(hitTarget.transform.position, transform.position, 40f * Time.deltaTime);
            hook.gameObject.transform.position = hitTarget.transform.position;

            //match the hook to the same position of the hit target as it's getting pulled back
            //hook.gameObject.transform.position = hitTarget.transform.position;


            lr.enabled = true;
            lr.SetPosition(1, hitTarget.transform.position);

            //check the distance between the hookshot position and the player
            //and if it's less than 1f, then cancel the hookshot
            if (Vector3.Distance(transform.position, hitTarget.transform.position) < 1f)
            {
                if (raycastHit.collider.gameObject.layer == 11)
                {
                    state = State.HookshotCarry;
                }

                else
                {
                    CancelHookshot();
                }

            }

            //if the player has jumped while hookshotting, cancel the hookshot
            if (jump.ReadValue<float>() > 0)
            {

                CancelHookshot();

            }
        }

    }

    private void HandleHookshotCarry()
    {
        if (hitTarget != null)
        {
            DetachSpringJoint();

            var hitTargetRB = hitTarget.GetComponent<Rigidbody>();

            //carry the hittarget game object with the player at the shot point
            hitTargetRB.useGravity = false;
            hitTargetRB.freezeRotation = true;
            hitTarget.transform.position = carryPoint.transform.position;

            lr.enabled = false;

        }
    }


        private void ThrowCarriedObject()
    {
        //play throw feedbacks
        onThrow.Invoke();

        var hitTargetRB = hitTarget.GetComponent<Rigidbody>();
        //if you're carrying an object, throw it
        if (state == State.HookshotCarry)
        {
            state = State.Normal;

            //check if it's got a rigid body or not
            if (hitTarget.GetComponent<Rigidbody>())
            {
                //launch the hittarget object in the direction the player is facing
                hitTargetRB.useGravity = true;
                hitTargetRB.freezeRotation = false;

                //check if the auto targeting is on and if the object you're throwing isn't an enemy
                if (autoTargetScript!=null && hitTarget.tag!="enemy")
                {
                    hitTargetRB.AddForce(autoTargetScript.attackDirection * throwForce);

                }
                else
                {
                    hitTargetRB.AddForce(transform.forward * throwForce);
                }

                //add a bit spin to the object when you throw it
                hitTargetRB.AddTorque(transform.forward * throwForce);
            }
           

            //check if the object has a navmesh agent and if so reset it so it works again after being thrown
            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = false;
                Invoke("ResetEnemyMovement", 1f);
               
            }
        }
    }

    private void LaunchHookshot()
    {
        state = State.HookshotLaunched;


        if (!hitSomething)
        {
            // shoot the hook at the object
            //hook.position = shotPoint.position;
            hook.position = Vector3.MoveTowards(shotPoint.position, shotPoint.position + transform.forward * 10f, 4f * Time.deltaTime);
            //when the hook reaches 10f in front of the player, set the hook.position back to the shotpoint.position
            if (Vector3.Distance(hook.position, shotPoint.position) < 1f)
            {
                hitSomething = true;
                
                
            }
        }

        else if (hitSomething)
        {
            hook.position = Vector3.MoveTowards(shotPoint.position + transform.forward * 10f, hook.position, 40f * Time.deltaTime);
            hitSomething = false;
            state = State.Normal;
        }

  

        //lr.enabled = true;
        //lr.SetPosition(1, hookshotPosition);

        //move the hook position from the shot point to a distacne of 10f in front of the player
        //hook.position = shotPoint.position + transform.forward * 10f;

/*
        if (Vector3.Distance(hook.position, Vector3.zero) < 3f)
        {
            state = State.Normal;
            lr.enabled = false;
        }*/
    }

    private void CancelHookshot()
    {
        stopFeedbacks.Invoke();

        //reset the enemy movement
        ResetEnemyMovement();
        DetachSpringJoint();

        if (hitTarget.GetComponent<NavMeshAgent>())
        {
            hitTarget.GetComponent<NavMeshAgent>().enabled = true;

        }

        //characterController.enabled = false;
        //Invoke("ResetController", 0.5f);

        //get the current velocity of the player
        Vector3 currentVelocity = characterController.velocity;

        //Debug.Log("current velocity " + currentVelocity + " CVM " + characterVelocityMomentum);

        //add the momentum to the current velocity
        //Vector3 momentumVelocityAddition = characterVelocityMomentum;
        /*
        currentVelocity += momentumVelocityAddition;
        */
        //momentumVelocityAddition.y = 0;


        state = State.Normal;
        lr.enabled = false;
       
    }

    private bool Jump()
    {
        return true;
    }

    private void ActivateHookshotPull()
    {
        if(state == State.HookshotAttached)
        {
            state = State.HookshotPull;

        }
    }

    private void DetachSpringJoint()
    {
        //if the spring joint is connect to something, disconnect it.
        if (joint.connectedBody != null)
        {
            joint.connectedBody = null;
        }
    }

    private void ResetController()
    {
        characterController.enabled = true;

    }

    private void ResetEnemyMovement()
    {
        if (hitTarget != null)
        {
            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = true;

            }
        }
       

    }

    //visualise the raycast in the scene view using gizmos
     void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(shotPoint.position, shotPoint.position + shotPoint.forward * hookshotMaxRange);
    }

}
