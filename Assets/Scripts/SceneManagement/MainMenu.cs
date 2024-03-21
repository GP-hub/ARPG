using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Load()
    {
        LoaderManager.Load(LoaderManager.Scene.LevelScene);
    }
    public void Quit()
    {
        Debug.Log("Quit!");
        EditorApplication.isPlaying = false;
        Application.Quit();
    }

}
