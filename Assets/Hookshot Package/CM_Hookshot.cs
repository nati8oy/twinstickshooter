using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEditor.PackageManager;

public class CM_Hookshot : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Inputs")]
    [SerializeField] private InputAction hookshot;
    [SerializeField] private InputAction hookshotPull;
    [SerializeField] private InputAction cancel;
    [SerializeField] private InputAction pull;
    private InputAction cycleTarget;
    private InputAction autoAction;
    private InputAction autoCancel;

    [SerializeField] private LayerMask layerMask;

    public Transform shotPoint;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform carryPoint;

    [SerializeField] private float throwForce = 2000f;

    private Vector3 hookshotPosition;

    [SerializeField] private LineRenderer lr;
    public Vector3 characterVelocityMomentum;

    private CharacterController controller;

    private bool hitSomething =false;

    private float t = 0f;
    private RaycastHit raycastHit;

    private Vector2 movement;
    private Vector3 playerVelocity;
    [SerializeField] private PlayerControls playerControls;

    [SerializeField] private SpringJoint joint;

    [SerializeField] private CharacterController characterController;

    private float hookshotMaxRange;
    private int hookshotStrength;
    private float hookshotBounciness;
    private float hookshotThrowAngle;

    public bool isGrappling = false;
    [HideInInspector] public float dragSpeedMultiplier = 1f;
    [Header("Carry Speed Multipliers")]
    [SerializeField] private float lightCarrySpeed = 1f;
    [SerializeField] private float mediumCarrySpeed = 0.7f;
    [SerializeField] private float heavyCarrySpeed = 0.45f;

    [Header("Throw")]
    [SerializeField] private float throwResetDelay = 1f;

    [Header("Feedbacks")]
    [SerializeField] private UnityEvent onGrapple;
    [SerializeField] private UnityEvent OnHookshotHit;
    [SerializeField] private UnityEvent onGrappleDrag;
    [SerializeField] private UnityEvent onThrow;
    [SerializeField] private UnityEvent stopFeedbacks;


    [Header("Targeting")]
    public GameObject hitTarget;
    [SerializeField] GameObject[] targets;
    [SerializeField] bool targetVisible;
    [SerializeField] Transform crosshair;
    private RaycastHit targetingRaycast;
    [SerializeField] private AutoTarget autoTargetScript;
    [SerializeField] private TargetIndicator targetIndicator;
    [SerializeField] private GameObject lockOnTarget;


    [Header("Hookshot")]

    [Header("Rope Animation")]
    [Tooltip("Number of segments in the line renderer. Higher = smoother curve.")]
    [SerializeField] private int ropeQuality = 20;
    [Tooltip("How fast the wave settles. Higher = wave dies out faster.")]
    [SerializeField] private float ropeDamper = 7f;
    [Tooltip("Spring stiffness. Higher = wave snaps back faster with tighter oscillation.")]
    [SerializeField] private float ropeStrength = 40f;
    [Tooltip("Initial kick when hookshot fires. Higher = bigger starting wave.")]
    [SerializeField] private float ropeVelocity = 20f;
    [Tooltip("Number of sine wave peaks along the rope.")]
    [SerializeField] private float ropeWaveCount = 3f;
    [Tooltip("Wave amplitude multiplier. Higher = wider sideways curves.")]
    [SerializeField] private float ropeWaveHeight = 3f;
    [Tooltip("How fast the rope visually travels from gun to target. Higher = faster.")]
    [SerializeField] private float ropeLerpSpeed = 12f;
    [Tooltip("Controls where the wave is strongest along the rope. Peaks in the middle by default.")]
    [SerializeField] private AnimationCurve ropeAffectCurve = new AnimationCurve(
        new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f)
    );
    private Spring ropeSpring;
    private Vector3 currentGrapplePosition;
    private State pendingState;

    [Header("Fuzzy Targeting")]
    [SerializeField] private float hookshotArcAngle = 20f;

    private float hookshotSpeedMin;
    private float hookshotSpeedMax;

    [SerializeField] private HookshotData hookshotData;
    
    //current state the player is in e.g. grappling, normal, carrying, hookshot attached
    public State state;
    public bool hookshotAutoPull;

    public enum State
    {
        Normal, HookshotLaunched, HookshotFlyingPlayer, HookshotPull, HookshotAttached, HookshotCarry,
    }


    private void OnEnable()
    {   
        //grab all the data from the hookshot data asset
        hookshotMaxRange = hookshotData.maxRange;
        hookshotSpeedMin = hookshotData.speedMin;
        hookshotSpeedMax = hookshotData.speedMax;
        hookshotStrength = (int)hookshotData.strength;
        hookshotBounciness = hookshotData.bounciness;
        throwForce = hookshotData.throwForce;
        hookshotThrowAngle = hookshotData.throwAngle;

        ropeSpring = new Spring();
        ropeSpring.SetTarget(0);

        if (joint != null)
            joint.spring = hookshotBounciness;

        // Sync auto-target detection radius to hookshot range
        if (autoTargetScript != null)
            autoTargetScript.detectionRadius = hookshotMaxRange;
    }

    private void Awake()
    {
        state = State.Normal;
    }


    // Start is called before the first frame update
    void Start()
    {
       

        characterController = gameObject.GetComponent<CharacterController>();

        hookshotPull = new InputAction();
        hookshotPull.AddBinding("<Mouse>/rightButton");
        hookshotPull.AddBinding("<Gamepad>/rightShoulder");
        hookshotPull.performed += _ =>
        {
            switch (state)
            {
                case State.Normal:
                case State.HookshotFlyingPlayer:
                    HandleHookshotStart();
                    break;
                case State.HookshotAttached:
                    ActivateHookshotPull();
                    break;
                case State.HookshotCarry:
                    ThrowCarriedObject();
                    break;
            }
        };
        hookshotPull.Enable();

        cancel = new InputAction();
        cancel.AddBinding("<Keyboard>/leftShift");
        cancel.AddBinding("<Gamepad>/leftShoulder");
        cancel.Enable();

        pull = new InputAction();
        pull.AddBinding("<Keyboard>/e");
        pull.AddBinding("<Gamepad>/buttonSouth");
        pull.performed += _ => ActivateHookshotPull();
        pull.Enable();

        cycleTarget = new InputAction();
        cycleTarget.AddBinding("<Keyboard>/tab");
        cycleTarget.AddBinding("<Gamepad>/buttonNorth");
        cycleTarget.performed += _ =>
        {
            if (DebugManager.Instance != null && DebugManager.Instance.autoTargetEnabled && autoTargetScript != null)
            {
                autoTargetScript.CycleTarget();
            }
        };
        cycleTarget.Enable();

        // Auto-target mode: South button is contextual (fire/pull/throw)
        autoAction = new InputAction(binding: "<Gamepad>/buttonSouth");
        autoAction.performed += _ =>
        {
            if (DebugManager.Instance == null || !DebugManager.Instance.autoTargetEnabled) return;

            switch (state)
            {
                case State.Normal:
                case State.HookshotFlyingPlayer:
                    HandleHookshotStart();
                    break;
                case State.HookshotAttached:
                    ActivateHookshotPull();
                    break;
                case State.HookshotCarry:
                    ThrowCarriedObject();
                    break;
            }
        };
        autoAction.Enable();

        // Auto-target mode: East button cancels hookshot
        autoCancel = new InputAction(binding: "<Gamepad>/buttonEast");
        autoCancel.performed += _ =>
        {
            if (DebugManager.Instance == null || !DebugManager.Instance.autoTargetEnabled) return;

            if (state == State.HookshotCarry)
            {
                ReleaseCarriedObject();
            }
            else if (state != State.Normal)
            {
                CancelHookshot();
            }
        };
        autoCancel.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        //stores the lock on target in the variable
        lockOnTarget = GetComponent<AutoTarget>().closestTarget;

        // Target highlight: show emission glow on the targeted object when in Normal state
        if (state == State.Normal && targetIndicator != null)
        {
            // Auto-target mode: use the tracked closest target
            if (DebugManager.Instance != null && DebugManager.Instance.autoTargetEnabled
                && autoTargetScript != null && autoTargetScript.HasTarget)
            {
                targetIndicator.ShowAtTarget(autoTargetScript.closestTarget);
            }
            // Standard mode: raycast + fuzzy targeting
            else if (FindBestTarget(out targetingRaycast))
            {
                targetIndicator.ShowAtTarget(targetingRaycast.collider.gameObject);
            }
            else
            {
                targetIndicator.Hide();
            }
        }
        else if (targetIndicator != null)
        {
            targetIndicator.Hide();
        }


        switch (state)
        {
            default:
            case State.Normal:
            lr.enabled = false;
            lr.positionCount = 0;
            isGrappling = false;
            dragSpeedMultiplier = 1f;
                break;
            case State.HookshotLaunched:
                LaunchHookshot();
                break;
            case State.HookshotFlyingPlayer:
                HandleHookshotMovement();
                hook.gameObject.SetActive(true);
                
                break;
            case State.HookshotPull:
                HandleHookshotPull();
                hook.gameObject.SetActive(true);
                break;
            case State.HookshotAttached:
                if (hookshotAutoPull && !IsGummyTooStrong())
                {
                    HandleHookshotPull();
                }
                else
                {
                    HandleHookshotAttached();
                }

                hook.gameObject.SetActive(true);
                break;
            case State.HookshotCarry:
                HandleHookshotCarry();
                hook.gameObject.SetActive(false);
                isGrappling = false;

                //slow player based on gummy weight when carrying
                if (hitTarget != null)
                {
                    GummyLevel gl = hitTarget.GetComponent<GummyLevel>();
                    if (gl != null)
                    {
                        switch (gl.weight)
                        {
                            case GummyLevel.Weight.Light:  dragSpeedMultiplier = lightCarrySpeed;  break;
                            case GummyLevel.Weight.Medium: dragSpeedMultiplier = mediumCarrySpeed; break;
                            case GummyLevel.Weight.Heavy:  dragSpeedMultiplier = heavyCarrySpeed;  break;
                        }
                    }
                    else
                    {
                        dragSpeedMultiplier = 1f;
                    }
                }

                if (IsCancelPressed())
                {
                    ReleaseCarriedObject();
                }

                break;
        }


        //target the closest object on layer 11
        //get all of the nearby enemies and put them in an array
        targets = GameObject.FindGameObjectsWithTag("enemy");

    }

private void LateUpdate(){
    if (!lr.enabled) return;

    // Update the spring simulation for rope wave decay
    ropeSpring.SetDamper(ropeDamper);
    ropeSpring.SetStrength(ropeStrength);
    ropeSpring.Update(Time.deltaTime);

    // Determine the actual target endpoint for the rope
    Vector3 ropeTarget = hookshotPosition;
    if ((state == State.HookshotPull || state == State.HookshotAttached) && hitTarget != null)
    {
        ropeTarget = hitTarget.transform.position;
    }

    // Lerp the rope endpoint toward the target (animates the rope shooting out during launch)
    currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, ropeTarget, Time.deltaTime * ropeLerpSpeed);

    // Check if rope reached target during launch — transition to pending state
    if (state == State.HookshotLaunched && Vector3.Distance(currentGrapplePosition, hookshotPosition) < 0.5f)
    {
        if (pendingState == State.HookshotFlyingPlayer)
        {
            onGrapple.Invoke();
            OnHookshotHit.Invoke();
        }
        state = pendingState;
    }

    var gunTipPosition = shotPoint.position;

    // Use the FINAL target direction for the up vector (not currentGrapplePosition)
    // so it's stable from the first frame and never zero
    var ropeDirection = (ropeTarget - gunTipPosition).normalized;
    if (ropeDirection.sqrMagnitude < 0.01f) return;
    var up = Quaternion.LookRotation(ropeDirection) * Vector3.right;

    // Ensure line renderer has enough points for the curve
    if (lr.positionCount != ropeQuality + 1)
    {
        lr.positionCount = ropeQuality + 1;
    }

    for (var i = 0; i < ropeQuality + 1; i++)
    {
        var delta = i / (float)ropeQuality;
        var offset = up * ropeWaveHeight * Mathf.Sin(delta * ropeWaveCount * Mathf.PI) * ropeSpring.Value *
                     ropeAffectCurve.Evaluate(delta);

        lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
    }
}

    private bool FindFuzzyTarget(out RaycastHit fuzzyHit)
    {
        fuzzyHit = default;
        Collider[] colliders = Physics.OverlapSphere(shotPoint.position, hookshotMaxRange, layerMask);

        float bestAngle = float.MaxValue;
        Collider bestCollider = null;

        foreach (Collider col in colliders)
        {
            Vector3 dirToTarget = (col.transform.position - shotPoint.position).normalized;
            float angle = Vector3.Angle(shotPoint.forward, dirToTarget);

            if (angle < hookshotArcAngle && angle < bestAngle)
            {
                bestAngle = angle;
                bestCollider = col;
            }
        }

        if (bestCollider == null)
            return false;

        // Raycast toward the chosen target to get a proper RaycastHit struct
        Vector3 direction = (bestCollider.transform.position - shotPoint.position).normalized;
        float distance = Vector3.Distance(shotPoint.position, bestCollider.transform.position);
        if (Physics.Raycast(shotPoint.position, direction, out fuzzyHit, distance + 1f, layerMask))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Finds the best hookshot target: direct raycast first, then fuzzy targeting as fallback.
    /// </summary>
    private bool FindBestTarget(out RaycastHit bestHit)
    {
        if (Physics.Raycast(shotPoint.position, shotPoint.forward, out bestHit, hookshotMaxRange, layerMask))
        {
            return true;
        }

        if (DebugManager.Instance != null && DebugManager.Instance.fuzzyTargetingEnabled)
        {
            return FindFuzzyTarget(out bestHit);
        }

        bestHit = default;
        return false;
    }

    private void HandleHookshotStart()
    {
        if (shotPoint == null) return;

        //if you are normal or flying through the air then shoot the hookshot
        if (state == State.Normal || state == State.HookshotFlyingPlayer)
        {
            
            {

                /*
                hitTarget = lockOnTarget;
                hook.position = lockOnTarget.transform.position;
                hookshotPosition = lockOnTarget.transform.position;
                state = State.HookshotPull;
                */


                //gameObject.GetComponent<AutoTarget>().FindClosestGrapplePoint();


                
                bool directHit = FindBestTarget(out raycastHit);

                if (directHit)
                    {

                        //check the layer of the object that was hit
                        //if the layer is grappleable, then set the hookshot position to the hit point
                        //if not, then set the hookshot position to the maximum distance of the hookshot
                        if (raycastHit.collider.gameObject.layer == 10)
                        {
                            hook.position = raycastHit.point;
                            hookshotPosition = raycastHit.point;
                            pendingState = State.HookshotFlyingPlayer;
                        }

                        //this is the pullable object layer
                        if (raycastHit.collider.gameObject.layer == 11)
                        {
                            hook.position = raycastHit.point;
                            hookshotPosition = raycastHit.point;
                            pendingState = State.HookshotAttached;

                            // Disable gravity so the object doesn't drop while being hooked/pulled
                            var rb = raycastHit.collider.gameObject.GetComponent<Rigidbody>();
                            if (rb != null) rb.useGravity = false;

                            //if hookshot is strong enough, freeze the gummy in place
                            GummyLevel gummyLevel = raycastHit.collider.gameObject.GetComponent<GummyLevel>();
                            if (gummyLevel != null && hookshotStrength >= gummyLevel.WeightValue)
                            {
                                GummyBehaviour gummyBehaviour = raycastHit.collider.gameObject.GetComponent<GummyBehaviour>();
                                if (gummyBehaviour != null)
                                {
                                    gummyBehaviour.enabled = false;
                                }
                                else
                                {
                                    SimpleEnemyMovement enemyMovement = raycastHit.collider.gameObject.GetComponent<SimpleEnemyMovement>();
                                    if (enemyMovement != null)
                                    {
                                        enemyMovement.enabled = false;
                                    }
                                }
                            }
                        }

                        //this is the layer for collectibles that you can pull towards yourself
                        if (raycastHit.collider.gameObject.layer == 12)
                        {
                            hook.position = raycastHit.point;
                            hookshotPosition = raycastHit.point;
                            pendingState = State.HookshotPull;

                            // Disable gravity so the object doesn't drop while being pulled
                            var rb = raycastHit.collider.gameObject.GetComponent<Rigidbody>();
                            if (rb != null) rb.useGravity = false;
                        }

                        //this is the layer for moveable objects that can be pulled but not carried
                        if (raycastHit.collider.gameObject.layer == 13)
                        {
                            hook.position = raycastHit.point;
                            hookshotPosition = raycastHit.point;
                            pendingState = State.HookshotAttached;

                            // Disable gravity so the object doesn't drop while being hooked/pulled
                            var rb = raycastHit.collider.gameObject.GetComponent<Rigidbody>();
                            if (rb != null) rb.useGravity = false;
                        }


                    //set the hit target to whatever the raycast has hit
                    hitTarget = raycastHit.collider.gameObject;
                    //Debug.Log("hit target is " + hitTarget.name);

                    //make the raycast ignore the hook object's collider
                    Physics.IgnoreCollision(raycastHit.collider, hook.GetComponent<Collider>());

                    // Enter launched state — rope animates toward target before transitioning
                    state = State.HookshotLaunched;
                    currentGrapplePosition = shotPoint.position;
                    ropeSpring.Reset();
                    ropeSpring.SetVelocity(ropeVelocity);
                    lr.positionCount = ropeQuality + 1;
                }


            }

        }

        lr.enabled = true;
    }

    private void HandleHookshotMovement()
    {

        isGrappling = true;

        
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        // Calculate the additional movement based on the current velocity of the Character Controller
        Vector3 additionalMovement = characterController.velocity * Time.deltaTime;

        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        // Calculate the final movement vector by combining the hookshot direction, additional movement, and hookshot speed
        Vector3 move = hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime + additionalMovement;

        characterController.Move(move);

        if (Vector3.Distance(transform.position, hookshotPosition) < 2f)
        {
            // Reached hookshot position
            state = State.Normal;
            lr.enabled = false;
            lr.positionCount = 0;
            // characterController.enabled = false;
        }

        //if the player has jumped/cancelled while hookshotting, cancel the hookshot
        if (IsCancelPressed())
        {
            CancelHookshot();
        }

        //dampen speed of momentum
        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 20f;

            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }           

        }

    }


    private void HandleHookshotAttached()
    {
        lr.enabled = true;
        hook.gameObject.transform.position = hookshotPosition;
        hookshotPosition = hitTarget.transform.position;


        if (hitTarget == null)
        {
            CancelHookshot();
        }

        if (!IsGummyTooStrong())
        {
            //deactivate the navmesh agent of the object you've hit
            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = false;
            }

            if (joint != null)
                joint.connectedBody = hitTarget.GetComponent<Rigidbody>();
        }

        //if the player has jumped/cancelled while attached, cancel the hookshot
        if (IsCancelPressed())
        {
            CancelHookshot();
        }

    }

    private void HandleHookshotPull()
    {

        if (hitTarget != null)
        {

            //deactivate the navmesh agent
            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = false;
            }

            //destroy the joint for an attached object
            if (hitTarget.GetComponent<FixedJoint>())
            {
                Destroy(hitTarget.GetComponent<FixedJoint>());
            }

            
            Vector3 hookshotDir = (hookshotPosition + transform.position).normalized;

            //pull the hittarget towards the player
            hitTarget.transform.position = Vector3.MoveTowards(hitTarget.transform.position, transform.position, 40f * Time.deltaTime);
            hook.gameObject.transform.position = hitTarget.transform.position;

            //match the hook to the same position of the hit target as it's getting pulled back
            //hook.gameObject.transform.position = hitTarget.transform.position;


            lr.enabled = true;

            //check the distance between the hookshot position and the player
            //and if it's less than 1f, then cancel the hookshot
            if (Vector3.Distance(transform.position, hitTarget.transform.position) < 1f)
            {
                if (raycastHit.collider.gameObject.layer == 11)
                {
                    state = State.HookshotCarry;
                }

                else
                {
                    CancelHookshot();
                }

            }

            //if the player has jumped/cancelled while pulling, cancel the hookshot
            if (IsCancelPressed())
            {
                CancelHookshot();
            }
        }

    }

    private void HandleHookshotCarry()
    {
        if (hitTarget != null)
        {
            DetachSpringJoint();

            var hitTargetRB = hitTarget.GetComponent<Rigidbody>();

            //carry the hittarget game object with the player at the shot point
            hitTargetRB.useGravity = false;
            hitTargetRB.freezeRotation = true;
            hitTarget.transform.position = carryPoint.transform.position;

            lr.enabled = false;
            lr.positionCount = 0;

        }
    }


        private void ThrowCarriedObject()
        {

        if (hitTarget != null)
        {

            //play throw feedbacks
            onThrow.Invoke();

            var hitTargetRB = hitTarget.GetComponent<Rigidbody>();


            //if you're carrying an object, throw it
            if (state == State.HookshotCarry)
            {
                state = State.Normal;

                //check if it's got a rigid body or not
                if (hitTarget.GetComponent<Rigidbody>())
                {
                    //launch the hittarget object in the direction the player is facing
                    hitTargetRB.useGravity = true;
                    hitTargetRB.freezeRotation = false;


                    //throw it in the direction the player is facing, angled upward
                    Vector3 throwDir = Quaternion.AngleAxis(-hookshotThrowAngle, transform.right) * transform.forward;
                    hitTargetRB.AddForce(throwDir * throwForce);

                    /*
                    //check if the auto targeting is on and if the object you're throwing isn't an enemy
                    if (autoTargetScript != null && hitTarget.tag != "enemy")
                    {
                        hitTargetRB.AddForce(autoTargetScript.attackDirection * throwForce);

                    }
                    else
                    {
                        hitTargetRB.AddForce(transform.forward * throwForce);
                    }
                    */

                    //add a bit spin to the object when you throw it
                    hitTargetRB.AddTorque(transform.forward * throwForce);
                }


                //check if the object has a navmesh agent and if so reset it so it works again after being thrown
                if (hitTarget.GetComponent<NavMeshAgent>())
                {
                    hitTarget.GetComponent<NavMeshAgent>().enabled = false;
                    Invoke("ResetEnemyMovement", throwResetDelay);
                }
            }
        }
    }

    private void LaunchHookshot()
    {
        lr.enabled = true;

        // Allow cancel during launch
        if (IsCancelPressed())
        {
            CancelHookshot();
        }
    }

    private void ReleaseCarriedObject()
    {
        if (hitTarget != null)
        {
            var hitTargetRB = hitTarget.GetComponent<Rigidbody>();
            if (hitTargetRB != null)
            {
                hitTargetRB.useGravity = true;
                hitTargetRB.freezeRotation = false;
                hitTargetRB.AddForce(transform.forward * throwForce * 0.15f);
            }

            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = false;
                Invoke("ResetEnemyMovement", 1f);
            }
        }

        stopFeedbacks.Invoke();
        ResetEnemyMovement();
        DetachSpringJoint();
        state = State.Normal;
        lr.enabled = false;
            lr.positionCount = 0;
    }

    public void CancelHookshot()
    {
        stopFeedbacks.Invoke();

        // Re-enable gravity on the hooked object
        if (hitTarget != null)
        {
            var hitTargetRB = hitTarget.GetComponent<Rigidbody>();
            if (hitTargetRB != null)
            {
                hitTargetRB.useGravity = true;
            }
        }

        //reset the enemy movement
        ResetEnemyMovement();
        DetachSpringJoint();

        if (hitTarget != null && hitTarget.GetComponent<NavMeshAgent>())
        {
            hitTarget.GetComponent<NavMeshAgent>().enabled = true;
        }

        //characterController.enabled = false;
        //Invoke("ResetController", 0.5f);

        //get the current velocity of the player
        Vector3 currentVelocity = characterController.velocity;

        //Debug.Log("current velocity " + currentVelocity + " CVM " + characterVelocityMomentum);

        //add the momentum to the current velocity
        //Vector3 momentumVelocityAddition = characterVelocityMomentum;
        /*
        currentVelocity += momentumVelocityAddition;
        */
        //momentumVelocityAddition.y = 0;


        state = State.Normal;
        lr.enabled = false;
            lr.positionCount = 0;

    }

    public void OnPlayerJump()
    {
        if (state != State.Normal)
        {
            CancelHookshot();
        }
    }

    private void ActivateHookshotPull()
    {
        if(state == State.HookshotAttached && !IsGummyTooStrong())
        {
            state = State.HookshotPull;

        }
    }

    private bool IsGummyTooStrong()
    {
        if (hitTarget == null) return false;
        GummyLevel gummyLevel = hitTarget.GetComponent<GummyLevel>();
        if (gummyLevel == null) return false;
        return gummyLevel.WeightValue > hookshotStrength;
    }

    private bool IsCancelPressed()
    {
        if (DebugManager.Instance != null && DebugManager.Instance.autoTargetEnabled)
        {
            return autoCancel.ReadValue<float>() > 0 || cancel.ReadValue<float>() > 0;
        }
        return cancel.ReadValue<float>() > 0;
    }

    private void DetachSpringJoint()
    {
        //if the spring joint is connect to something, disconnect it.
        if (joint != null && joint.connectedBody != null)
        {
            joint.connectedBody = null;
        }
    }

    private void ResetController()
    {
        characterController.enabled = true;

    }

    private void ResetEnemyMovement()
    {
        if (hitTarget != null)
        {
            if (hitTarget.GetComponent<NavMeshAgent>())
            {
                hitTarget.GetComponent<NavMeshAgent>().enabled = true;
            }

            GummyBehaviour gummyBehaviour = hitTarget.GetComponent<GummyBehaviour>();
            if (gummyBehaviour != null)
            {
                gummyBehaviour.enabled = true;
            }
            else
            {
                SimpleEnemyMovement enemyMovement = hitTarget.GetComponent<SimpleEnemyMovement>();
                if (enemyMovement != null)
                {
                    enemyMovement.enabled = true;
                }
            }
        }


    }

    /*
    //visualise the raycast in the scene view using gizmos
     void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(shotPoint.position, shotPoint.position + shotPoint.forward * hookshotMaxRange);
    }
    */
}
