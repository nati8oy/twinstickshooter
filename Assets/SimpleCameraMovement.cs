using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SimpleCameraMovement : MonoBehaviour
{
    private CameraControlActions cameraActions;

    private InputAction movement;
    private Transform cameraTransform;

    //value set in various functions 
    //used to update the position of the camera base object.
    private Vector3 targetPosition;


    private void Awake()
    {
        cameraTransform = GetComponent<CinemachineVirtualCamera>().transform;


        //CMinput = GetComponent<CinemachineInputProvider>();

    }

    private void OnEnable()
    {
        cameraActions = new CameraControlActions();

        movement = cameraActions.Camera.Movement;


        // Create an Action that binds to the primary action control on all devices.
        movement = new InputAction(binding: "<Gamepad>/leftStick");
        movement.performed += _ => GetKeyboardMovement();
        // Start listening for control changes.
        movement.Enable();

    }

    private void OnDisable()
    {
        movement.Disable();

    }

    // Update is called once per frame
    void Update()
    {
        GetKeyboardMovement();
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
                    + movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
            targetPosition += inputValue;
    }



    //gets the horizontal forward vector of the camera
    private Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward;
    }
    //gets the horizontal right vector of the camera
    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y = 0f;
        return right;
    }
}
