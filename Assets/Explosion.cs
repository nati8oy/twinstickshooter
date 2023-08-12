using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : MonoBehaviour
{

    [SerializeField] private float radius = 10.0F;
    [SerializeField] private float power = 10.0F;
    [SerializeField] private GameObject explosion;

    [SerializeField] LayerMask layerMask;


    public void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.tag == "enemy")
        {
            Explode();
        }

    }

    private void Explode()
    {

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 100.0F);
            //Debug.Log("no. of object in explosion: " +  colliders.Length);

        }

        Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);


        gameObject.SetActive(false);
    }


}
