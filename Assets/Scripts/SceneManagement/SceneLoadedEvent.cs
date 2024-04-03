using UnityEngine;

public class SceneLoadedEvent : MonoBehaviour
{
    private void Awake() { EventManager.SceneLoad(LoaderManager.ReturnCurrentLoadedScene()); }
}