using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains;
using MoreMountains.Tools;

public class ThrowDamage : MonoBehaviour
{
    [SerializeField] private UnityEvent onHit;
    [SerializeField] private float damageThreshold = 10f; // Minimum collision velocity required to trigger damage
    [SerializeField] private float throwDamage; 


    [Header("Health Bar")]
    [SerializeField] private MMHealthBar healthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float minHealth = 0f;
    //[SerializeField] private EnemyHealth enemyHealth;


    private Rigidbody rb;
 
    void Start()
    {

        currentHealth = maxHealth;  
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Get the relative velocity of the collision
        float collisionVelocity = collision.relativeVelocity.magnitude;

        if (collisionVelocity >= damageThreshold)
        {
            Damage(throwDamage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
            
        }

    }


    public void Damage(float damageAmount)
    {
        // Perform damage logic here

        onHit.Invoke();


        if (currentHealth > minHealth)
        {
            currentHealth = currentHealth - damageAmount;
            healthBar.UpdateBar(currentHealth, minHealth, maxHealth, true);
        }
        else
        {
            currentHealth = minHealth;
        }

            
    }
}
