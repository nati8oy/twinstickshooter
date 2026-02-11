using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrictionController : MonoBehaviour
{

    private PlayerControls playerControls;
    private PlayerInput playerInput;

    private CharacterController characterController;
    private Vector3 velocity;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;


    [Header("Acceleration")]
    public float acceleration = 200f;
    public float decceleration = 20f;
    public float damping = 50;
    public float targetSpeed = 150f;

    [Header("Movement")]
    [SerializeField] private float speedDif;
    [SerializeField] private float accelRate;
    [SerializeField] private Vector3 currentVelocity;
    [SerializeField] private float velPower;

    /// <summary>
    /// The friction-based velocity for TwinStickMovement to read and combine into a single Move() call.
    /// </summary>
    public Vector3 FrictionVelocity => velocity;

    private bool isButtonHeld;
    private float movement;

    private PlayerControls controls;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Controls.Movement.performed += OnMovePerformed;
        controls.Controls.Movement.performed += OnMoveCanceled;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        isButtonHeld = true;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isButtonHeld = false;
    }

    private void Update()
    {
        currentVelocity = velocity;
        accelRate = (Mathf.Abs(targetSpeed)>0.01f) ? acceleration : decceleration;


        //if a button is being held down, accelerate in the input direction

        if(controls.Controls.Movement.ReadValue<Vector2>() != Vector2.zero)
        {
            Vector2 rawInput = controls.Controls.Movement.ReadValue<Vector2>();
            Vector3 inputDirection = new Vector3(rawInput.x, 0f, rawInput.y).normalized;

            velocity += inputDirection * accelRate * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, targetSpeed);
        }




        //if a button is not being held down call the onmovecanceled function
        if (controls.Controls.Movement.ReadValue<Vector2>() == Vector2.zero)
        {
            velocity -= velocity * damping * Time.deltaTime;
            //Debug.Log("current velocity: " + velocity);
            //reduce velocity by damping over time
            // velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);
        }

    

        // Velocity is now read by TwinStickMovement via FrictionVelocity property
        // instead of calling Move() directly, to avoid dual Move() conflicts with jump/gravity.
    }
 }


