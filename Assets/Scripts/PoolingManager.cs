using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance { get; private set; }

    [SerializeField] private GameObject player;

    private GameObject prefab;
    private int initialPoolSize = 10;

    private List<GameObject> objectPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (player!=null)
        {
            prefab = player.transform.GetComponent<AttackAndPowerCasting>().fireballExplosionPrefab;
        }

        objectPool = new List<GameObject>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Add(obj);
        }

    }

    public void Pooling(Vector3 position, float timeBeforeDisabling)
    {
        GameObject obj = GetPooledObject();
        if (obj != null)
        {
            obj.transform.position = position;
            obj.SetActive(true);
            StartCoroutine(DisablePowerObjectAfterTime(obj, timeBeforeDisabling));
        }
    }

    private IEnumerator DisablePowerObjectAfterTime(GameObject objectToDisable, float time)
    {
        yield return new WaitForSeconds(time);
        objectToDisable.SetActive(false);
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        // If no inactive object is found, create a new one and add it to the pool
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        objectPool.Add(obj);
        return obj;
    }
}

