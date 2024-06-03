using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]private GameObject characterSelectScreen;
    [SerializeField]private GameObject startScreen;

    [SerializeField] private GameObject[] availableCharacters;
    [SerializeField] private GameObject[] displayedCharacters;
    private GameObject characterSelected;

    public void Load()
    {
        LoaderManager.Load(LoaderManager.Scene.LevelScene);

        // HERE LOAD THE CORRECT SELECTED CHARACTER
    }
    public void Quit()
    {
        Debug.Log("Quit!");
        EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void CharacterSelection()
    {
        characterSelectScreen.SetActive(true);
        startScreen.SetActive(false);
        SelectCharacter(0);
    }

    public void SelectCharacter(int i)
    {
        try
        {
            characterSelected = availableCharacters[i];
            ChangeDisplayedCharacter(i);
            EventManager.character = i;
        }
        catch (Exception)
        {
            Debug.Log("That character is not available yet");
            throw;
        }
    }

    // The displayed chars are just the model of the playable characters
    public void ChangeDisplayedCharacter(int index)
    {
        for (int i = 0; i < displayedCharacters.Length; i++)
        {
            if (displayedCharacters[i] != null)
            {
                displayedCharacters[i].SetActive(i == index);
            }
        }
    }

}
