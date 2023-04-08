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

    private void OnEnable()
    {
        //movePositionTransform = GameManager.Instance.player.transform;
        if (GameObject.Find("EndPoint"))
        {
            movePositionTransform = GameObject.Find("EndPoint").transform;

        }

        else
        {
            Debug.LogError("There is no EndPoint available");
        }

        roamPosition = navPoints[Random.Range(0, 5)].transform.position;

    }

    private void Start()
    {
        startingPosition = gameObject.transform.position;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (followPlayer)
        {
            movePositionTransform = GameObject.Find("EndPoint").transform;

            //movePositionTransform = GameManager.Instance.player.transform;

        }

        //fill the array with the nav points that are tagged with "nav point"
        navPoints = GameObject.FindGameObjectsWithTag("nav point");

    }

    private Vector3 GetRoamingPosition()
    {
       return startingPosition + Utilities.GetRandomDir() * Random.Range(10f, 70f);
    }

    private void Update()
    {

        //this is defaulted to whatever movePositionTransform.position is. It can be a stationary point or the player
        if (followPlayer)
        {
            navMeshAgent.destination = movePositionTransform.position;
             
        }

        //this returns to roaming behaviour if followPlayer is false
        else if(followPlayer==false)
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
