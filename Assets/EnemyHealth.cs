using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains;
using MoreMountains.Tools;
using UnityEngine.Events;


public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private UnityEvent onHit;


    [SerializeField] private MMHealthBar healthBar;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float minHealth = 0f;


    // Update is called once per frame
    void Update()
    {
        healthBar.UpdateBar(currentHealth, minHealth, maxHealth, true);

        if (currentHealth <= 0)
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
