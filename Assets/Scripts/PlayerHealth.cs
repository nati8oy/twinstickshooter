using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private MMHealthBar healthBar;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float hazardDamage = 25f;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;
    private float invincibilityTimer;
    private float blinkTimer;
    private Renderer[] renderers;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float knockbackDrag = 8f;
    [HideInInspector] public Vector3 knockbackVelocity;

    [Header("Events")]
    [SerializeField] private UnityEvent onDamage;
    [SerializeField] private UnityEvent onDeath;

    public bool IsInvincible => invincibilityTimer > 0f;

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        invincibilityTimer = 0f;
        if (renderers != null) SetRenderersVisible(true);
        UpdateHealthBar();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        renderers = GetComponentsInChildren<Renderer>();
        UpdateHealthBar();
    }

    private void Update()
    {
        // Invincibility timer and blink
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0f)
            {
                ToggleRenderers();
                blinkTimer = blinkInterval;
            }

            if (invincibilityTimer <= 0f)
            {
                SetRenderersVisible(true);
            }
        }

        // Decay knockback
        if (knockbackVelocity.sqrMagnitude > 0.01f)
        {
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDrag * Time.deltaTime);
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }
    }

    public void TakeHazardDamage(Vector3 hazardPosition)
    {
        if (IsInvincible) return;

        currentHealth -= hazardDamage;
        if (currentHealth < 0f)
            currentHealth = 0f;

        UpdateHealthBar();

        // Knockback away from hazard
        Vector3 knockDir = (transform.position - hazardPosition).normalized;
        knockDir.y = 0f;
        knockbackVelocity = knockDir * knockbackForce;

        // Start invincibility
        invincibilityTimer = invincibilityDuration;
        blinkTimer = blinkInterval;

        onDamage.Invoke();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        onDeath.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.UpdateBar(currentHealth, 0f, maxHealth, true);
    }

    private void ToggleRenderers()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = !renderers[i].enabled;
        }
    }

    private void SetRenderersVisible(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = visible;
        }
    }
}
