using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class Grenade : MonoBehaviour
{

    public float delay = 2f;
    private float countdown;
    private bool hasExploded;
    public float blastRadius = 5f;
    public float explosionForce = 500f;
    public int damageInflicted = 3;

    //public MMF_Player explosionFeedback;

    public UnityEvent onExplode;

    //public GameObject explosionEffect;
    private void OnEnable()
    {
        // initializes the player and all its feedbacks, making sure everything's correctly setup before playing it
        //explosionFeedback.Initialization();
    }

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
           // Debug.Log("Boom! Timed out!");
        }
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

        GameObject boom = ObjectPooler.SharedInstance.GetPooledObject("explosion");

        if (boom != null)
        {
            boom.transform.position = transform.position;
            boom.SetActive(true);

            if (onExplode != null)
            {
                onExplode.Invoke();
            }
           // explosionFeedback?.PlayFeedbacks();
            hasExploded = true;
           // Debug.Log("Boom! HeadSHOT!");
            // asks the player to play its sequence of feedbacks

        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
//        Debug.Log(colliders.Length);
        foreach (Collider nearbyObject in colliders)
        {          

            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            EnemyBehaviour enemyBehaviour = nearbyObject.GetComponent<EnemyBehaviour>();

            if (rb != null && enemyBehaviour != null)
            {
                enemyBehaviour.Damage(damageInflicted);
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
                //Debug.Log("enemy destroyed");
            }

        }
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        gameObject.SetActive(false);

    }


}
