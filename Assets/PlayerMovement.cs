using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 1.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    public Rigidbody playerRB;


    public Camera cam;
    Vector2 movement;
    Vector2 mousePos;
    public Transform lookPoint;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;





    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        controller = gameObject.AddComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        mousePos  = cam.ScreenToWorldPoint(Input.mousePosition);


        //transform.rotation = Quaternion.FromToRotation(new Vector3(lookPoint.transform.position.x, 0f, lookPoint.transform.position.z), transform.forward);

        //look at option
         transform.LookAt(new Vector3(lookPoint.transform.position.x, transform.position.y, lookPoint.transform.position.z));

        /*
        Vector3 targetDir = lookPoint.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        */



    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    private void FixedUpdate()
    {
        
    }
}
