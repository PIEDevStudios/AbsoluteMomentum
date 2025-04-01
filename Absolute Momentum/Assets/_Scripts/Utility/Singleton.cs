using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Basic singleton and persistent singleton implementation with generic type.
/// </summary>

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this as T;
    }
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}

public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("NETWORK SINGLETON DELETED");
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
            Debug.Log("NETWORK SINGLETON CREATED: " + Instance.name);
        }
    }
    protected virtual void OnApplicationQuit()
    {
        Debug.Log("NETWORK SINGLETON DELETED");
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class NetworkSingletonPersistent<T> : NetworkSingleton<T> where T : NetworkBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("NETWORK SINGLETON DELETED");
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
    }
}

