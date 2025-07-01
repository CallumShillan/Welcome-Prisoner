using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generic Singleton base class for Unity MonoBehaviours.
/// Ensures only one instance exists and persists across scenes.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// Access the singleton instance.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        // Optionally, do not auto-create to avoid hidden objects in the scene.
                        var singletonObject = new GameObject($"(singleton) {typeof(T)}");
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"[Singleton] An instance of {typeof(T)} was created with DontDestroyOnLoad.");
                    }
                }
                return _instance;
            }
        }
        protected set
        {
            lock (_lock)
            {
                _instance = value;
            }
        }
    }

    /// <summary>
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} detected. Destroying this instance.");
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Set the quitting flag to avoid creating new instances during shutdown.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    /// <summary>
    /// Set the quitting flag if destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _applicationIsQuitting = true;
        }
    }
}
