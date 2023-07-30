using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_enemy : MonoBehaviour
{

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float timeProjectileLifeTime = 5f;
    [SerializeField] private float timeExplosionFadeOut = 2f;


    void OnTriggerEnter(Collider other)
    {
        // Instantiate the explosion prefab at the bullet's position
        //Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (other.tag == "Enemy") return;

        Explosion();
    }
    private void OnEnable()
    {
        // Start the coroutine when the projectile is enabled
        StartCoroutine(DisableFireballObjectAfterTime(this.gameObject, timeProjectileLifeTime, timeExplosionFadeOut));
    }

    private void OnDisable()
    {
        // Make sure to stop the coroutine when the projectile is disabled or removed
        StopAllCoroutines();
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Enemy healthComponent = collider.GetComponent<Enemy>();
                if (healthComponent != null)
                {
                    healthComponent.TakeDamage(damageAmount);
                }
            }
        }
        //StartCoroutine(DisableFireballObjectAfterTime(this.gameObject, timeExplosionFadeOut));

        PoolingManager.Instance.Pooling(this.transform.position, timeExplosionFadeOut);
        gameObject.SetActive(false);
    }

    private IEnumerator DisableFireballObjectAfterTime(GameObject objectToDisable, float timeProjectileExpire, float timeExplosionExpire)
    {
        yield return new WaitForSeconds(timeProjectileExpire);
        PoolingManager.Instance.Pooling(objectToDisable.transform.position, timeExplosionExpire);
        objectToDisable.SetActive(false);
    }
}

