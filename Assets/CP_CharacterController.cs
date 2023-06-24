using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_CharacterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
/*

    //a function that makes the player face the direction of the mouse
    //this is used for the player character
    public void HandleRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        }


    //a function to handle player movement only using x and z axis
    //that uses the character controller component
    //this is used for the player character
    //it accelerates and decelerates the player
    //it also handles jumping

    public void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        characterController.Move(move * Time.deltaTime * speed);
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
        if (characterController.isGrounded)
        {
            playerVelocity.y = 0f;
            if (jumping)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                jumping = false;
            }
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
*/