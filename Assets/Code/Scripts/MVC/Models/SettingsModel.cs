using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class SettingsModel : MonoBehaviour
{
    #region Singleton
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
        }
    }
    #endregion

    [Header("TRUE before build/commit")]
    public bool saveAndLoadFile = true;    //TODO: Change to SaveAndLoadFile to block undesired changes to saved file. Also add context menu function to erase existing savefile
    public bool doOfflineEarning = true;
    public bool spawnBlocks = true;

    [Header("FALSE before build/commit")]
    public bool removeUpgradesRequirements;
    public bool showDebugWindow;
    [Space(5)]
    public bool changeTimeScale;
    [ConditionalField(nameof(changeTimeScale))]
    public float timeScale = 1f;
    public float clickDamage;

    [Header("Settings edited from game UI")]
    #region Is60fps
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
            onSettingsChange?.Invoke();
        }
    }
    #endregion

    #region DisplayFloatingText
    [ReadOnly][SerializeField] private bool displayFloatingText;
    public bool DisplayFloatingText
    {
        get
        {
            return displayFloatingText;
        }
        set
        {
            displayFloatingText = value;
            onSettingsChange?.Invoke();
        }
    }
    #endregion

    [Header("Debug informations")]
    [ReadOnly] public int ballsAttemptedCorrections = 0;
    [ReadOnly] public int ballsAppliedCorrections = 0;

    public UnityAction onSettingsChange;

    private void OnValidate()
    {
        onSettingsChange?.Invoke();
    }

    public void SavePersistentData(PersistentData data)
    {
        data.is60fps = Is60fps;
        data.displayFloatingText = DisplayFloatingText;
    }

    public void LoadPersistentData(PersistentData data)
    {
        Is60fps = data?.is60fps ?? true;
        DisplayFloatingText = data?.displayFloatingText ?? false;
    }

    [ContextMenu("ERASE Save File")]
    private void EraseSaveFile()
    {
        SavingManager.EraseSaveFile();
    }

    [ContextMenu("Restore Normal Settings")]
    private void RestoreNormalSettings()
    {
        saveAndLoadFile = true;    //TODO: Change to SaveAndLoadFile to block undesired changes to saved file. Also add context menu function to erase existing savefile
        doOfflineEarning = true;
        spawnBlocks = true;
        showDebugWindow = false;
        changeTimeScale = false;
    }
}
