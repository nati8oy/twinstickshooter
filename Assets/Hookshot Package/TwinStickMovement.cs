using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class TwinStickMovement : MonoBehaviour
{

    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float gravityValue = -20.98f;
    [SerializeField] private float controllerDeadZone = 0.1f;
    [SerializeField] private float gamepadRotateSmoothing = 1000f;


    public float maxSpeed = 10;//This is the maximum speed that the object will achieve
    public float acceleration = 10;//How fast will object reach a maximum speed
    public float aeceleration = 10;//How fast will object reach a speed of 0


    public Transform firePoint;
    public Transform secondaryFirePoint;
    public GameObject bulletPrefab;
    public GameObject secondaryPrefab;

    public float weaponCooldown;
    public bool cooldownComplete;


    public float bulletForce = 40f;
    public float secondaryWeaponForce = 80f;

    Vector3 CVM;

    [SerializeField] private bool isGamepad;

    private CharacterController controller;

    private Vector2 movement { get; set; }
    private Vector2 aim;

    public Vector3 playerVelocity;

    private PlayerControls playerControls;
    private PlayerInput playerInput;
    private AutoTarget autoTarget;
    private FrictionController frictionController;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
        autoTarget = GetComponent<AutoTarget>();
        frictionController = GetComponent<FrictionController>();
    }


    private void OnEnable()
    {
        transform.position = gameObject.transform.position;
    }

    private void OnDisable()
    {
        playerControls.Disable();
 
    }
    // Update is called once per frame
    void Update()
    {



        if (GetComponent<PlayerManager>())
        {
            if (PlayerManager.Instance.playerDead || (GameManager.Instance.gameState == GameManager.GameState.building))
            {
                playerControls.Disable();
            }
        }


        else 
        {
            playerControls.Enable();
            HandleInput();
            HandleMovement();
            HandleRotation();
        }
       

    }

    public void HandleInput()
    {
        movement = playerControls.Controls.Movement.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();

        // Auto-detect which input device is being used for aiming
        // If gamepad right stick has significant input, use gamepad mode
        // Otherwise, if mouse position is being updated, use mouse mode
        Vector2 gamepadAim = Gamepad.current != null ? Gamepad.current.rightStick.ReadValue() : Vector2.zero;
        if (Mathf.Abs(gamepadAim.x) > controllerDeadZone || Mathf.Abs(gamepadAim.y) > controllerDeadZone)
        {
            isGamepad = true;
        }
        else if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
        {
            isGamepad = false;
        }

    }

   
    public void HandleMovement()
    {
        /*
        if (gameObject.GetComponent<CM_Hookshot>())
        {
            Vector3 CVM = gameObject.GetComponent<CM_Hookshot>().characterVelocityMomentum;
            playerVelocity += CVM;
            Debug.Log("CVM amount: " + CVM);
        }

        */


        // Ground check — reset vertical velocity when grounded to prevent accumulation
        if (controller.isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity.y = -2f;
        }

        CM_Hookshot hookshot = GetComponent<CM_Hookshot>();

        Vector3 move = new Vector3(movement.x, 0, movement.y);

        float speedMod = 1f;
        if (hookshot != null)
        {
            speedMod = hookshot.dragSpeedMultiplier;
        }

        // Disable gravity while grappling so player travels in a straight line to the grapple point
        if (hookshot != null && hookshot.isGrappling)
        {
            playerVelocity.y = 0f;
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        // Include friction-based velocity if FrictionController is present
        Vector3 frictionMove = Vector3.zero;
        if (frictionController != null)
        {
            frictionMove = frictionController.FrictionVelocity;
        }

        // Combine horizontal movement, friction velocity, and vertical gravity into a single Move call
        Vector3 finalMove = (move * playerSpeed * speedMod) + frictionMove + new Vector3(0, playerVelocity.y, 0);
        controller.Move(finalMove * Time.deltaTime);

        
        

    }

    public void HandleRotation()
    {
        // Auto-target mode: face tracked gummy, or face movement direction if no target/carrying
        if (DebugManager.Instance != null && DebugManager.Instance.autoTargetEnabled)
        {
            CM_Hookshot hookshot = GetComponent<CM_Hookshot>();
            bool isCarrying = hookshot != null && hookshot.state == CM_Hookshot.State.HookshotCarry;
            bool isHooked = hookshot != null && (hookshot.state == CM_Hookshot.State.HookshotAttached
                || hookshot.state == CM_Hookshot.State.HookshotPull
                || hookshot.state == CM_Hookshot.State.HookshotFlyingPlayer);

            // When hooked onto a gummy, face the hooked target instead of auto-target
            if (isHooked && hookshot.hitTarget != null)
            {
                Vector3 hookedDir = hookshot.hitTarget.transform.position - transform.position;
                hookedDir.y = 0f;

                if (hookedDir.sqrMagnitude > 0.01f)
                {
                    Quaternion hookedRotation = Quaternion.LookRotation(hookedDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, hookedRotation, gamepadRotateSmoothing * Time.deltaTime);
                }
                return;
            }

            // When carrying, face movement direction
            // When no target, face movement direction
            // Otherwise, track closest gummy
            if (!isCarrying && autoTarget != null && autoTarget.HasTarget)
            {
                Vector3 targetDir = autoTarget.closestTarget.transform.position - transform.position;
                targetDir.y = 0f;

                if (targetDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, gamepadRotateSmoothing * Time.deltaTime);
                }
            }
            else if (movement.sqrMagnitude > controllerDeadZone * controllerDeadZone)
            {
                // No target — face movement direction
                Vector3 moveDir = new Vector3(movement.x, 0f, movement.y);

                if (moveDir.sqrMagnitude > 0.01f)
                {
                    Quaternion moveRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, moveRotation, gamepadRotateSmoothing * Time.deltaTime);
                }
            }
            return;
        }

        // Standard mode: gamepad right stick or mouse
        if (isGamepad)
        {
            if (Mathf.Abs(aim.x) > controllerDeadZone || Mathf.Abs(aim.y) > controllerDeadZone)
            {
                Vector3 playerDirection = Vector3.right * aim.x + Vector3.forward * aim.y;

                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    Quaternion newrotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newrotation, gamepadRotateSmoothing * Time.deltaTime);
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(aim);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if(groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    public void OnDeviceChange( PlayerInput pi)
    {
        isGamepad = pi.currentControlScheme.Equals("Gamepad") ?true : false;
    }

    

}
