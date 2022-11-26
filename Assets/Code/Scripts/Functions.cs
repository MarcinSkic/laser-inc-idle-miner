using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Here you write any function that is universal and needed globally
/// </summary>
public static class Functions {
    public static T GetObjectCopy<T>(T obj)
    {
        var json = JsonUtility.ToJson(obj);
        return JsonUtility.FromJson<T>(json);
    }
}
