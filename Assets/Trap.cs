using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public int DPS = 1;
    private bool hasAttacked;

    private bool isTriggerActive = false;
    private float timeSinceLastAttack;
    public float attackCooldown;
    public LayerMask enemyLayer;


    public bool knockback = true;
    public float knockbackRadius = 1f;
    public float knockbackForce = 500f;

    private Collider enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DealDamage", 0, attackCooldown);
    }
    // Update is called once per frame
    void Update()
    {
        if (hasAttacked)
        {
            timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack >= attackCooldown)
            {
                hasAttacked = false;
            }
        }
        
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyCollider = other;
        enemyCollider.gameObject.GetComponent<EnemyBehaviour>().Damage(DPS);
        if (knockback)
        {
            enemyCollider.GetComponent<Rigidbody>().AddExplosionForce(1000f, transform.position, 2f);
            Debug.Log("knockback applied");

        }
    }

    void OnTriggerStay(Collider other)
    {
        isTriggerActive = true;
    }

    void OnTriggerExit(Collider other)
    {
        isTriggerActive = false;
    }


    void DealDamage()
    {
        if (isTriggerActive)
        {
            if (enemyLayer == (enemyLayer | (1 << enemyCollider.gameObject.layer)))
            {
                enemyCollider.gameObject.GetComponent<EnemyBehaviour>().Damage(DPS);
                if (knockback)
                {
                    enemyCollider.GetComponent<Rigidbody>().AddExplosionForce(1000f, transform.position, 2f);
                    Debug.Log("knockback applied");

                }



                if (!hasAttacked)
                {
                    Debug.Log("attacked");
                    hasAttacked = true;
                    timeSinceLastAttack = 0f;
                }
            }

        }

        /*

        private void OnTriggerEnter(Collider collision)
        {
            //

            if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)))
            {
                collision.gameObject.GetComponent<EnemyBehaviour>().Damage(DPS);

                if (!hasAttacked)
                {
                    Debug.Log("attacked");
                    hasAttacked = true;
                    timeSinceLastAttack = 0f;
                }
            }

        }*/
    }
 }
