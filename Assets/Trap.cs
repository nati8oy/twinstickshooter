using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public int DPS = 1;
    private bool hasAttacked;
    private float timeSinceLastAttack;
    public float attackCooldown;
    public LayerMask enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        
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

   
    private void OnTriggerStay(Collider collision)
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

     
    }


}
