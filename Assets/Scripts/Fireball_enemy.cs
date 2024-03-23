using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityValues))]
public class Fireball_enemy : MonoBehaviour
{

    [SerializeField] private float explosionRadius = 5f;
    //[SerializeField] private int damageAmount = 5;
    [SerializeField] private float timeProjectileLifeTime = 5f;
    [SerializeField] private float timeExplosionFadeOut = 2f;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private AbilityValues abilityValues;

    private List<GameObject> playersToDamage = new List<GameObject>();

    //public int DamageAmount { get => damageAmount; set => damageAmount = value; }

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
                //EventManager.PlayerTakeDamage(damageAmount);
                abilityValues.playersToDamage.Add(hitColliders[i].gameObject);
                abilityValues.DoDamage(abilityValues.Damage);
            }
        }

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

