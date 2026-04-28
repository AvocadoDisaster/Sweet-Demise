using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHitbox : MonoBehaviour
{
    public string targetTag = "Enemy";


    public float knockbackForce = 8f;


    [HideInInspector] public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;


        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
            damageable.TakeDamage(Mathf.RoundToInt(damage));


        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 kbDir = (other.transform.position - transform.position).normalized;
            rb.AddForce(kbDir * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
