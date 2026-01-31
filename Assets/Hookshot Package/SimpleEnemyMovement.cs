using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using MoreMountains.Feedbacks;  

public class SimpleEnemyMovement : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;
    private Transform player;
    [SerializeField] private UnityEvent onStun;
    [SerializeField] CM_Hookshot hookshotScript;

    private bool stunned;

    private void Start()
    {
        //set the hookshot script referennce
        hookshotScript = GameObject.Find("PlayerHookshot").GetComponent<CM_Hookshot>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set the destination to player's initial position
        navMeshAgent.SetDestination(player.position);
    }

    private void Update()
    {
        if (navMeshAgent.enabled == true)
        {
            // Update the destination to player's current position
            navMeshAgent.SetDestination(player.position);

        }
    }

    private void OnDisable()
    {
        if (hookshotScript.hitTarget == this)
        {
            //hookshotScript.hitTarget = null;
            Debug.Log("hitTarget is: " + hookshotScript.hitTarget);
           
        }
    }
}

