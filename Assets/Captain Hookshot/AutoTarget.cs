using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 15f;
    [SerializeField] private LayerMask targetLayerMask;

    [Header("Debug")]
    [SerializeField] private bool drawDebugLines = true;

    public GameObject closestTarget;
    public GameObject closestGrapplePoint;
    public Vector3 attackDirection;
    public Vector3 grappleDirection;

    public bool HasTarget => closestTarget != null && closestTarget.activeInHierarchy;

    private List<GameObject> targetsInRadius = new List<GameObject>();
    private int currentTargetIndex = 0;
    private bool hasCycled = false;

    private void Start()
    {
        StartCoroutine(FindClosestTargetRoutine());
    }

    private IEnumerator FindClosestTargetRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            UpdateTargetsInRadius();
        }
    }

    private void UpdateTargetsInRadius()
    {
        targetsInRadius.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayerMask);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.activeInHierarchy)
            {
                targetsInRadius.Add(hit.gameObject);
            }
        }

        // Sort by distance
        targetsInRadius.Sort((a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.transform.position);
            float distB = Vector3.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        // If we haven't manually cycled, default to closest
        if (!hasCycled)
        {
            currentTargetIndex = 0;
        }

        // Clamp index if targets changed
        if (targetsInRadius.Count > 0)
        {
            // If the currently selected target left the radius, reset to closest
            if (hasCycled && closestTarget != null && !targetsInRadius.Contains(closestTarget))
            {
                currentTargetIndex = 0;
                hasCycled = false;
            }

            currentTargetIndex = Mathf.Clamp(currentTargetIndex, 0, targetsInRadius.Count - 1);
            closestTarget = targetsInRadius[currentTargetIndex];
            attackDirection = (closestTarget.transform.position - transform.position).normalized;
        }
        else
        {
            closestTarget = null;
            attackDirection = transform.forward;
            currentTargetIndex = 0;
            hasCycled = false;
        }

        if (drawDebugLines && closestTarget != null)
        {
            Debug.DrawLine(transform.position, closestTarget.transform.position, Color.green, 0.2f);
        }
    }

    public void CycleTarget()
    {
        if (targetsInRadius.Count <= 1) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetsInRadius.Count;
        closestTarget = targetsInRadius[currentTargetIndex];
        attackDirection = (closestTarget.transform.position - transform.position).normalized;
        hasCycled = true;
    }

    public void FindClosestGrapplePoint()
    {
        GameObject[] grapplePoints = GameObject.FindGameObjectsWithTag("grapple point");

        float closestDistance = Mathf.Infinity;
        closestGrapplePoint = null;

        foreach (GameObject grapplePoint in grapplePoints)
        {
            if (grapplePoint != null)
            {
                float distance = Vector3.Distance(transform.position, grapplePoint.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGrapplePoint = grapplePoint;
                }
            }
        }

        if (closestGrapplePoint != null)
        {
            grappleDirection = (closestGrapplePoint.transform.position - transform.position).normalized;

            if (drawDebugLines)
            {
                Debug.DrawLine(transform.position, closestGrapplePoint.transform.position, Color.magenta);
            }
        }
    }
}
