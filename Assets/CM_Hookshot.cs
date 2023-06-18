using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CM_Hookshot : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private InputAction hookshot;
    [SerializeField] private InputAction hookshotCancel;
    [SerializeField] private InputAction jump;

    [SerializeField] private Transform shotPoint;
    [SerializeField] private Transform debugHitPointTransform;
    private Vector3 hookshotPosition;

    private Vector3 characterVelocityMomentum;

    [SerializeField] float hookshotSpeedMultiplier = 2f;
    private CharacterController controller;


    private Vector2 movement;
    private Vector3 playerVelocity;



    [SerializeField] private CharacterController characterController;
    private TwinStickMovement twinStick;

    private State state;
    private enum State
    {
        Normal, HookshotFlyingPlayer,
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


        hookshotCancel = new InputAction(binding: "<Mouse>/rightButton");
        //edit mode is handled by functions inside of the builder manager script
        hookshotCancel.performed += _ => CancelHookshot();
        hookshotCancel.Enable();

        jump = new InputAction(binding: "<Keyboard>/space");
        //edit mode is handled by functions inside of the builder manager script
        jump.performed += _ => Jump();
        jump.Enable();

        //characterController.HandleMovement();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Normal:
                //HandleCharacterMovement();
                //HandleHookshotStart();
                break;

            case State.HookshotFlyingPlayer:
                HandleHookshotMovement();
                break;
        }

        Debug.Log(state);

    }



    private void HandleHookshotStart()
    {
        RaycastHit hit;
        if (Physics.Raycast(shotPoint.position, shotPoint.forward, out RaycastHit raycastHit))
        {
            //hit something
            debugHitPointTransform.position = raycastHit.point;
            hookshotPosition = raycastHit.point;
            state = State.HookshotFlyingPlayer;

        }

    }


    private void HandleHookshotMovement()
    {

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        Vector3 move = new Vector3(hookshotDir.x, 0, hookshotDir.y);

        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        //distance to get within before changing state
        float destinationThreshold = 2f;

        characterController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);
        if(Vector3.Distance(transform.position, hookshotPosition)< destinationThreshold)
        {
            //reached hookshot position
            state = State.Normal;
        }

        //twinStick.playerVelocity += characterVelocityMomentum;

        float momentumExtraSpeed = 7f; 
        characterVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;

    }

    private void CancelHookshot()
    {
        state = State.Normal;
    }

    private void Jump()
    {
    }

}
