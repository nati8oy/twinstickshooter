using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Dash : MonoBehaviour
{
    public float dashForce = 10f; // The force with which the player will dash.
    private bool hasDashed = false; // Flag to indicate if the player has dashed yet.
    public float dashCooldown = 2f; // The amount of time the player must wait before dashing again.

    private Rigidbody rb;

    public Event onDashStart;

    private TwinStickMovement playerControls;


    public InputAction dash;
    private float timeSinceLastDash = Mathf.Infinity; // The amount of time that has passed since the player last dashed.


    private void OnEnable()
    {
        ///player controls
        dash = new InputAction(binding: "<Gamepad>/buttonEast");
        //edit mode is handled by functions inside of the builder manager script
        dash.performed += _ => OnDash();
        dash.Enable();

        playerControls = gameObject.GetComponent<TwinStickMovement>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void OnDash()
    {
        if (!hasDashed)
        {

            if (playerControls)
            {
                playerControls.enabled = false;
            }
            Debug.Log("Dashing!");
            // Calculate the dash direction based on the direction the player is facing.
            Vector3 dashDirection = transform.forward;
            Debug.Log(dashDirection);
            // Apply the dash force in the dash direction.
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

            // Set the hasDashed flag to true to prevent the player from dashing again until they land.
            hasDashed = true;
            timeSinceLastDash = 0f;

        }

    }

    private void Update()
    {
        if (hasDashed)
        {
            timeSinceLastDash += Time.deltaTime;
            if (timeSinceLastDash >= dashCooldown)
            {
                hasDashed = false;
                playerControls.enabled = true;
            }
        }
    }
}
