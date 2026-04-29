using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class BossEnemy : MonoBehaviour
{
    [Header("Boss Movement")]
    
    public float flySpeed = 2f;
   
    public float patrolRadius = 4f;
    
    public float preferredHeight = 6f;

    [Header("Nodes")]
    
    public List<BossNode> nodes = new List<BossNode>();

    [Header("Death")]
   
    public GameObject deathVFX;
   


   
    private Rigidbody2D rb;
    private Vector2 startPos;
    private float timeAlive;
    private bool isDead;

  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;              
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        startPos = transform.position;

       
        foreach (BossNode node in nodes)
        {
            if (node != null)
                node.OnNodeDestroyed += HandleNodeDestroyed;
        }

       
        GetComponent<Damageable>().OnDeath.AddListener(Die);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        Hover();
    }

   
    private void Hover()
    {
        timeAlive += Time.fixedDeltaTime;

        float x = startPos.x + Mathf.Sin(timeAlive * flySpeed) * patrolRadius;
        float y = preferredHeight + Mathf.Sin(timeAlive * flySpeed * 0.5f) * 1.5f;

        Vector2 target = new Vector2(x, y);
        rb.MovePosition(Vector2.Lerp(rb.position, target, Time.fixedDeltaTime * flySpeed));
    }

    
    private void HandleNodeDestroyed(BossNode destroyedNode)
    {
        nodes.Remove(destroyedNode);

        if (nodes.Count == 0)
        {
            Die();
        }
    }

    
    public void Die()
    {
        if (isDead) return;
        isDead = true;

       
     

        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        
        foreach (BossNode node in nodes)
        {
            if (node != null) Destroy(node.gameObject);
        }

        Destroy(gameObject);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPos : (Vector2)transform.position,
                              patrolRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,
                        transform.position + Vector3.up * preferredHeight);
    }
}
