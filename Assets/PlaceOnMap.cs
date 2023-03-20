using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlaceOnMap : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    public LayerMask layerMask;
    public Structure structure;

    //use this to reference the collider height
    private float structureVertSize;

    //use this to reference the mesh object for height
    private float vertMeshSize;

    public Vector3 raycastHitPoint;

    public InputAction placeBuildingAction;

    //checks if the object is blocked by another object
    public bool objectBlocked;

    private Collider objectCollider;
    private Vector3 colliderCentrePoint;
    private Vector3 m_Size, m_Min, m_Max;


    private void Awake()
    {
       mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        /*
        GameObject meshObject = transform.Find("mesh").gameObject;

        // Get the Mesh component from the child object
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

        // Get the bounds of the mesh in world space
        Bounds bounds = mesh.bounds;

        // Get the height of the mesh by accessing the bounds size along the y-axis
        vertMeshSize = bounds.size.y/2;

        */


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

        //this gets the vertical size of the object and subtracts it from itself to give us 0 so that it is level with the ground layer
        structureVertSize = m_Size.y - m_Size.y;


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

    void Update()
    {
        
    }


    public void PlaceObject()
    {

        if (objectBlocked == false)
        {
            Debug.Log("placed object");


            // Get the transform of the child game object
            Transform rotationTransform = transform.Find("RotationTarget");

            // Get the rotation of the child game object
            Quaternion rotation = rotationTransform.rotation;

            // You can then use the rotation as needed, for example:
            // Rotate the parent object based on the child object's rotation
            transform.rotation = rotation;

            //instantiates and adds models based on what is in their structure data object
            //sets it on the screen where the raycast hits
            //Instantiate(structure.structureModel, new Vector3(raycastHitPoint.x, structureVertSize, raycastHitPoint.z), Quaternion.identity);
            Instantiate(structure.structureModel, new Vector3(gameObject.transform.position.x, structureVertSize, gameObject.transform.position.z), rotation) ;


            Destroy(GameObject.FindGameObjectWithTag("ghost"));
        }
      
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("blocked = " + objectBlocked);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("blocked = " + objectBlocked);

    }

}
