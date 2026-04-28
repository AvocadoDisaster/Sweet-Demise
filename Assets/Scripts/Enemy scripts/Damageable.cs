using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class Damageable : MonoBehaviour
{
  
    [Header("Health")]
    public int maxHealth = 100;
    
    public int startingHealth = 0;

    [Header("Invincibility Frames")]
   
    public float invincibilityDuration = 0.5f;

    [Header("Low Health")]
    
    [Range(0, 1)]
    public float lowHealthThreshold = 0.25f;

    [Header("Death Options")]
    
    public bool destroyOnDeath = false;
   
    public float destroyDelay = 0f;
   
    public GameObject deathVFX;

    [Header("Hit Flash")]
   
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

  
    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent<int> OnDamageTaken;   
    public UnityEvent<int> OnHealReceived;  
    public UnityEvent OnLowHealth;          

   
    public int  CurrentHealth    { get; private set; }
    public bool IsDead           { get; private set; }
    public bool IsInvincible     { get; private set; }
    
    public float HealthPercent => (float)CurrentHealth / maxHealth;

   
    private bool lowHealthFired;
    private Color originalColor;

    
    private void Awake()
    {
        CurrentHealth = startingHealth > 0 ? startingHealth : maxHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

   
    public void TakeDamage(int amount)
    {
        if (IsDead || IsInvincible || amount <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamageTaken?.Invoke(amount);

        
        if (spriteRenderer != null)
            StartCoroutine(FlashCoroutine());

       
        if (invincibilityDuration > 0f)
            StartCoroutine(InvincibilityCoroutine());

        
        if (!lowHealthFired && HealthPercent <= lowHealthThreshold)
        {
            lowHealthFired = true;
            OnLowHealth?.Invoke();
        }

        if (CurrentHealth <= 0)
            Die();
    }

    
    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;

        int before = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        int actual = CurrentHealth - before;

        if (actual > 0)
            OnHealReceived?.Invoke(actual);

        
        if (lowHealthFired && HealthPercent > lowHealthThreshold)
            lowHealthFired = false;
    }

    
    public void FullHeal() => Heal(maxHealth);

    
    public void InstantKill()
    {
        if (IsDead) return;
        CurrentHealth = 0;
        Die();
    }

    
    public void Revive()
    {
        IsDead        = false;
        IsInvincible  = false;
        lowHealthFired = false;
        CurrentHealth = maxHealth;
    }

    
    public void SetInvincible(bool value) => IsInvincible = value;

    
    private void Die()
    {
        if (IsDead) return;   
        IsDead = true;

        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        OnDeath?.Invoke();

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        IsInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        IsInvincible = false;
    }

    private IEnumerator FlashCoroutine()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

   
  
}
