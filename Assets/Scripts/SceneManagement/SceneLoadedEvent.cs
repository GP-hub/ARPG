using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadedEvent : Singleton<SceneLoadedEvent>
{

    private void Start()
    {
        EventManager.SceneLoad(LoaderManager.ReturnCurrentLoadedScene());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Optional: Load an intermediate loading screen
        // yield return SceneManager.LoadSceneAsync(loadingSceneName);

        // Reset persistent systems (e.g., static classes)
        ResetAllPersistentState();

        // Now load target scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // Optional: Run post-load initializations
        yield return null; // wait one frame
        InitSceneSystems();
    }

    private void ResetAllPersistentState()
    {
        //SpellCharge.Reset();
        //EventManager.Clear(); // if needed
        // Add more resets as you need
    }

    private void InitSceneSystems()
    {
        // Optionally force certain systems to initialize
        // E.g., tell healthbars to rebind if they are loaded from pooled objects
    }
}