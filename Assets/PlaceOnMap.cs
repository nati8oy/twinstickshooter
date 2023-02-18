using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlaceOnMap : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    [SerializeField] private LayerMask layerMask;
    private float structureVertSize;
    public Structure structure;


    public Vector3 raycastHitPoint;

    public InputAction placeBuildingAction;


    private Collider objectCollider;
    private Vector3 colliderCentrePoint;
    private Vector3 m_Size, m_Min, m_Max;


    private void Awake()
    {
       mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void OnEnable()
    {

        //set the RMB to place the structure
        placeBuildingAction = new InputAction(binding: "<DualShockGamepad>/buttonSouth");

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


        if (mainCamera != null)
        {

            //main code to track mouse position on the screen using raycasting
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f,0));


            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
            {
                transform.position = new Vector3(raycastHit.point.x, structureVertSize, raycastHit.point.z);
                raycastHitPoint = raycastHit.point;
            }

            //Mouse.current.leftButton.ReadValue();

        }

        else
        {
            Debug.LogError("Camera for Raycast reference is missing");
        }


    }

    private void OnDisable()
    {
        // Start listening for control changes.
        placeBuildingAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlaceObject()
    {
        Debug.Log("placed object");


        //instantiates and adds models based on what is in their structure data object
        //sets it on the screen where the raycast hits
        //Instantiate(structure.structureModel, new Vector3(raycastHitPoint.x, structureVertSize, raycastHitPoint.z), Quaternion.identity);
        Instantiate(structure.structureModel, new Vector3(gameObject.transform.position.x, structureVertSize, gameObject.transform.position.z), Quaternion.identity);


        Destroy(GameObject.FindGameObjectWithTag("ghost"));


    }


}
