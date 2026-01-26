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

        //make sure the player is constantly on the ground
        playerVelocity.y += gravityValue * Time.deltaTime;

        //move the player down at the speed of gravity
        characterController.Move(playerVelocity * Time.deltaTime);

        currentVelocity = velocity;
        accelRate = (Mathf.Abs(targetSpeed)>0.01f) ? acceleration : decceleration;
        speedDif = targetSpeed - characterController.velocity.x;

       
        //if a button is being held down call the onmoveperformed function

        if(controls.Controls.Movement.ReadValue<Vector2>() != Vector2.zero)
        {
            movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
            Vector2 movementInput = movement * controls.Controls.Movement.ReadValue<Vector2>();
            Vector3 inputDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

            velocity += inputDirection * accelRate * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, targetSpeed);
        }




        //if a button is not being held down call the onmovecanceled function
        if (controls.Controls.Movement.ReadValue<Vector2>() == Vector2.zero);
        {
            velocity -= velocity * damping * Time.deltaTime;
            //Debug.Log("current velocity: " + velocity);
            //reduce velocity by damping over time
            // velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);
        }

    

        characterController.Move(velocity * Time.deltaTime);
    }
 }


