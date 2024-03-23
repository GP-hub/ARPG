using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused;
    private GameObject pauseMenu;
    [SerializeField] private InputActionAsset inputActionAsset;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu == null)
            {
                pauseMenu = GameObject.Find("UICanvas").transform.Find("PauseMenuCanvas").gameObject;
            }
            if (pauseMenu != null)
            {
                PauseUnpause();
            }

        }
    }

    void PauseUnpause()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }


    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;

        inputActionAsset.Disable();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;

        inputActionAsset.Enable();
    }

    public void BackToMainMenu()
    {
        ResumeGame();
        LoaderManager.Load(LoaderManager.Scene.MainMenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
