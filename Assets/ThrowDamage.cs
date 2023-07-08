using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrowDamage : MonoBehaviour
{
    [SerializeField] private UnityEvent onHit;
    [SerializeField] private float damageThreshold = 10f; // Minimum collision velocity required to trigger damage

    private Rigidbody rb;
    // Start is called before the first frame update


    private void Damage()
    {
        // Perform damage logic here

        onHit.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Get the relative velocity of the collision
        float collisionVelocity = collision.relativeVelocity.magnitude;

        if (collisionVelocity >= damageThreshold)
        {
            Damage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
