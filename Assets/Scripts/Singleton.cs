using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component
{
    static public T Instance { get; protected set; }

    protected virtual void Awake()
    {
        SubscribeToEvents();
        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        UnSubscribeFromEvents();
    }

    protected virtual void SubscribeToEvents() { }

    protected virtual void UnSubscribeFromEvents() { }
}