using UnityEngine;

public class FireGroundDoT : MonoBehaviour
{
    public float radius = 2.5f;
    public float tickRate = 0.5f;
    public LayerMask characterLayer;

    private float timer = 0f;

    // Max number of colliders to check each tick
    private const int maxHits = 10;
    private Collider[] hits = new Collider[maxHits];

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tickRate)
        {
            timer = 0f;
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, hits, characterLayer);

            for (int i = 0; i < hitCount; i++)
            {
                GameObject target = hits[i].gameObject;

                if (target.CompareTag("Enemy"))
                {
                    Enemy enemy = target.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        EventManager.EnemyTakeDamage(enemy, gameObject.name);
                    }
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
