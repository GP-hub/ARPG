using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    [SerializeField] private float explosionRadius;
    [SerializeField] private float timeProjectileLifeTime;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private LayerMask allowedLayersToCollideWith;
    [SerializeField] private float projectileSpeed;

    [HideInInspector] public int procChance;

    [Space(10)]
    [Header("Overing Raycast")]
    [SerializeField] private LayerMask checkingLayer;
    [SerializeField] private float hoverHeight = 1f;
    [SerializeField] private float raycastDistance = 10f;

    private TrailRenderer[] trailRenderers;

    private void Awake()
    {
        trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
    }



    private void Update()
    {
        //transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;

        if (Physics.SphereCast(transform.position, 0.2f, Vector3.down, out RaycastHit hit, raycastDistance, checkingLayer))
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, hit.point.y + hoverHeight, 10f * Time.deltaTime);
            transform.position = pos;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") return;
        if (allowedLayersToCollideWith == (allowedLayersToCollideWith | (1 << other.gameObject.layer)))
        {
            Explosion();
        }
    }

    private void OnEnable()
    {
        ResetTrails();
        procChance = SpellCharge.CalculateTotalChance();

        // Start the coroutine when the projectile is enabled
        StartCoroutine(DisableFireballObjectAfterTime(this.gameObject, timeProjectileLifeTime));
    }

    private void OnDisable()
    {
        procChance = SpellCharge.fireballBaseProcChance;
        // Make sure to stop the coroutine when the projectile is disabled or removed
        StopAllCoroutines();
    }

    private void Explosion()
    {
        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Enemy"))
            {
                Enemy enemy = hitColliders[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    EventManager.EnemyTakeDamage(enemy, this.name);
                    EventManager.EnemyGetCC(enemy, this.gameObject.name);

                    // Increase the spell charge count based on the proc chance if we hit an enemy
                    SpellCharge.IncreaseSpellCount(Mathf.Clamp(procChance, 0, 100));
                }
            }
        }


        PoolingManagerSingleton.Instance.GetObjectFromPool("Hit_fire", this.transform.position);

        gameObject.SetActive(false);
    }

    private IEnumerator DisableFireballObjectAfterTime(GameObject objectToDisable, float timeProjectileExpire)
    {
        yield return new WaitForSeconds(timeProjectileExpire);
        Explosion();
        //PoolingManagerSingleton.Instance.GetObjectFromPool("Hit_fire", objectToDisable.transform.position);

        objectToDisable.SetActive(false);
    }
    private void ResetTrails()
    {
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.Clear();
            trail.emitting = false;
            trail.emitting = true;
        }
    }

}

