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
        anchor.transform.position = worldPosition;
        anchor.SetActive(true);

        DamageNumber dmg = anchor.GetComponentInChildren<DamageNumber>();
        dmg.Initialize(text, color);
    }

    private GameObject GetFromPool()
    {
        if (pool.Count == 0)
        {
            GameObject newOne = Instantiate(damageNumberAnchorPrefab, transform);
            newOne.SetActive(false);
            pool.Enqueue(newOne);
        }

        return pool.Dequeue();
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
