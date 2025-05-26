using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused;
    private GameObject pauseMenu;
    [SerializeField] private GameObject gameStateScreen;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private InputActionAsset inputActionAsset;

    private void OnEnable()
    {
        EventManager.onPlayerDeath += ShowGameOverScreen;
        EventManager.onBossDeath += ShowVictoryScreen;
    }

    private void OnDisable()
    {
        EventManager.onPlayerDeath -= ShowGameOverScreen;
        EventManager.onBossDeath -= ShowVictoryScreen;
    }


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

    public void Quit()
    {
        Debug.Log("Quit!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ShowGameOverScreen()
    {
        gameStateScreen.SetActive(true);
        gameStateText.text = "You died";
    }

    public void ShowVictoryScreen()
    {
        gameStateScreen.SetActive(true);
        gameStateText.text = "Victory";
    }


}
