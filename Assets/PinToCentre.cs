using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PinToCentre : MonoBehaviour
{
    Vector3 pos = new Vector3(500, 500, 0);

    [SerializeField] private LayerMask layerMask;
    private Vector3 raycastHitPoint;
    private Vector3 m_Size, m_Min, m_Max;
    private Collider objectCollider;
    private Vector3 colliderCentrePoint;
    private float structureVertSize;



    private void OnEnable()
    {
        //Fetch the Collider from the GameObject
        objectCollider = GetComponent<Collider>();

        m_Size = objectCollider.bounds.size;
        
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

    void Update()
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(pos);

        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
        {
            transform.position = raycastHit.point;
            raycastHitPoint = raycastHit.point;
            Debug.Log(raycastHit.point);
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.yellow);
        }
        */

        //main code to track mouse position on the screen using raycasting
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));


        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
        {
            transform.position = new Vector3(raycastHit.point.x, structureVertSize, raycastHit.point.z);
            raycastHitPoint = raycastHit.point;

        }


    }

}
