using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifespan = 5f;
    public int bulletDamage;



    public void OnCollisionEnter(Collision collision)
    {
        //set the enemy layer up so the bullet knows when it collides with it.
        int enemyLayerIndex = LayerMask.NameToLayer("Enemies");
        int environmentLayerIndex = LayerMask.NameToLayer("Environment");



        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(bulletDamage);
            Debug.Log("damage taken by " + collision.gameObject.name + bulletDamage);
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }


        if (collision.gameObject.layer == environmentLayerIndex)
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }


        if (collision.gameObject.layer == enemyLayerIndex)
        {
           // collision.gameObject.GetComponent<EnemyBehaviour>().Damage(bulletDamage);
            collision.gameObject.GetComponent<EnemyAttributes>().Damage(bulletDamage);
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }


        if (collision.gameObject.tag == "player")
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "bullet")
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }


    private void OnEnable()
    {
        //sets a timer before killing the bullet
        Invoke(nameof(killBullet), lifespan);
    }
    private void killBullet()
    {
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
       // Debug.Log("bullet deleted");

    }

    private void OnDisable()
    {
        CancelInvoke();
    }

}
