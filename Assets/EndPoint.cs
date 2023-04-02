using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public HealthBar healthBar;

    private int maxHealth;
    //private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        
        maxHealth = GetComponent<Destructible>().HPTotal;
        GetComponent<Destructible>().currentHP = maxHealth;
        healthBar.SetMaxBarAmount(maxHealth);

    }

    private void Update()
    {
        healthBar.SetAmount(GetComponent<Destructible>().currentHP);
    }


}
