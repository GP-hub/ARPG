using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private GameObject[] rockTransformArea1;
    [SerializeField] private GameObject[] rockTransformArea2;
    [SerializeField] private GameObject[] rockTransformArea3;
    [SerializeField] private GameObject[] rockTransformArea4;

    private GameObject[][] rockAreas;

    void Awake()
    {
        // Initialize array of arrays (jagged array)
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
        foreach (var area in rockAreas)
        {
            EnableRandomRocks(area, numberOfRocks);
        }
    }

    private void EnableRandomRocks(GameObject[] rocksArray, int numberToEnable)
    {
        if (rocksArray == null || rocksArray.Length == 0) return;

        int maxRocks = Mathf.Min(numberToEnable, rocksArray.Length);

        // Fisher-Yates shuffle manually
        for (int i = 0; i < rocksArray.Length; i++)
        {
            int randomIndex = Random.Range(i, rocksArray.Length);
            (rocksArray[i], rocksArray[randomIndex]) = (rocksArray[randomIndex], rocksArray[i]);
        }

        // Activate first 'maxRocks' rocks
        for (int i = 0; i < maxRocks; i++)
        {
            if (rocksArray[i] != null)
                rocksArray[i].SetActive(true);
        }
    }
}
