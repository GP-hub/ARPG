using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManagerSingleton : Singleton<PoolingManagerSingleton>
{
    [Serializable]
    public class ObjectPool
    {
        public GameObject prefab;
        public int poolSize;
        public List<GameObject> objects;
    }

    public List<ObjectPool> objectPools;

    void Awake()
    {
        EventManager.onSceneLoad += CheckForGameSceneLoad;
        //InitializeObjectPools();
    }

    void CheckForGameSceneLoad(string sceneName)
    {
        if (sceneName == LoaderManager.Scene.LevelScene.ToString())
        {
            InitializeObjectPools();
        }
        //else
        //{
        //    foreach (ObjectPool pool in objectPools)
        //    {
        //        pool.objects = new List<GameObject>();
        //        for (int i = 0; i < pool.poolSize; i++)
        //        {
        //            pool.objects = null;
        //        }
        //    }
        //}
    }

    void InitializeObjectPools()
    {
        foreach (ObjectPool pool in objectPools)
        {
            pool.objects = new List<GameObject>();
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                pool.objects.Add(obj);
            }
        }
    }

    public GameObject GetObjectFromPool(string prefabName, Vector3 position)
    {
        ObjectPool pool = objectPools.Find(x => x.prefab.name == prefabName);

        if (pool != null)
        {
            foreach (GameObject obj in pool.objects)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.transform.position = position;
                    obj.SetActive(true);
                    return obj;
                }
            }

            // If no inactive object is found, create a new one
            GameObject newObj = Instantiate(pool.prefab);
            pool.objects.Add(newObj);
            return newObj;
        }
        Debug.Log("test");
        return null;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

}
