using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    private string objectPoolerTag;

    public Weapon weapon;


    private int damage;
    private float attackRate;

    private int shotSpeed = 40;

    private void Start()
    {
        damage = weapon.damage;
        shotSpeed = weapon.shotSpeed;
        attackRate = weapon.attackRate;
        objectPoolerTag = weapon.ammoType;
    }

    public void Shoot()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject(objectPoolerTag);


        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.SetActive(true);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * shotSpeed, ForceMode.Impulse);
        }

    }

    public void ShootHoming()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject(objectPoolerTag);


        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.SetActive(true);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * shotSpeed, ForceMode.Impulse);
        }
    }


             
}
