using UnityEngine;

public class SceneLoadedEvent : MonoBehaviour
{
    private void Start()
    {
        EventManager.SceneLoad(LoaderManager.ReturnCurrentLoadedScene()); 
    }
}