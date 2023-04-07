using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifespan = 5f;
    public int bulletDamage;

    public LayerMask environmentCollisionLayer;


    public void OnCollisionEnter(Collision collision)
    {


        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(bulletDamage);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
        
        
        if(collision.gameObject.tag == "enemy")
        {
            if (collision.gameObject.GetComponent<EnemyBehaviour>()!=null)
            {
                collision.gameObject.GetComponent<EnemyBehaviour>().Damage(bulletDamage);
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.SetActive(false);
            }
            
        }



        if (environmentCollisionLayer == (environmentCollisionLayer | (1 << collision.gameObject.layer)))
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "player")
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "bullet")
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        /*
        if (collision.gameObject.tag != null)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }*/
    }


    private void OnEnable()
    {
        //sets a timer before killing the bullet
        Invoke(nameof(killBullet), lifespan);
    }
    private void killBullet()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
       // Debug.Log("bullet deleted");

    }

    private void OnDisable()
    {
        CancelInvoke();
    }

}
