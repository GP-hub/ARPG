using UnityEngine;

public class SceneLoadedEvent : MonoBehaviour
{
    private void Awake()
    {
        EventManager.Instance?.SceneLoad(LoaderManager.ReturnCurrentLoadedScene());
    }
}
