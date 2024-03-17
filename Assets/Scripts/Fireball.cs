using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fireball : MonoBehaviour
{

    [SerializeField] private float explosionRadius;
    //[SerializeField] private int damageAmount = 5;
    [SerializeField] private float timeProjectileLifeTime = 5f;
    [SerializeField] private float timeExplosionFadeOut = 2f;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private LayerMask allowedLayersToCollideWith;
    [SerializeField] private float projectileSpeed;

    [HideInInspector] public int procChance;


    private void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
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
        procChance = PlayerStats.CalculateTotalChance();
        // Start the coroutine when the projectile is enabled
        StartCoroutine(DisableFireballObjectAfterTime(this.gameObject, timeProjectileLifeTime, timeExplosionFadeOut));
    }

    private void OnDisable()
    {
        procChance = PlayerStats.fireballBaseProcChance;
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
                    EventManager.Instance.EnemyTakeDamage(enemy, this.name);
                    EventManager.Instance.EnemyGetCC(enemy, this.gameObject.name);
                    //healthComponent.TakeDamage(damageAmount);
                }
            }
        }

        SpellCharge.IncreaseSpellCount(Mathf.Clamp(procChance, 0, 100));

        PoolingManagerSingleton.Instance.GetObjectFromPool("placeholder_puff", this.transform.position);

        gameObject.SetActive(false);
    }

    private IEnumerator DisableFireballObjectAfterTime(GameObject objectToDisable, float timeProjectileExpire, float timeExplosionExpire)
    {
        yield return new WaitForSeconds(timeProjectileExpire);

        PoolingManagerSingleton.Instance.GetObjectFromPool("placeholder_puff", objectToDisable.transform.position);

        objectToDisable.SetActive(false);
    }

}

