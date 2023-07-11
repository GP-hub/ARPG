using UnityEngine;

/// <summary>
/// A utility class implementing the singleton pattern. Monobehaviour classes
/// which derive from the Singleton class will (1) have only a single instance
/// of the class at runtime, and (2) persist globally throughout a game session
/// via DontDestroyOnLoad.
/// </summary>
/// <typeparam name="T">The singleton class, of type MonoBehaviour.</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject
                    {
                        name = typeof(T).Name
                    };
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


