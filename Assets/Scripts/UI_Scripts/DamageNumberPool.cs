using System.Collections.Generic;
using UnityEngine;

public class DamageNumberPool : MonoBehaviour
{
    public static DamageNumberPool Instance;

    [SerializeField] private GameObject damageNumberAnchorPrefab; // Prefab with empty GameObject + DamageNumber child inside
    [SerializeField] private int initialPoolSize = 20;

    private Queue<GameObject> pool = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject instance = Instantiate(damageNumberAnchorPrefab, transform);
            instance.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public void ShowDamage(Vector3 worldPosition, string text, Color color)
    {
        GameObject anchor = GetFromPool();
        anchor.SetActive(true);

        // Random offset to avoid overlapping
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.3f, 0.3f),  // X jitter
            Random.Range(0.4f, 0.6f),   // Slight upward offset
            Random.Range(-0.3f, 0.3f)   // Z jitter (optional)
        );

        // Pass position and direction to the anchor
        DamageNumberAnchor anchorScript = anchor.GetComponent<DamageNumberAnchor>();
        anchorScript.Initialize(worldPosition + randomOffset);

        DamageNumber dmg = anchor.GetComponentInChildren<DamageNumber>();
        dmg.Initialize(text, color);
    }


    private GameObject GetFromPool()
    {
        // Search for an inactive object in the pool
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (!obj.activeInHierarchy)
                return obj;
        }

        // If all objects are in use, instantiate a new one
        GameObject newOne = Instantiate(damageNumberAnchorPrefab, transform);
        newOne.SetActive(false);
        return newOne;
    }


    public void ReturnToPool(GameObject anchor)
    {
        anchor.SetActive(false);

        // Reset local position of child DamageNumber to zero to avoid accumulated offset
        DamageNumber dmg = anchor.GetComponentInChildren<DamageNumber>();
        dmg.ResetLocalPosition();

        pool.Enqueue(anchor);
    }
}
