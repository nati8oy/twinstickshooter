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
    public bool isFollower;
    public int interval = 20;


    private Vector3 startingPosition;
    private Vector3 roamPosition;


    private void OnEnable()
    {
        movePositionTransform = GameManager.Instance.player.transform;
        roamPosition = navPoints[Random.Range(0, 5)].transform.position;
        //roamPosition = GetRoamingPosition();

        //Invoke(nameof(SetNewRandomPosition), positionChange);
    }

    private void Start()
    {
        startingPosition = gameObject.transform.position;
        //Debug.Log("first random location is: " + roamPosition);
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (isFollower)
        {
            movePositionTransform = GameManager.Instance.player.transform;

        }


        navPoints = GameObject.FindGameObjectsWithTag("nav point");

    }

    private Vector3 GetRoamingPosition()
    {
       return startingPosition + Utilities.GetRandomDir() * Random.Range(10f, 70f);
    }

    private void Update()
    {
        //Debug.DrawLine(startingPosition, roamPosition, color: Color.blue);

        
        if (isFollower)
        {
            navMeshAgent.destination = movePositionTransform.position;
             
        }
        
        else if(isFollower==false)
        {

            navMeshAgent.destination = roamPosition;


            float reachedPositionDistance = 10f;
            if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
            {
                //Debug.Log("position reached! " + roamPosition);

                //reach roam position
                roamPosition = navPoints[Random.Range(0, 5)].transform.position;
                //Debug.Log("new destination is: " + roamPosition);

            }

            /*
            float reachedPositionDistance = 10f;
            if (Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance)
            {
                Debug.Log("position reached! " + roamPosition);

                //reach roam position
                roamPosition = GetRoamingPosition();
                Debug.Log("new destination is: " + roamPosition);

            }*/

            //navMeshAgent.destination = navPoints[Random.Range(0, 5)].transform.position;

        }
    }

    public void SetNewRandomPosition()
    {
        roamPosition = GetRoamingPosition();
        Debug.Log("roam point changed");
    }

}
