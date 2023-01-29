using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

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

    public bool loadSaveFile = true;
    public bool showDebugWindow;
    [Space(5)]
    public bool changeTimeScale;
    [ConditionalField(nameof(changeTimeScale))]
    public float timeScale = 1f;

    [Header("Settings edited from game UI")]
    [ReadOnly][SerializeField] private bool is60fps;
    public bool Is60fps
    {
        get
        {
            return is60fps;
        }
        set
        {
            is60fps = value;
            onSettingsChange.Invoke();
        }
    }

    [ReadOnly][SerializeField] public bool displayFloatingText;

    public UnityAction onSettingsChange;

    private void OnValidate()
    {
        onSettingsChange?.Invoke();
    }
}
