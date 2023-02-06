using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Here you write any function that is universal and needed globally
/// </summary>
public static class Functions {
    public static T GetObjectCopy<T>(T obj)
    {
        var json = JsonUtility.ToJson(obj);
        return JsonUtility.FromJson<T>(json);
    }

    public static void DisplayDictionary<T1,T2>(Dictionary<T1,T2> dictionary,Func<T1,string> displayKey,Func<T2,string> displayValue)
    {
        string message = "";
        foreach(var pair in dictionary)
        {
            message += $"{displayKey(pair.Key)}: {displayValue(pair.Value)}\n";
        }
        Debug.Log(message);
    }
}
