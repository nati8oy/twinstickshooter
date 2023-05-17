using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerControlMapping : MonoBehaviour
{
    private PlayerInput playerInput;
    public InputAction shootAction;
    public InputAction editMode;
    public InputAction secondaryShootAction;
    public InputAction debugKillAll;

    public InputAction cycleRight;
    public InputAction cycleLeft;
    public InputAction rotateLeft;
    public InputAction rotateRight;

    public InputAction special;

    private PlayerControls playerControls;
    public GameObject buildCamera;
    public Camera mainCamera;

    public Transform firePoint;
    public Transform secondaryFirePoint;
    public GameObject bulletPrefab;
    public GameObject secondaryPrefab;

    public float weaponCooldown;
    public bool cooldownComplete;

    public float bulletForce = 40f;
    public float secondaryWeaponForce = 80f;



    private void Awake()
    {
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {

        playerControls.Enable();


        ///player controls
        editMode = new InputAction(binding: "<DualShockGamepad>/leftStickPress");
        //edit mode is handled by functions inside of the builder manager script
        editMode.performed += _ => BuilderManager.Instance.EditMode();
        editMode.Enable();

        shootAction = new InputAction(binding: "<DualShockGamepad>/rightShoulder");
        shootAction.performed += _ => GetComponent<IAttack>().Attack();
        shootAction.Enable();


        secondaryShootAction = new InputAction(binding: "<DualShockGamepad>/leftShoulder");
        secondaryShootAction.performed += _ => SecondaryWeapon();
        secondaryShootAction.Enable();


        /*
        debugKillAll = new InputAction(binding: "<DualShockGamepad>/rightTrigger");
        debugKillAll.performed += _ => Utilities.DebugKillAllEnemies();
        debugKillAll.Enable();
        */


        //edit mode controls
        cycleRight = new InputAction(binding: "<DualShockGamepad>/rightShoulder");
        cycleRight.performed += _ => BuilderManager.Instance.Cycle("right");

        cycleLeft = new InputAction(binding: "<DualShockGamepad>/leftShoulder");
        cycleLeft.performed += _ => BuilderManager.Instance.Cycle("left");

        rotateRight = new InputAction(binding: "<DualShockGamepad>/buttonWest");
        rotateRight.performed += _ => BuilderManager.Instance.RotateObject("right");

        rotateLeft = new InputAction(binding: "<DualShockGamepad>/buttonEast");
        rotateLeft.performed += _ => BuilderManager.Instance.RotateObject("left");


        special = new InputAction(binding: "<Gamepad>/buttonSouth");
        special.performed += _ => UseSpecial();


    }

    private void OnDisable()
    {
        playerControls.Disable();
        shootAction.Disable();
        secondaryShootAction.Disable();
    }

    private void Update()
    {
        if (PlayerManager.Instance.playerDead || (GameManager.Instance.gameState == GameManager.GameState.building))
        {
            //disable the player controls
            playerControls.Disable();
            shootAction.Disable();
            secondaryShootAction.Disable();

            //enable the cycling controls for edit mode
            cycleRight.Enable();
            cycleLeft.Enable();
            rotateLeft.Enable();
            rotateRight.Enable();
        }

        else
        {
            //enable the player controls
            playerControls.Enable();
            shootAction.Enable();
            secondaryShootAction.Enable();
            special.Enable();

            //disable the edit mode controls for cycling left and right
            cycleRight.Disable();
            cycleLeft.Disable();
            rotateLeft.Disable();
            rotateRight.Disable();
        }
    }

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
            }
            AmmoManager.ammoSecondary -= 1;

        }
    }

   public void UseSpecial()
    {
        Debug.Log("special activated");
      
    }
    

   
}
