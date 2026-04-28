using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damageable))]
public class BullRushEnemy : MonoBehaviour
{
    
    [Header("Detection")]
    
    public float detectionRadius = 8f;
 
    public string playerTag = "Player";

    [Header("Rush")]
    
    public float rushSpeed = 14f;
   
    public float rushDuration = 0.6f;
    
    public float cooldown = 2f;
    
    public int contactDamage = 20;

    [Header("Idle Movement")]
   
    public float wanderSpeed = 1.5f;

    [Header("Death")]
    public GameObject deathVFX;
   


    private Rigidbody2D rb;
    private enum State { Idle, WindUp, Rushing, Cooldown }
    private State state = State.Idle;
    private Vector2 rushDirection;
    private float stateTimer;
    private Vector2 wanderTarget;

 
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;           
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Damageable>().OnDeath.AddListener(Die);
        PickNewWanderTarget();
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Idle:      UpdateIdle();    break;
            case State.WindUp:    UpdateWindUp();  break;
            case State.Rushing:   UpdateRushing(); break;
            case State.Cooldown:  UpdateCooldown();break;
        }
    }

    private void FixedUpdate()
    {
        if (state == State.Rushing)
            rb.velocity = rushDirection * rushSpeed;
        else if (state == State.Idle)
            WanderTowards(wanderTarget);
        else
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * 6f);
    }

   
    private void UpdateIdle()
    {
       
        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
            PickNewWanderTarget();

        GameObject target = FindNearestPlayerInRange();
        if (target != null)
        {
            rushDirection = (target.transform.position - transform.position).normalized;
            state = State.WindUp;
            stateTimer = 0.5f;   
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateWindUp()
    {
       
        FaceDirection(rushDirection);

        if (stateTimer <= 0f)
        {
            state = State.Rushing;
            stateTimer = rushDuration;
        }
    }

    private void UpdateRushing()
    {
        if (stateTimer <= 0f)
        {
            state = State.Cooldown;
            stateTimer = cooldown;
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateCooldown()
    {
        if (stateTimer <= 0f)
        {
            state = State.Idle;
            PickNewWanderTarget();
        }
    }

    
    private GameObject FindNearestPlayerInRange()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject nearest = null;
        float best = detectionRadius;

        foreach (GameObject p in players)
        {
            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < best) { best = d; nearest = p; }
        }
        return nearest;
    }

    private void WanderTowards(Vector2 target)
    {
        Vector2 dir = (target - rb.position).normalized;
        rb.velocity = Vector2.Lerp(rb.velocity, dir * wanderSpeed, Time.fixedDeltaTime * 3f);
        FaceDirection(dir);
    }

    private void PickNewWanderTarget()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * 4f;
    }

    private void FaceDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = dir.x > 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
            transform.localScale = s;
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != State.Rushing) return;
        if (!collision.gameObject.CompareTag(playerTag)) return;

        Damageable dmg = collision.gameObject.GetComponent<Damageable>();
        if (dmg != null) dmg.TakeDamage(contactDamage);

        rb.velocity = -rushDirection * (rushSpeed * 0.3f);
        state = State.Cooldown;
        stateTimer = cooldown;
    }

    
    private void Die()
    {
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        if (state == State.Rushing || state == State.WindUp)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, rushDirection * 3f);
        }
    }
}
