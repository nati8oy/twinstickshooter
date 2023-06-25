using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class CM_Hookshot : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Inputs")]
    [SerializeField] private InputAction hookshot;
    [SerializeField] private InputAction hookshotPull;
    [SerializeField] private InputAction jump;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Transform shotPoint;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform carryPoint;

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



    [SerializeField] private CharacterController characterController;

    [SerializeField] private float hookshotMaxRange = 10f;
    private TwinStickMovement twinStick;

    //used to check which arm is being used to grapple
    //0 means first arm

    [Header("Arm Controls")]
    private int currentArm = 0;
    [SerializeField] private bool multiArmSwing;
    [SerializeField] private Transform[] arms;


    [Header("Feedbacks")]
    [SerializeField] private UnityEvent onGrapple;
    [SerializeField] private UnityEvent onGrappleDrag;
    [SerializeField] private UnityEvent stopFeedbacks;

    private GameObject hitTarget;

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
        twinStick = gameObject.GetComponent<TwinStickMovement>();


        hookshot = new InputAction(binding: "<Mouse>/leftButton");
        //hookshot.performed += _ => LaunchHookshot();
        hookshot.performed += _ => HandleHookshotStart();
        hookshot.Enable();

        
        hookshotPull = new InputAction(binding: "<Mouse>/rightButton");
        hookshotPull.performed += _ => ThrowCarriedObject();
        hookshotPull.Enable();

        
        jump = new InputAction(binding: "<Keyboard>/space");
        jump.performed += _ => Jump();
        jump.Enable();


        shotPoint = arms[0];



    }

    // Update is called once per frame
    void Update()
    {
        //get player velocity from the twinstick object
        playerVelocity = twinStick.playerVelocity;

        switch (state)
        {
            default:
            case State.Normal:
            lr.enabled = false;

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
                break;
            case State.HookshotCarry:
                HandleHookshotCarry();
                hook.gameObject.SetActive(false);
                break;
        }

        Debug.Log(state);

    }

private void LateUpdate(){
   // lr.SetPosition(1, hookshotPosition);
    //set the line renderer start point to that of the shot point
    lr.SetPosition(0, shotPoint.position);
}

    private void HandleHookshotStart()
    {

        if (multiArmSwing)
        {
            currentArm = (currentArm + 1) % arms.Length;
            shotPoint = arms[currentArm];
        }

        //adds the particles when moving
        //onGrapple.Invoke();

        //if you are normal or flying through the air then shoot the hookshot
        if (state == State.Normal || state == State.HookshotFlyingPlayer)
        {

            if (Physics.Raycast(shotPoint.position, shotPoint.forward, out raycastHit, hookshotMaxRange))
            {
                //hit something
                //hookshotPosition = raycastHit.point;

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

                }

                //this is the pullable object layer
                if (raycastHit.collider.gameObject.layer == 11)
                {
                    //Debug.Log("hit pullable object");
                    hook.position = raycastHit.point;
                    hookshotPosition = raycastHit.point;
                    state = State.HookshotPull;
                }

                //this is the layer for collectibles that you can pull towards yourself
                if (raycastHit.collider.gameObject.layer == 12)
                {
                    // Debug.Log("hit collectible object");
                    hook.position = raycastHit.point;
                    hookshotPosition = raycastHit.point;
                    state = State.HookshotPull;

                }


                //set the hit target to whatever the raycast has hit
                hitTarget = raycastHit.collider.gameObject;
                //make the raycast ignore the hook object's collider
                Physics.IgnoreCollision(raycastHit.collider, hook.GetComponent<Collider>());
                
            }


        }
        
        lr.enabled = true;
        lr.SetPosition(1, hookshotPosition);
    }




    private void HandleHookshotMovement()
    {



        if (multiArmSwing)
        {

        }

        onGrappleDrag.Invoke();


        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        //Vector3 move = new Vector3(hookshotDir.x, 0, hookshotDir.y);

        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        //distance to get within before changing state
        float destinationThreshold = 2f;


        characterController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        if(Vector3.Distance(transform.position, hookshotPosition)< destinationThreshold)
        {
            //reached hookshot position
            state = State.Normal;
            lr.enabled = false;
            //characterController.enabled = false;
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

    private void HandleHookshotPull()
    {
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
        //and if it's less than 3, then cancel the hookshot
        if (Vector3.Distance(transform.position, hitTarget.transform.position) < 3f)
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

    private void HandleHookshotCarry()
    {

        var hitTargetRB = hitTarget.GetComponent<Rigidbody>();

        //carry the hittarget game object with the player at the shot point
        hitTargetRB.useGravity = false;
        hitTargetRB.freezeRotation = true;
        hitTarget.transform.position = carryPoint.transform.position;
        lr.enabled = false;
    }


    private void ThrowCarriedObject()
    {

        var hitTargetRB = hitTarget.GetComponent<Rigidbody>();
        //if you're carrying an object, throw it
        if (state == State.HookshotCarry)
        {
            state = State.Normal;
            //launch the hittarget object in the direction the player is facing
            hitTargetRB.useGravity = true;
            hitTargetRB.freezeRotation = false;
            hitTargetRB.AddForce(transform.forward * 2000f);
            Debug.Log("Object Thrown");   
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
        characterController.enabled = false;
        Invoke("ResetController", 0.5f);
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

    private void ResetController()
    {
        characterController.enabled = true;

    }

}
