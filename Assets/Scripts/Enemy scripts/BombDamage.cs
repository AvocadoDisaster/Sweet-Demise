using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDamage : MonoBehaviour
{
    public int damage = 25;
    public float blastRadius = 2.5f;
    public float fuseTime = 1.5f;
    public string targetTag = "Enemy";
    public GameObject explosionVFX;

    private void Start() => StartCoroutine(Fuse());

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(targetTag)) Explode();
    }

    public IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    public void Explode()
    {
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);


        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(targetTag)) continue;
            Damageable d = hit.GetComponent<Damageable>();
            if (d != null) d.TakeDamage(damage);


            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * 10f, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.3f, 0, 0.4f);
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}