using UnityEngine;
using UnityEngine.AI;

public class GummyBehaviour : MonoBehaviour
{
    public enum IdleMovement { Still, WanderShort, WanderMedium, WanderFar }
    public enum PlayerReaction { Follow, ScatterFar, ScatterNearby, Ignore }

    [Header("Behaviour Type")]
    public IdleMovement idleMovement = IdleMovement.WanderShort;
    public PlayerReaction playerReaction = PlayerReaction.Ignore;

    [Header("Player Detection")]
    [SerializeField] private float playerDetectionRadius = 8f;

    [Header("Idle Wander Distances")]
    [SerializeField] private float wanderShortRadius = 3f;
    [SerializeField] private float wanderMediumRadius = 7f;
    [SerializeField] private float wanderFarRadius = 15f;

    [Header("Idle Timing")]
    [SerializeField] private float idlePauseDuration = 1.5f;

    [Header("Scatter Distances")]
    [SerializeField] private float scatterFarDistance = 20f;
    [SerializeField] private float scatterNearbyDistance = 5f;

    [Header("Movement Speeds")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float scatterSpeed = 6f;

    [Header("Follow Settings")]
    [SerializeField] private float followStopDistance = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject goalExplosionPrefab;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private Vector3 homePosition;
    private bool isIdlePausing;
    private float idlePauseTimer;
    private bool isReactingToPlayer;
    private bool hasScattered;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        homePosition = transform.position;
        GenerateIdleDestination();
    }

    private void OnEnable()
    {
        isReactingToPlayer = false;
        hasScattered = false;
        isIdlePausing = false;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            GenerateIdleDestination();
        }
    }

    private void OnDisable()
    {
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.ResetPath();
        }
    }

    private void Update()
    {
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh || !navMeshAgent.enabled) return;

        if (playerTransform == null)
        {
            HandleIdleWander();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= playerDetectionRadius && playerReaction != PlayerReaction.Ignore)
        {
            if (!isReactingToPlayer)
            {
                isReactingToPlayer = true;
                hasScattered = false;
            }
            HandlePlayerReaction();
        }
        else
        {
            if (isReactingToPlayer)
            {
                isReactingToPlayer = false;
                hasScattered = false;
                GenerateIdleDestination();
            }
            HandleIdleWander();
        }
    }

    private void HandleIdleWander()
    {
        if (idleMovement == IdleMovement.Still) return;

        if (HasReachedDestination())
        {
            if (!isIdlePausing)
            {
                isIdlePausing = true;
                idlePauseTimer = 0f;
            }
            else
            {
                idlePauseTimer += Time.deltaTime;
                if (idlePauseTimer >= idlePauseDuration)
                {
                    isIdlePausing = false;
                    GenerateIdleDestination();
                }
            }
        }
    }

    private void GenerateIdleDestination()
    {
        if (idleMovement == IdleMovement.Still) return;

        float radius = idleMovement switch
        {
            IdleMovement.WanderShort => wanderShortRadius,
            IdleMovement.WanderMedium => wanderMediumRadius,
            IdleMovement.WanderFar => wanderFarRadius,
            _ => wanderShortRadius
        };

        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += homePosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            MoveToDestination(hit.position, wanderSpeed);
        }
    }

    private void HandlePlayerReaction()
    {
        switch (playerReaction)
        {
            case PlayerReaction.Follow:
                HandleFollow();
                break;
            case PlayerReaction.ScatterFar:
                HandleScatter(scatterFarDistance);
                break;
            case PlayerReaction.ScatterNearby:
                HandleScatter(scatterNearbyDistance);
                break;
            case PlayerReaction.Ignore:
                HandleIdleWander();
                break;
        }
    }

    private void HandleFollow()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > followStopDistance)
        {
            MoveToDestination(playerTransform.position, followSpeed);
        }
        else if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.ResetPath();
        }
    }

    private void HandleScatter(float scatterDistance)
    {
        if (hasScattered && !HasReachedDestination()) return;

        if (hasScattered) return;

        Vector3 fleeDirection = (transform.position - playerTransform.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * scatterDistance;

        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, scatterDistance, NavMesh.AllAreas))
        {
            MoveToDestination(hit.position, scatterSpeed);
        }

        hasScattered = true;
    }

    private void MoveToDestination(Vector3 destination, float speed)
    {
        if (!navMeshAgent.isOnNavMesh) return;
        navMeshAgent.speed = speed;
        navMeshAgent.SetDestination(destination);
    }

    private bool HasReachedDestination()
    {
        if (!navMeshAgent.isOnNavMesh) return false;
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

    public void SetHomePosition(Vector3 position)
    {
        homePosition = position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            HUD hud = FindObjectOfType<HUD>();
            if (hud != null)
            {
                hud.IncreaseScore();
            }

            if (goalExplosionPrefab != null)
            {
                Instantiate(goalExplosionPrefab, transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
    }
}
