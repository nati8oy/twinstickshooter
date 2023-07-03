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

    public float acceleration = 200f;
    public float decceleration = 20f;
    public float damping = 50;
    public float targetSpeed = 150f;
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

        speedDif = targetSpeed - characterController.velocity.x;

       if (isButtonHeld)
        {

            movement = Mathf.Pow(Mathf.Abs(speedDif)* accelRate, velPower)* Mathf.Sign(speedDif);
            Vector2 movementInput = movement* controls.Controls.Movement.ReadValue<Vector2>();
            Vector3 inputDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

            velocity += inputDirection * accelRate * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, targetSpeed);



        }
        else
        {
            velocity -= velocity * damping * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
 }


