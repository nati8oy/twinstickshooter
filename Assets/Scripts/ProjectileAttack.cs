using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour, IAttack
{
    public Transform firePoint;
    private string objectPoolerTag;
    private float cooldown = 3f;

    public Weapon weapon;

    private int damage;
    private float attackRate;


    private int shotSpeed = 40;

    private void Start()
    {
        //cooldown = weapon.cooldown;
        damage = weapon.damage;
        shotSpeed = weapon.shotSpeed;
        attackRate = weapon.attackRate;
        objectPoolerTag = weapon.ammoType;
    }

    //regular attack
    public void Attack()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject(objectPoolerTag);


       

        //if the attacking object is the player then do this.
        //just manages player ammo whereas enemies don't have ammo.

        if(gameObject.tag == "Player")
        {
            //check if the debug infinite ammo is on or not.
            if (AmmoManager.ammoPrimary > 0)
            {
                if (bullet != null)
                {
                    bullet.transform.position = firePoint.position;
                    bullet.transform.rotation = firePoint.transform.rotation;
                    bullet.SetActive(true);
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.AddForce(firePoint.forward * shotSpeed, ForceMode.Impulse);
                }
            }

            if(DebugManager.Instance.infiniteAmmo != true)
            {
                //reduce ammo amount if infinite ammo is not on in Debug Manager
                AmmoManager.ammoPrimary -= 1;

            }

        }

        //otherwise if it's any other object that is attacking that isn't the player then just do this part

        else if (bullet != null)
        {
            
                bullet.transform.position = firePoint.position;
                bullet.SetActive(true);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(firePoint.forward * shotSpeed, ForceMode.Impulse);
            
        }


        //play feedbacks for shooting
        if (GetComponent<PlayFeedbacksGeneric>())
        {
            GetComponent<PlayFeedbacksGeneric>().ShootFeedback();
        }

    }

    //timed attack set by using nextAttackTime as a cooldown
    public void TimedAttack()
    {
        if (Time.time > cooldown)
        {
            gameObject.GetComponent<IAttack>().Attack();
             cooldown = Time.time + attackRate;
        }
    }
    
}
