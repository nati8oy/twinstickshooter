using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    public float lifespan = 5f;


    private void OnEnable()
    {
        //sets a timer before killing the bullet
        Invoke(nameof(killBullet), lifespan);
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        // GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("bullet");


        if (collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "enemy")
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag=="enemy bullet")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }

        if (collision.gameObject.tag == "bullet")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }

    private void killBullet()
    {
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
        // Debug.Log("bullet deleted");

    }


}
