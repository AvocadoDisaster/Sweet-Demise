using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class HoppingEnemy : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpInterval = 2f;
    [SerializeField] private float jumpRange = 8f;
    [SerializeField] private float detectionRange = 12f;

    [Header("Contact Damage")]
    [SerializeField] private int contactDamage = 15;
    [SerializeField] private float damageCooldown = 0.5f;
    [SerializeField] private float knockbackForce = 6f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Death")]
    [SerializeField] private GameObject deathVFX;
    


    private Rigidbody2D rb;
   [SerializeField] private Transform cachedPlayer;


    private float jumpTimer;
    private float damageTimer;
    private bool isGrounded;
    private bool isDead;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Damageable d = GetComponent<Damageable>();
        d.OnDeath.AddListener(HandleDeath);

    }

    private void Start()
    {
        jumpTimer = jumpInterval;
    }

    private void Update()
    {
        if (isDead) return;


      
        CheckGrounded();

        float dist = Vector2.Distance(transform.position, cachedPlayer.position);


        if (dist <= detectionRange)
        {
            float dir = cachedPlayer.position.x - transform.position.x;
            Vector3 s = transform.localScale;
            s.x = dir >= 0f ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
            transform.localScale = s;
        }


        if (dist <= detectionRange)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f && isGrounded && dist <= jumpRange)
            {
                JumpTowardPlayer();
                jumpTimer = jumpInterval;
            }
        }


        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;
    }


    private void CheckGrounded()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


    private void JumpTowardPlayer()
    {
        if (cachedPlayer == null) return;


        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.angularVelocity = 0f;

        Vector2 direction = (cachedPlayer.position - transform.position).normalized;
        Vector2 jumpVelocity = new Vector2(direction.x * jumpForce, jumpForce * 0.8f);
        rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || damageTimer > 0f) return;


        Damageable target = collision.gameObject.GetComponent<Damageable>();
        if (target == null) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        target.TakeDamage(contactDamage);
        damageTimer = damageCooldown;

        
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 kbDir = (collision.transform.position - transform.position).normalized;
            playerRb.AddForce(kbDir * knockbackForce, ForceMode2D.Impulse);
        }
    }


    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;



        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);


        rb.velocity = Vector2.zero;
        rb.simulated = false;

        Destroy(gameObject, 0.1f);
    }
}

    
  