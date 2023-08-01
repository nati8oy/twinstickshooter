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
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float minHealth = 0f;

    // Start is called before the first frame update


    private Rigidbody rb;
    // Start is called before the first frame update


    private void Damage(float damageAmount)
    {
        // Perform damage logic here

        onHit.Invoke();


        if (currentHealth> minHealth)
        {
            currentHealth = currentHealth - damageAmount;
            healthBar.UpdateBar(currentHealth, minHealth, maxHealth, true);
        }
        else
        {
            currentHealth = minHealth;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
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
        
    }
}
