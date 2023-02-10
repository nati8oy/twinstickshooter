using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamagable
{
    private float HPTotal = 2f;
    private float currentHP;
    private bool itemDestroyed;

    private void OnEnable()
    {
        currentHP = HPTotal;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(1);
        }
    }

    public void Damage(int damageAmount)
    {
        //Debug.Log("damagable implemented!");
        //float damage = 1;
        currentHP -= damageAmount;
        //healthBar.SetAmount(currentHP);
        if (currentHP <= 0)
        {
            itemDestroyed = true;
            gameObject.SetActive(false);
        }
    }
}
