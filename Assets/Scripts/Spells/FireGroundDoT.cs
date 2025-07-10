using UnityEngine;
using System.Collections.Generic;


public enum DoTDetectionMode
{
    PhysicsOverlap,
    ColliderTrigger
}

public class FireGroundDoT : MonoBehaviour
{
    public DoTDetectionMode detectionMode = DoTDetectionMode.PhysicsOverlap;
    public float radius = 2.5f;
    public float tickRate = 0.5f;
    public LayerMask characterLayer;

    private float timer = 0f;
    private const int maxHits = 10;
    private Collider[] hits = new Collider[maxHits];

    private Firewall.FirewallAttackContext context;

    // For trigger-based detection
    private readonly HashSet<Collider> triggerColliders = new HashSet<Collider>();

    public void SetContext(Firewall.FirewallAttackContext ctx)
    {
        context = ctx;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < tickRate)
            return;

        timer = 0f;

        if (context != null && Time.time - context.lastTickTime >= tickRate)
        {
            context.tickHitEnemies.Clear();
            context.lastTickTime = Time.time;
        }

        if (detectionMode == DoTDetectionMode.PhysicsOverlap)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, hits, characterLayer);
            for (int i = 0; i < hitCount; i++)
                TryDealDamage(hits[i]);
        }
        else if (detectionMode == DoTDetectionMode.ColliderTrigger)
        {
            foreach (var col in triggerColliders)
                TryDealDamage(col);
        }
    }

    private void TryDealDamage(Collider collider)
    {
        if (!collider.CompareTag("Enemy"))
            return;

        if (context != null && context.tickHitEnemies.Contains(collider))
            return;

        Enemy enemy = collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            context?.tickHitEnemies.Add(collider);
            EventManager.EnemyTakeDamage(enemy, gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (detectionMode != DoTDetectionMode.ColliderTrigger) return;
        if ((characterLayer.value & (1 << other.gameObject.layer)) != 0)
        triggerColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectionMode != DoTDetectionMode.ColliderTrigger) return;
        triggerColliders.Remove(other);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionMode == DoTDetectionMode.PhysicsOverlap)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
