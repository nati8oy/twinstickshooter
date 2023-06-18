using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Hookshot : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private InputAction grapple;

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask whatIsGrappleable;
    private PlayerControls playerControls;

    private Rigidbody rb;
    private float rbDrag;
    private bool activeGrapple;

    [SerializeField] private LineRenderer lr;

    [SerializeField] private Transform shotPoint;

    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    private Vector3 grapplePoint;

    [SerializeField] private float grapplingCooldown;
    private float grapplingCooldownTimer;

    private bool grappling;
    private Vector3 velocityToSet;


    [SerializeField] private float overshootYAxis;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        ///player controls
        grapple = new InputAction(binding: "<Mouse>/leftButton");
        //edit mode is handled by functions inside of the builder manager script
        grapple.performed += _ => StartGrapple();
        grapple.Enable();


        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();

        rbDrag = rb.drag; 
       

    }

    // Update is called once per frame
    void Update()
    {
        if (grapplingCooldownTimer > 0)
        {
            grapplingCooldownTimer -= Time.deltaTime;
        }

        if (activeGrapple)
        {
            rb.drag = 0;
            gameObject.GetComponent<TwinStickMovement>().enabled = false;
        } else
        {
            rb.drag = rbDrag;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void SetVelocity()
    {
       rb.velocity = velocityToSet;
    }

    private void StartGrapple()
    {
        Debug.Log("grappled!");

        if (grapplingCooldown > 0) return;
        {
            grappling = true;
        }

        RaycastHit hit;
        if (Physics.Raycast (shotPoint.position, shotPoint.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }

        else {
            grapplePoint = shotPoint.position + shotPoint.forward* maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);

        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);

    }
    private void StopGrapple() {

        grappling = false;
        grapplingCooldownTimer = grapplingCooldown;
        lr.enabled = false;

        activeGrapple = false;
        gameObject.GetComponent<TwinStickMovement>().enabled = true;
        gameObject.GetComponent<TwinStickMovement>().HandleMovement();


    }

    private void ExecuteGrapple()
    {
        //pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }


    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    private Vector3 CalculateJumpVelocity( Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity* trajectoryHeight);

        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2* trajectoryHeight / gravity)
            + Mathf.Sqrt(2*(displacementY - trajectoryHeight)/gravity));


        return velocityXZ + velocityY;
    }
}
