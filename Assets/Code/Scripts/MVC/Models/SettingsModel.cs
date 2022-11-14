using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsModel : MonoBehaviour
{


    public static SettingsModel Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        } 
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
