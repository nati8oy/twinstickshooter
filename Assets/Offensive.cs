using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offensive : MonoBehaviour
{

    private GameObject currentTarget;
    private GameObject[] targets;

    public float attackRange = 15f;

    // Update is called once per frame
    void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("enemy");

        foreach(GameObject targetEnemy in targets)
        {
            if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < attackRange)
            {
                Debug.DrawLine(transform.position, targetEnemy.transform.position, color: Color.red);
                Debug.Log("target acquired");
            }
        }

    }
}
