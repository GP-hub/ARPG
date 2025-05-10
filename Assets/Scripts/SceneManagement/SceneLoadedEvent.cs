using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadedEvent : MonoBehaviour
{

    private void Start()
    {
        EventManager.SceneLoad(LoaderManager.ReturnCurrentLoadedScene());
    }

}