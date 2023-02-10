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



    [SerializeField] private bool isGamepad;

    private CharacterController controller;

    private Vector2 movement;
    private Vector2 aim;

    private Vector3 playerVelocity;

    private PlayerControls playerControls;
    private PlayerInput playerInput;
    public InputAction shootAction;
    public InputAction secondaryShootAction;
    public InputAction debugKillAll;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {


        playerControls.Enable();

        
        // Create an Action that binds to the primary action control on all devices.
        shootAction = new InputAction(binding: "<DualShockGamepad>/rightShoulder");

        // Have it run your code when the Action is triggered.
        shootAction.performed += _ => GetComponent<IAttack>().Attack();

        // Start listening for control changes.
        shootAction.Enable();
        


        // Create an Action that binds to the primary action control on all devices.
        secondaryShootAction = new InputAction(binding: "<DualShockGamepad>/leftShoulder");

        // Have it run your code when the Action is triggered.
        secondaryShootAction.performed += _ => SecondaryWeapon();

        // Start listening for control changes.
        secondaryShootAction.Enable();

        // Create an Action that binds to the primary action control on all devices.
        debugKillAll = new InputAction(binding: "<DualShockGamepad>/rightTrigger");

        // Have it run your code when the Action is triggered.
        debugKillAll.performed += _ =>Utilities.DebugKillAllEnemies();

        // Start listening for control changes.
        debugKillAll.Enable();


    }


    private void OnDisable()
    {
        playerControls.Disable();
        shootAction.Disable();
        secondaryShootAction.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();


        /*
        weaponCooldown -= Time.deltaTime;
        if (weaponCooldown <= 0 && !cooldownComplete)
        {
            secondaryShootAction.Enable();
            cooldownComplete = true;
        }

        */

        if (PlayerManager.Instance.playerDead || GameManager.levelComplete)
        {
            playerControls.Disable();
            shootAction.Disable();
            secondaryShootAction.Disable();
        }

        else 
        {
            playerControls.Enable();
            shootAction.Enable();
            secondaryShootAction.Enable();
        }
       

    }

    public void HandleInput()
    {
        movement = playerControls.Controls.Movement.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();

    }

   
    public void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);


        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


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

    /*
    public void Shoot()
    {
        if (AmmoManager.ammoPrimary > 0)
        {
            GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("bullet");


            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.SetActive(true);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
            }
            AmmoManager.ammoPrimary -= 1;
        }

       

    }
    */

    public void SecondaryWeapon()
    {
        GameObject secondary = ObjectPooler.SharedInstance.GetPooledObject("grenade");


        if (AmmoManager.ammoSecondary > 0)
        {
            if (secondary != null)
            {
                secondary.transform.position = secondaryFirePoint.position;
                secondary.SetActive(true);
                Rigidbody rb = secondary.GetComponent<Rigidbody>();
                rb.AddForce(secondaryFirePoint.forward * secondaryWeaponForce, ForceMode.Impulse);
                //rb.AddRelativeTorque(new Vector3(100,100,100));
                //Debug.Log("secondary shot");
            }
            AmmoManager.ammoSecondary -= 1;

        }
    }

}
