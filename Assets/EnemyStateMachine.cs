using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyStateMachine : MonoBehaviour
{
   [SerializeField] private enum EnemyState
    {
        chasing,
        carried,
        stunned,
    }

    [SerializeField] UnityEvent onStun;
    [SerializeField] float stunTime; 

    private bool canBeStunned;

    private NavMeshAgent navMeshAgent;

    private EnemyState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.stunned;
        
        if(gameObject.GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        

        // State machine logic based on current state
        switch (currentState)
        {
            case EnemyState.chasing:
                navMeshAgent.enabled = true;
                break;

            case EnemyState.carried:
                navMeshAgent.enabled = false;
                break;

            case EnemyState.stunned:
                navMeshAgent.enabled = false;
                //countdown timer
                if (stunTime > 0)
                {
                    stunTime -= Time.deltaTime;

                }
                else if (stunTime <= 0)
                {
                    stunTime = 3f;
                    currentState = EnemyState.chasing;
                }
                break;

        }



    }
}
