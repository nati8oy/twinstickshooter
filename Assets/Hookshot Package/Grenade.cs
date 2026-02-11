using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class Grenade : MonoBehaviour
{

    private bool hasExploded;
    public float blastRadius = 5f;
    public float explosionForce = 500f;
    public int damageInflicted = 3;

    //public MMF_Player explosionFeedback;

    public UnityEvent onExplode;

    [SerializeField] private GameObject explosionPrefab;

    private void OnEnable()
    {
        hasExploded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        //set the enemy layer up so the bullet knows when it collides with it.
        int enemyLayerIndex = LayerMask.NameToLayer("Enemies");
        int environmentLayerIndex = LayerMask.NameToLayer("Environment");

        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(damageInflicted);
            Explode();

        }

        //check the layers and see if they are the enemy or environment layers
        if (collision.gameObject.layer == environmentLayerIndex  || collision.gameObject.layer == enemyLayerIndex)
        {
            Explode();
            hasExploded = true;
        }

    }


    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            EnemyBehaviour enemyBehaviour = nearbyObject.GetComponent<EnemyBehaviour>();

            if (rb != null && enemyBehaviour != null)
            {
                enemyBehaviour.Damage(damageInflicted);
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }
        }

        if (onExplode != null)
        {
            onExplode.Invoke();
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }


}
