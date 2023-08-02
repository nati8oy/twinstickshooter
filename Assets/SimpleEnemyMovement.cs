using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SimpleEnemyMovement : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;
    private Transform player;

    private void Start()
    {
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



}

