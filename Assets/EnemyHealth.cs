using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains;
using MoreMountains.Tools;



public class EnemyHealth : MonoBehaviour
{

    [SerializeField] private MMHealthBar healthBar;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float minHealth = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.UpdateBar(currentHealth, minHealth, maxHealth,true);
    }
}
