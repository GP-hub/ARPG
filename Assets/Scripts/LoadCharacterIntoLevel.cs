using System;
using UnityEngine;

public class LoadCharacterIntoLevel : MonoBehaviour
{
    [SerializeField]private GameObject[] characters;
    void Awake()
    {
        EventManager.onSceneLoad += CheckForGameSceneLoad;
    }

    void CheckForGameSceneLoad(string sceneName)
    {
        if (sceneName == LoaderManager.Scene.LevelScene.ToString())
        {
            LoadCharacterIn(EventManager.character);
        }
    }

    void LoadCharacterIn(int character)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
            {
                characters[i].SetActive(i == character);
            }
        }
    }
}
