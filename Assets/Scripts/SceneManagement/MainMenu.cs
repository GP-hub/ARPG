using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject characterSelectScreen;
    [SerializeField] private GameObject startScreen;

    [SerializeField] private GameObject[] availableCharacters;
    [SerializeField] private GameObject[] displayedCharacters;
    [SerializeField] private GameObject[] charactersAbilities;
    private GameObject characterSelected;

    [SerializeField] private TMP_Text characterSelectButtonText;
    [SerializeField] private Button characterSelectButton;

    public void Load()
    {
        LoaderManager.Load(LoaderManager.Scene.LevelScene);

        // HERE LOAD THE CORRECT SELECTED CHARACTER (actually its done in the envent manager for now ?)
    }
    public void Quit()
    {
        Debug.Log("Quit!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void CharacterSelection()
    {
        characterSelectScreen.SetActive(true);
        startScreen.SetActive(false);
        SelectCharacter(0);
    }

    public void BackToMainMenu()
    {
        characterSelectScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    public void SelectCharacter(int i)
    {
        characterSelectButtonText.text = (i != 0) ? "Locked" : "Start";
        characterSelectButtonText.raycastTarget = (i == 0);
        characterSelectButton.interactable = (i == 0);

        try
        {
            characterSelected = availableCharacters[i];
            ChangeDisplayedCharacterAndAbilities(i);
            UpdateDisplayCharacterAnimator(i);

            EventManager.character = i;
        }
        catch (Exception)
        {
            Debug.Log("That character is not available yet");
            throw;
        }
    }

    // The displayed chars are just the model of the playable characters
    public void ChangeDisplayedCharacterAndAbilities(int index)
    {
        for (int i = 0; i < displayedCharacters.Length; i++)
        {
            if (displayedCharacters[i] != null)
            {
                displayedCharacters[i].SetActive(i == index);
                charactersAbilities[i].SetActive(i == index);
            }
        }
    }

    public void UpdateDisplayCharacterAnimator(int index)
    {
        Animator animator = displayedCharacters[index].GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("CharNumber", index);
        }
        else
        {
            Debug.LogWarning("Animator not found on character display at index " + index);
        }
    }

}
