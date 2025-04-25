using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Enemy))]
public class EnemyTargetDistanceMonitor : MonoBehaviour
{
    [SerializeField] private float targetForgetDistance;
    [SerializeField] private float maxTimeTargetTooFar;
    [SerializeField] private float checkInterval;

    private Enemy enemy;
    private float timeTargetTooFar = 0f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        StartCoroutine(MonitorTargetDistance());
    }

    private IEnumerator MonitorTargetDistance()
    {
        WaitForSeconds delay = new WaitForSeconds(checkInterval);

        while (true)
        {
            if (enemy.Target != null)
            {
                float sqrDistance = (enemy.Target.position - transform.position).sqrMagnitude;

                if (sqrDistance > targetForgetDistance)
                {
                    timeTargetTooFar += checkInterval;
                    if (timeTargetTooFar >= maxTimeTargetTooFar)
                    {
                        enemy.Target = null;
                        timeTargetTooFar = 0f;
                    }
                }
                else
                {
                    timeTargetTooFar = 0f;
                }
            }
            else
            {
                timeTargetTooFar = 0f;
            }

            yield return delay;
        }
    }
}
