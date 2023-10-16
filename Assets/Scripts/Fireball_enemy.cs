using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_enemy : MonoBehaviour
{

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float timeProjectileLifeTime = 5f;
    [SerializeField] private float timeExplosionFadeOut = 2f;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private float projectileSpeed;


    private void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
    }


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
        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {

                Debug.Log("DAMAGE PLAYER");
                //Enemy healthComponent = hitColliders[i].GetComponent<Enemy>();
                //if (healthComponent != null)
                //{
                //    healthComponent.TakeDamage(damageAmount);
                //}
            }
        }

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

