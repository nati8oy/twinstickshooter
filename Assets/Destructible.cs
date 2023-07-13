using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour, IDamagable
{
    public int HPTotal = 2;
    public int currentHP;
    private bool itemDestroyed;

    [SerializeField] private UnityEvent onDestruction;

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

            //on destructible items if there is a loot bag, drop the loot! 
            if (gameObject.GetComponent<LootBag>())
            {
                gameObject.GetComponent<LootBag>().DropLoot();
            }
        }
    }
}
