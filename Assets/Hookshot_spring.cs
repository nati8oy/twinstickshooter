using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hookshot_spring : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;

    private SpringJoint joint;

    private RaycastHit hit;

    [Header("Inputs")]
    [SerializeField] private InputAction hookshot;
    [SerializeField] private InputAction hookshotPull;
    [SerializeField] private InputAction jump;

    [SerializeField] private Transform hook;

    [SerializeField] Transform shotPoint;


    [SerializeField] float hookshotMaxRange = 100f;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        
    }


    private void Start()
    {
        hookshot = new InputAction(binding: "<Mouse>/leftButton");
        hookshot.performed += _ => StartGrapple();
       // hookshot.performed += _ => HandleHookshotStart();
        hookshot.Enable();


        hookshotPull = new InputAction(binding: "<Mouse>/rightButton");
        //hookshotPull.performed += _ => ThrowCarriedObject();
        hookshotPull.Enable();


        jump = new InputAction(binding: "<Keyboard>/space");
        jump.performed += _ => StopGrapple();
        jump.Enable();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        Debug.Log("Grapple started!");
        if (Physics.Raycast(shotPoint.position, shotPoint.forward, out hit, hookshotMaxRange))
        {
            grapplePoint = hit.point;

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            hook.transform.position = grapplePoint;

            float distanceFromPoint = Vector3.Distance(gameObject.transform.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

        }

    }

    private void StopGrapple()
    {

    }

    private void DrawRope()
    {

        Debug.Log("rope drawn");
        lr.SetPosition(0, shotPoint.position);
        lr.SetPosition(1, grapplePoint);
    }
}
