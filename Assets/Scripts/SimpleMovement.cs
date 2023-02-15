using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 movement;
    [SerializeField] private float movementSpeed = 5f;
    private Vector3 playerVelocity;
    private PlayerControls playerControls;
    private PlayerInput playerInput;


    private void Awake()
    {
        gameObject.AddComponent<CharacterController>();
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }


    private void OnDisable()
    {
        playerControls.Disable();

    }

    void Update()
    {
        HandleMovement();
    }

    public void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * movementSpeed);


       // playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


    }
}
