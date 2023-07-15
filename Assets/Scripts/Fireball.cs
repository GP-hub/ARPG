using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float timeExplosionFadeOut = 2f;


    void OnTriggerEnter(Collider other)
    {
        // Instantiate the explosion prefab at the bullet's position
        //Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Explosion();
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy healthComponent = collider.GetComponent<Enemy>();
                if (healthComponent != null)
                {
                    healthComponent.TakeDamage(damageAmount);
                }
            }
        }

        PoolingManager.Instance.Pooling(this.transform.position, timeExplosionFadeOut);
        gameObject.SetActive(false);
    }
}
