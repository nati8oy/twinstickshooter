using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyFollow : MonoBehaviour
{


    public Transform target;
    public float speed = 5f;

    private bool isChasing = true;

    void Update()
    {
        if (isChasing && target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(target);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "ground"){
            isChasing = false;

        }
    }
}
