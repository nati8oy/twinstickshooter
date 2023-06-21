using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class CM_Hookshot : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private InputAction hookshot;
    [SerializeField] private InputAction hookshotPull;
    [SerializeField] private InputAction jump;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Transform shotPoint;
    [SerializeField] private Transform hook;
    private Vector3 hookshotPosition;

    [SerializeField] private LineRenderer lr;
    public Vector3 characterVelocityMomentum;

    [SerializeField] float hookshotSpeedMultiplier = 2f;
    private CharacterController controller;


   private float t = 0f;


    private Vector2 movement;
    private Vector3 playerVelocity;
    [SerializeField] private PlayerControls playerControls;


    [SerializeField] private CharacterController characterController;
    private TwinStickMovement twinStick;

    private GameObject hitTarget;

    private State state;
    private enum State
    {
        Normal, HookshotLaunched, HookshotFlyingPlayer, HookshotPull,
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
        //edit mode is handled by functions inside of the builder manager script
        hookshot.performed += _ => HandleHookshotStart();
        hookshot.Enable();


        /*
        hookshotPull = new InputAction(binding: "<Mouse>/rightButton");
        //edit mode is handled by functions inside of the builder manager script
        hookshotPull.performed += _ => HandleHookshotPull();
        hookshotPull.Enable();

        */

        jump = new InputAction(binding: "<Keyboard>/space");
        //edit mode is handled by functions inside of the builder manager script
        jump.performed += _ => Jump();
        jump.Enable();

        //characterController.HandleMovement();


        
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
                //HookshotStarted();
                break;

            case State.HookshotFlyingPlayer:
                HandleHookshotMovement();
                break;
            case State.HookshotPull:
                HandleHookshotPull();

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
        RaycastHit hit;
        if (Physics.Raycast(shotPoint.position, shotPoint.forward, out RaycastHit raycastHit))
        {
            //hit something
            //hookshotPosition = raycastHit.point;

            //check the layer of the object that was hit
            //if the layer is grappleable, then set the hookshot position to the hit point
            //if not, then set the hookshot position to the maximum distance of the hookshot
            if (raycastHit.collider.gameObject.layer == 10){
                Debug.Log("hit grappleable object");
                hook.position = raycastHit.point;
                hookshotPosition = raycastHit.point;
                state = State.HookshotFlyingPlayer;

            }

             if (raycastHit.collider.gameObject.layer == 11){
                Debug.Log("hit pullable object");
                hook.position = raycastHit.point;
                hookshotPosition = raycastHit.point;
                state = State.HookshotPull;

            }

             //set the hit target to whatever the raycast has hit
            hitTarget =  raycastHit.collider.gameObject;

        }
        
        lr.enabled = true;
        lr.SetPosition(1, hookshotPosition);
    }




    private void HandleHookshotMovement()
    {

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
            Debug.Log("jumped while hookshotting");
            //also maintain momentum

            float momentumExtraSpeed = 3f;

            //characterVelocityMomentum = hookshotDir * hookshotSpeed;

            CancelHookshot();

        }

        //dampen speed of momentum
        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 6f;

            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }           

        }

    }

    private void HandleHookshotPull()
    {
        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;

        Vector3 hookshotDir = (hookshotPosition + transform.position).normalized;
        //pull the hittarget towards the player
        hitTarget.transform.position = Vector3.MoveTowards(hitTarget.transform.position, transform.position, 40f * Time.deltaTime);
        hook.gameObject.transform.position = Vector3.MoveTowards(hitTarget.transform.position, transform.position, 40f * Time.deltaTime);


        hook.gameObject.transform.position = hitTarget.transform.position;


        lr.enabled = true;
        lr.SetPosition(1, hitTarget.transform.position);

        //check the distance between the hookshot position and the player
        //and if it's less than 3, then cancel the hookshot
        if (Vector3.Distance(transform.position, hitTarget.transform.position) < 3f)
        {
            CancelHookshot();
        }

    

    }

    private void CancelHookshot()
    {
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
