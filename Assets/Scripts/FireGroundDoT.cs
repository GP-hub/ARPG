using UnityEngine;
using System.Collections.Generic;

public class FireGroundDoT : MonoBehaviour
{
    public float radius = 2.5f;
    public float tickRate = 0.5f;
    public LayerMask characterLayer;

    private float timer = 0f;
    private const int maxHits = 10;
    private Collider[] hits = new Collider[maxHits];

    private Firewall.FirewallAttackContext context;

    public void SetContext(Firewall.FirewallAttackContext ctx)
    {
        context = ctx;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tickRate)
        {
            timer = 0f;

            // Reset the tickHitEnemies *only once per tick across all segments*
            if (context != null)
            {
                if (Time.time - context.lastTickTime >= tickRate)
                {
                    context.tickHitEnemies.Clear();
                    context.lastTickTime = Time.time;
                }
            }

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, hits, characterLayer);

            for (int i = 0; i < hitCount; i++)
            {
                GameObject target = hits[i].gameObject;
                Collider collider = hits[i];

                if (!target.CompareTag("Enemy"))
                    continue;

                if (context != null && context.tickHitEnemies.Contains(collider))
                    continue;

                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    context?.tickHitEnemies.Add(collider);
                    EventManager.EnemyTakeDamage(enemy, gameObject.name);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
