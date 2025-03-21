using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MousePosition3D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    public Vector2 movementVector;

    public Structure structure;
    private float structureVertSize;

    private PlayerControls playerControls;

    public Vector3 raycastHitPoint;

    public InputAction placeBuildingAction;


    private Collider objectCollider;
    private Vector3 colliderCentrePoint;
    private Vector3 m_Size, m_Min, m_Max;

    private void Awake()
    {
        // playerControls = new PlayerControls();

        //if the game state == building then
        //set the main camera to the one in the camera rig object

        if (GameManager.Instance.gameState == GameManager.GameState.building)
        {
            mainCamera = GameObject.Find("CameraRigBase").GetComponentInChildren<Camera>();

        }
        else
        {
            //set the main camera to the one in the default scene main camera
            mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        }
    }

    private void OnEnable()
    {

        //set the RMB to place the structure
        placeBuildingAction = new InputAction(binding: "<Mouse>/rightButton");

        //mainCamera = GameObject.Find("CameraRigBase").GetComponentInChildren<Camera>();



        // when the input action is called then run the PlaceObject function.
        placeBuildingAction.performed += _ => PlaceObject();


        // Start listening for control changes.
        placeBuildingAction.Enable();


        //Fetch the Collider from the GameObject
        objectCollider = GetComponent<Collider>();
        //Fetch the center of the Collider volume
        colliderCentrePoint = objectCollider.bounds.center;
        //Fetch the size of the Collider volume
        m_Size = objectCollider.bounds.size;
        //Fetch the minimum and maximum bounds of the Collider volume
        m_Min = objectCollider.bounds.min;
        m_Max = objectCollider.bounds.max;

        //this gets the vertical size of the object and divides it by 2 so that it is level with the ground layer
        structureVertSize = m_Size.y / 2;



    }

   

    private void OnDisable()
    {
        // Start listening for control changes.
        placeBuildingAction.Disable();
    }
  

    private void Update()
    {

        if (mainCamera != null)
        {

        //main code to track mouse position on the screen using raycasting
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
        {
            transform.position = new Vector3 (raycastHit.point.x, structureVertSize, raycastHit.point.z);
            raycastHitPoint = raycastHit.point;
        }

        Mouse.current.leftButton.ReadValue();

        }

        else
        {
            Debug.LogError("Camera for Raycast reference is missing");
        }


    }



    public void PlaceObject()
    {

        //instantiates and adds models based on what is in their structure data object
        Instantiate(structure.structureModel, new Vector3(raycastHitPoint.x, structureVertSize, raycastHitPoint.z), Quaternion.identity);

        
        Destroy(GameObject.FindGameObjectWithTag("ghost"));

        ///*OBJECT POOLING
        ///can use this later if needed for performance reasons. But right now, fuck it.
        //get the name and type of the structured from the structure data object
        //GameObject newStructure = ObjectPooler.SharedInstance.GetPooledObject(structure.type);

        /*
        if (newStructure != null)
        {

            newStructure.transform.position = new Vector3 (raycastHitPoint.x, structureVertSize, raycastHitPoint.z);
            newStructure.SetActive(true);
            gameObject.SetActive(false);

            
            //Debug.Log("object " +structure.type +  " placed");

        }


        Debug.Log(structure.type);
        */

    }

}
