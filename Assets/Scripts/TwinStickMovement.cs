using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class TwinStickMovement : MonoBehaviour
{

    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float controllerDeadZone = 0.1f;
    [SerializeField] private float gamepadRotateSmoothing = 1000f;


    public float maxSpeed = 10;//This is the maximum speed that the object will achieve
    public float acceleration = 10;//How fast will object reach a maximum speed
    public float aeceleration = 10;//How fast will object reach a speed of 0


    public Transform firePoint;
    public Transform secondaryFirePoint;
    public GameObject bulletPrefab;
    public GameObject secondaryPrefab;

    public float weaponCooldown;
    public bool cooldownComplete;


    public float bulletForce = 40f;
    public float secondaryWeaponForce = 80f;

    Vector3 CVM;

    [SerializeField] private bool isGamepad;

    private CharacterController controller;

    private Vector2 movement { get; set; }
    private Vector2 aim;

    public Vector3 playerVelocity;

    private PlayerControls playerControls;
    private PlayerInput playerInput;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }


    private void OnEnable()
    {
        transform.position = gameObject.transform.position;
    }

    private void OnDisable()
    {
        playerControls.Disable();
 
    }
    // Update is called once per frame
    void Update()
    {



        if (GetComponent<PlayerManager>())
        {
            if (PlayerManager.Instance.playerDead || (GameManager.Instance.gameState == GameManager.GameState.building))
            {
                playerControls.Disable();
            }
        }


        else 
        {
            playerControls.Enable();
            HandleInput();
            HandleMovement();
            HandleRotation();
        }
       

    }

    public void HandleInput()
    {
        //movement = playerControls.Controls.Movement.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();

    }

   
    public void HandleMovement()
    {
        /*
        if (gameObject.GetComponent<CM_Hookshot>())
        {
            Vector3 CVM = gameObject.GetComponent<CM_Hookshot>().characterVelocityMomentum;
            playerVelocity += CVM;
            Debug.Log("CVM amount: " + CVM);
        }

        */


        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        /*
        if (CVM.magnitude >= 0f)
        {
            float momentumDrag = 3f;

            CVM -= CVM * momentumDrag * Time.deltaTime;
            if (CVM.magnitude < .0f)
            {
                CVM = Vector3.zero;
            }
        }

        */

        playerVelocity.y += gravityValue * Time.deltaTime;

        
        

    }

    public void HandleRotation()
    {
        if (isGamepad)
        {
            //rotate player
            if (Mathf.Abs(aim.x) > controllerDeadZone || Mathf.Abs(aim.y) > controllerDeadZone)
            {
                Vector3 playerDirection = Vector3.right * aim.x + Vector3.forward * aim.y;

                //shoots whenever the player is rotating 
                //Shoot();

                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    Quaternion newrotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newrotation, gamepadRotateSmoothing * Time.deltaTime);
                }
            }
        }

        else
        {
            Ray ray = Camera.main.ScreenPointToRay(aim);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if(groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    public void OnDeviceChange( PlayerInput pi)
    {
        isGamepad = pi.currentControlScheme.Equals("Gamepad") ?true : false;
    }

    

}
