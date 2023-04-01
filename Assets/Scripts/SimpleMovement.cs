using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMovement : MonoBehaviour
{

    private float moveSpeed = 15f;
    private PlayerControls playerControls;
    private Vector2 movement;


    public void MoveGamepad()
    {
        
        Vector2 input = Gamepad.current.leftStick.ReadValue();
        if (input.magnitude > 0.1f)
        {
            //just use the x and y inputs of the left stick controller do move the object
            transform.Translate(new Vector3 (input.x, 0, input.y) * Time.deltaTime * moveSpeed);
        }
    }
  

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            MoveGamepad();
        }
    }

     private void OnDisable()
    {
        playerControls.Disable();
 
    }
}
