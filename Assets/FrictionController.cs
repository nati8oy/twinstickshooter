using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrictionController : MonoBehaviour
{

    private PlayerControls playerControls;
    private PlayerInput playerInput;

    [SerializeField] Vector2 moveInput;
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody rb;
    [SerializeField] float acceleration;
    [SerializeField] float decceleration;
    [SerializeField] float velPower;

   [SerializeField] private float movement;

   // [SerializeField] CharacterController characterController;


    [SerializeField] float targetSpeed;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
        playerControls.Enable();

    }

    private void FixedUpdate()
    {
        HandleInput();
        HandleMovement();

    }

    private void HandleInput()
    {

        moveInput = playerControls.Controls.Movement.ReadValue<Vector2>();
        Vector3 newMoveInput = new Vector3(moveInput.x, 0f, moveInput.y);

        targetSpeed = newMoveInput.z * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.z;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector3.forward);

        targetSpeed = newMoveInput.x * moveSpeed;
        speedDif = targetSpeed - rb.velocity.x;

        movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector3.right);



    }

    private void HandleMovement()
    {
        //Vector3 move = new Vector3(movement.x, 0, movement.y);
        //characterController.Move(move * Time.deltaTime * moveSpeed);

        
    }
}
