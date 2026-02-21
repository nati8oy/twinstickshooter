using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] public Transform movePositionTransform;

    private NavMeshAgent navMeshAgent;
    private Transform moveTarget;
    private float positionChange = 4f;
   // [SerializeField] EnemyData enemyData;

    public GameObject[] navPoints;
    public bool followPlayer;
    public int interval = 20;


    private Vector3 startingPosition;
    private Vector3 roamPosition;

    /*
    private Behaviour behaviour;
    private enum Behaviour
    {
        Free,
        FollowPlayer,
        FollowEndPoint,
    }
    */

    private void Start()
    {
        startingPosition = gameObject.transform.position;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //fill the array with the nav points that are tagged with "nav point"
        navPoints = GameObject.FindGameObjectsWithTag("nav point");
    }

    private void OnEnable()
    {
        if (followPlayer)
        {
            if (GameManager.Instance != null && GameManager.Instance.player != null)
            {
                movePositionTransform = GameManager.Instance.player.transform;
            }
        }
        else if (navPoints.Length > 0)
        {
            movePositionTransform = navPoints[Random.Range(0, navPoints.Length)].transform;
            roamPosition = navPoints[Random.Range(0, navPoints.Length)].transform.position;
        }
    }

    private Vector3 GetRoamingPosition()
    {
       return startingPosition + Utilities.GetRandomDir() * Random.Range(10f, 70f);
    }

    private void Update()
    {

        //this returns to roaming behaviour if followPlayer is false
         if (followPlayer == false)
        {

            navMeshAgent.destination = roamPosition;


            float reachedPositionDistance = 10f;
            if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
            {
                //reach roam position
                roamPosition = navPoints[Random.Range(0, 5)].transform.position;

            }

        }

      

        
    }


}
