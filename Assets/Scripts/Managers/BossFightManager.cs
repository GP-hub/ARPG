using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossFightManager : Singleton<BossFightManager>
{
    private GameObject[][] rockAreas;
    private Transform bottomLeftCorner;
    private Transform topRightCorner;

    public Transform BottomLeftCorner => bottomLeftCorner;
    public Transform TopRightCorner => topRightCorner;

    protected override void Awake()
    {
        base.Awake();
    }


    void OnEnable()
    {
        EventManager.onBossRockFall += SpawnRocks;
        EventManager.onSceneLoad += FetchBossArenaRocks;
    }

    void OnDisable()
    {
        EventManager.onBossRockFall -= SpawnRocks;
        EventManager.onSceneLoad -= FetchBossArenaRocks;
    }

    private void SpawnRocks(int numberOfRocks)
    {
        if (rockAreas == null) return;
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

    private void FetchBossArenaRocks(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        if (!sceneName.Contains("LevelScene")) return;

        BossArenaReferences arena = FindFirstObjectByType<BossArenaReferences>();
        if (arena == null)
        {
            Debug.Log("BossArenaReferences not found in scene.");
            return;
        }

        rockAreas = arena.RockAreas;
        bottomLeftCorner = arena.BottomLeftCorner;
        topRightCorner = arena.TopRightCorner;
    }

}
