using UnityEngine;
using UnityEngine.Events;

public class GenericCollisions : MonoBehaviour
{
    public UnityEvent onHitGoal;
    public UnityEvent onHitHazard;

    [SerializeField] private GameObject goalExplosionPrefab;
    [SerializeField] private GameObject hazardExplosionPrefab;
    [SerializeField] private float explosionScale = 2.25f; 
    
    
    public void OnCollisionEnter(Collision collision)
    {
        HandleHazard(collision.gameObject);
        HandleGoal(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHazard(other.gameObject);
        HandleGoal(other.gameObject);
    }

    private void HandleHazard(GameObject other)
    {
        if (!other.CompareTag("hazard")) return;

        // Skip hazard collision if player is grappling
        CM_Hookshot hookshot = GetComponent<CM_Hookshot>();
        if (hookshot != null && hookshot.isGrappling)
            return;

        // If the player has a health component, deal damage instead of instant death
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (playerHealth.IsInvincible) return;
            playerHealth.TakeHazardDamage(other.transform.position);
            return;
        }

        if (hazardExplosionPrefab != null)
        {
            Instantiate(hazardExplosionPrefab, transform.position, transform.rotation);
            hazardExplosionPrefab.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
        }

        // If the player is holding/pulling this object, cancel the hookshot
        CM_Hookshot playerHookshot = FindObjectOfType<CM_Hookshot>();
        if (playerHookshot != null && playerHookshot.hitTarget == gameObject)
        {
            playerHookshot.CancelHookshot();
        }

        gameObject.SetActive(false);
        onHitHazard.Invoke();
    }

    public void TriggerHazardExplosion()
    {
        if (hazardExplosionPrefab != null)
        {
            Instantiate(hazardExplosionPrefab, transform.position, transform.rotation);
            hazardExplosionPrefab.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
        }

        CM_Hookshot playerHookshot = FindObjectOfType<CM_Hookshot>();
        if (playerHookshot != null && playerHookshot.hitTarget == gameObject)
        {
            playerHookshot.CancelHookshot();
        }

        gameObject.SetActive(false);
        onHitHazard.Invoke();
    }

    private void HandleGoal(GameObject other)
    {
        if (!other.CompareTag("goal")) return;

        // Only deactivate if this is NOT the player (Gummies get collected, player walks through)
        if (GetComponent<CM_Hookshot>() == null)
        {
            if (goalExplosionPrefab != null)
            {
                Instantiate(goalExplosionPrefab, transform.position, transform.rotation);
                goalExplosionPrefab.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
            }

            // If the player is holding/pulling this object, cancel the hookshot
            CM_Hookshot playerHookshot = FindObjectOfType<CM_Hookshot>();
            if (playerHookshot != null && playerHookshot.hitTarget == gameObject)
            {
                playerHookshot.CancelHookshot();
            }

            gameObject.SetActive(false);
        }

        onHitGoal.Invoke();
    }
}
