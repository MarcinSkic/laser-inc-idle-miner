using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public bool displayFloatingText;
    public bool showDebugWindow;
    [Space(5)]
    public bool addDebugMoney;
    public double debugMoney = 0;
    [Space(5)]
    public bool changeTimeScale;
    public float timeScale = 1f;

    public UnityAction onSettingsChange;

    private void OnValidate()
    {
        onSettingsChange?.Invoke();
    }
}
