using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Damageable))]
public class BossNode : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    
    public float fireRate = 2f;
   
    public float projectileSpeed = 6f;
   
    public int projectileDamage = 10;
    
    public string playerTag = "Player";

    [Header("Visuals")]
    public GameObject destroyVFX;


    public event Action<BossNode> OnNodeDestroyed;

    private float fireTimer;


    private void Awake()
    {
        GetComponent<Damageable>().OnDeath.AddListener(HandleDeath);
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootAtNearestPlayer();
        }
    }

  
    private void ShootAtNearestPlayer()
    {
        if (projectilePrefab == null) return;

        GameObject target = FindNearestPlayer();
        if (target == null) return;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(projectilePrefab, transform.position,
                                      Quaternion.Euler(0, 0, angle));

       
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
            projRb.velocity = direction * projectileSpeed;

        
        BossProjectile bossProj = proj.GetComponent<BossProjectile>();
        if (bossProj != null)
            bossProj.damage = projectileDamage;
    }

    private GameObject FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        if (players.Length == 0) return null;

        GameObject nearest = null;
        float bestDist = float.MaxValue;
        foreach (GameObject p in players)
        {
            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = p;
            }
        }
        return nearest;
    }

  
    private void HandleDeath()
    {
        if (destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);

        OnNodeDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}
