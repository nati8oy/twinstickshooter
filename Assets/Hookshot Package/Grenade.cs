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
    [SerializeField] private LayerMask affectedLayers = ~0;
    [SerializeField] private float explosionSize = 2f;

    private void OnEnable()
    {
        hasExploded = false;
    }

    private bool IsBeingCarried()
    {
        CM_Hookshot hookshot = FindObjectOfType<CM_Hookshot>();
        return hookshot != null && hookshot.hitTarget == gameObject && hookshot.state == CM_Hookshot.State.HookshotCarry;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Don't explode while the player is carrying this object
        if (IsBeingCarried()) return;

        // Check if the collided object's layer is in our affected layers mask
        if ((affectedLayers & (1 << collision.gameObject.layer)) == 0) return;

        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage(damageInflicted);
        }

        // Destroy the directly hit object if it's tagged Destructible
        if (collision.gameObject.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
        }

        Explode();
    }


    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, affectedLayers);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.gameObject == gameObject) continue;

            // Disable NavMeshAgent so physics force actually applies
            UnityEngine.AI.NavMeshAgent agent = nearbyObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.enabled)
            {
                agent.enabled = false;
            }

            // Apply damage to anything damageable
            EnemyHealth enemyHealth = nearbyObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.Damage(damageInflicted);
            }

            // Apply explosion force to anything with a rigidbody
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }

            // Destroy gummies caught in the blast and trigger their explosion effect
            GummyBehaviour gummy = nearbyObject.GetComponent<GummyBehaviour>();
            if (gummy != null)
            {
                GenericCollisions gc = nearbyObject.GetComponent<GenericCollisions>();
                if (gc != null)
                {
                    gc.TriggerHazardExplosion();
                }
                else
                {
                    nearbyObject.gameObject.SetActive(false);
                }
            }

            // Destroy any object tagged as Destructible
            if (nearbyObject.CompareTag("Destructible"))
            {
                Destroy(nearbyObject.gameObject);
            }
        }

        if (onExplode != null)
        {
            onExplode.Invoke();
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            explosionPrefab.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
        }

        // If the player is holding/pulling this object, cancel the hookshot
        CM_Hookshot hookshot = FindObjectOfType<CM_Hookshot>();
        if (hookshot != null && hookshot.hitTarget == gameObject)
        {
            hookshot.CancelHookshot();
        }

        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }


}
