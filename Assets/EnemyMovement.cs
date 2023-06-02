using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float roamingRadius = 10f;
    public float detectionRadius = 5f;
    public float chaseSpeed = 10f;
    public float delayDuration = 2f;

    private Vector3 initialPosition;
    private Vector3 destination;
    private NavMeshAgent navMeshAgent;
    private bool isDelaying;
    private float delayTimer;
    private Transform playerTransform;
    private Transform endPointTransform;
    [SerializeField] private Transform currentPriorityTarget; // Added variable to store the current priority target

    private void Start()
    {
        initialPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player Alt").transform;
        endPointTransform = GameObject.Find("EndPoint").transform;
        currentPriorityTarget = playerTransform; // Set player as the initial priority target
        GenerateRandomDestination();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float distanceToEndPoint = Vector3.Distance(transform.position, endPointTransform.position);

        // Check if the current priority target is within the detection radius
        if (currentPriorityTarget == playerTransform && distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
        }
        else if (currentPriorityTarget == endPointTransform && distanceToEndPoint <= detectionRadius)
        {
            ChaseEndPoint();
        }
        else
        {
            if (HasReachedDestination())
            {
                if (!isDelaying)
                {
                    isDelaying = true;
                    delayTimer = 0f;
                }
                else
                {
                    if (delayTimer >= delayDuration)
                    {
                        isDelaying = false;
                        GenerateRandomDestination();
                        MoveToDestination();
                    }
                    else
                    {
                        delayTimer += Time.deltaTime;
                    }
                }
            }
            
        }

       
    }

    private void ChasePlayer()
    {
        isDelaying = false;
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(playerTransform.position);
    }

    private void ChaseEndPoint()
    {
        isDelaying = false;
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(endPointTransform.position);
    }

    private bool HasReachedDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void GenerateRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamingRadius;
        randomDirection += initialPosition;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomDirection, out navMeshHit, roamingRadius, -1);
        destination = navMeshHit.position;
    }

    private void MoveToDestination()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(destination);
    }

    public void SetPriorityTargetPlayer()
    {
        currentPriorityTarget = playerTransform;
    }

    public void SetPriorityTargetEndPoint()
    {
        currentPriorityTarget = endPointTransform;
    }
}
