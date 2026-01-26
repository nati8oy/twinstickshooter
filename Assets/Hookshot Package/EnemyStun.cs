using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyStun : MonoBehaviour
{
    [SerializeField] private UnityEvent onStun;
    [SerializeField] private UnityEvent onRecover;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public void Stun(float stunTime)
    {
        StartCoroutine(StunCoroutine(stunTime));

        if (gameObject.activeSelf)
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            onStun.Invoke();  
        }

        



    }

    private IEnumerator StunCoroutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        onRecover.Invoke();
       
    }
}
