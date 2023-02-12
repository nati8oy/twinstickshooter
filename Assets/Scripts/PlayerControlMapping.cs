using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;


public class PlayerControlMapping : MonoBehaviour
{
    private PlayerInput playerInput;
    public InputAction shootAction;
    public InputAction editMode;
    public InputAction secondaryShootAction;
    public InputAction debugKillAll;
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

        // Create an Action that binds to the primary action control on all devices.
        editMode = new InputAction(binding: "<DualShockGamepad>/leftStickPress");
        editMode.performed += _ => EditMode();
        // Start listening for control changes.
        editMode.Enable();


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
        debugKillAll.performed += _ => Utilities.DebugKillAllEnemies();

        // Start listening for control changes.
        debugKillAll.Enable();


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

    public void EditMode()
    {

        //change the game mode to building
        Debug.Log("Entered Build Mode");

        //toggle between edit/building and play mode
       if(GameManager.Instance.gameState == GameManager.GameState.play)
        {
            buildCamera.SetActive(true);
            MMTimeManager.Instance.SetTimeScaleTo(0.2f);
            mainCamera = GameObject.Find("CameraRigBase").GetComponentInChildren<Camera>();

            GameManager.Instance.gameState = GameManager.GameState.building;
        }
        else if(GameManager.Instance.gameState == GameManager.GameState.building)
        {
            buildCamera.SetActive(false);
            MMTimeManager.Instance.SetTimeScaleTo(1f);
            GameManager.Instance.gameState = GameManager.GameState.play;
            mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        }



    }
}
