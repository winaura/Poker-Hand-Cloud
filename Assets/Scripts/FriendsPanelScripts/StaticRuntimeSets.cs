using System;
using System.Collections.Generic;
using UnityEngine;

public static class StaticRuntimeSets
{
    public static Dictionary<string, GameObject> Items = new Dictionary<string, GameObject>();
    public static Dictionary<object, object> ItemsTypes = new Dictionary<object, object>();

    public static void Add(string name, GameObject gameObject)
    {
        if (!Items.ContainsValue(gameObject))
            Items.Add(name, gameObject);
    }

    public static void Remove(string name, GameObject gameObject)
    {
        if (Items.ContainsValue(gameObject))
            Items.Remove(name);
    }

    public static T GetType<T>()
    {
        try
        {
            return (T)ItemsTypes[typeof(T)];
        }
        catch
        {
            throw new ApplicationException("The requested service is not found.");
        }
    }
}
