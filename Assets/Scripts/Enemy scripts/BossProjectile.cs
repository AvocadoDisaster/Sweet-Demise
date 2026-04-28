using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BossProjectile : MonoBehaviour
{
    [Tooltip("Damage dealt on hit (overwritten at runtime by BossNode)")]
    public int damage = 10;
    [Tooltip("Seconds before auto-destroy")]
    public float lifetime = 5f;
    [Tooltip("Tag of objects this projectile can damage")]
    public string targetTag = "Player";
    public GameObject hitVFX;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        
        GetComponent<Collider2D>().isTrigger = true;
        
        GetComponent<Rigidbody2D>().gravityScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
            damageable.TakeDamage(damage);

        if (hitVFX != null)
            Instantiate(hitVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
