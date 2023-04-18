using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSceneSingleton<T> : Singleton<T> where T : Component
{
    protected override void Awake()
    {
        if (!Instance)
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
}
