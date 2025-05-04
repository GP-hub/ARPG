using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : Singleton<BossFightManager>
{
    [SerializeField] private GameObject[] rockTransformArea1;
    [SerializeField] private GameObject[] rockTransformArea2;
    [SerializeField] private GameObject[] rockTransformArea3;
    [SerializeField] private GameObject[] rockTransformArea4;

    private GameObject[][] rockAreas;

    [SerializeField] private Transform bottomLeftCorner;
    [SerializeField] private Transform topRightCorner;
    [SerializeField] private float spacing;

    public Transform BottomLeftCorner { get => bottomLeftCorner; }
    public Transform TopRightCorner { get => topRightCorner; }
    public float Spacing { get => spacing; }

    protected override void Awake()
    {
        base.Awake(); 

        rockAreas = new GameObject[][]
        {
        rockTransformArea1,
        rockTransformArea2,
        rockTransformArea3,
        rockTransformArea4
        };

    }


    void OnEnable()
    {
        EventManager.onBossRockFall += SpawnRocks;
    }

    void OnDisable()
    {
        EventManager.onBossRockFall -= SpawnRocks;
    }

    private void SpawnRocks(int numberOfRocks)
    {
        foreach (GameObject[] area in rockAreas)
        {
            EnableRandomRocks(area, numberOfRocks);
        }
    }

    private void EnableRandomRocks(GameObject[] rocksArray, int numberToEnable)
    {
        if (rocksArray == null || rocksArray.Length == 0) return;

        int maxRocks = Mathf.Min(numberToEnable, rocksArray.Length);

        for (int i = 0; i < rocksArray.Length; i++)
        {
            int randomIndex = Random.Range(i, rocksArray.Length);
            (rocksArray[i], rocksArray[randomIndex]) = (rocksArray[randomIndex], rocksArray[i]);
        }

        for (int i = 0; i < maxRocks; i++)
        {
            if (rocksArray[i] != null)
                rocksArray[i].SetActive(true);
        }
    }

}
