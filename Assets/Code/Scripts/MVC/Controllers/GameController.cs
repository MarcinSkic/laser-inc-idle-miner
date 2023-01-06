using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

/// <summary>
/// Methods from this object should not be called by other objects. When such action direction is needed (for example UI or world events) it should connect methods to events HERE.
/// </summary>
public class GameController : BaseController<GameView>
{
    [Header("Models")]
    [SerializeField] private GameModel model;
    [SerializeField] private BallsModel ballsModel;
    [SerializeField] private UpgradesModel upgradesModel;
    [SerializeField] private ResourcesModel resourcesModel;

    [Header("Managers")]
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private ResourcesManager resourcesManager;
    [SerializeField] private OfflineManager offlineManager;
    [SerializeField] private BlocksManager blocksManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;

    //KEEP MONOBEHAVIOUR METHODS (Start, Update etc.) ON TOP
    /// <summary>
    /// This Start should be considered root Start of game, all inits where order of operations is important should originate from here
    /// </summary>
    void Start()
    {
        CreateBallBars();

        ConnectToUpgradesEvents();  //TODO-FT-CURRENT: Move this functionality to upgrade manager?
        ConnectToResourceManagerEvents();
        ConnectToOfflineManagerEvents();
        ConnectToBlocksManagerEvents();
        ConnectToGameModelEvents();
        ConnectToSettingsModelEvents();
        ConnectToViewElements();

        if (SettingsModel.Instance.loadSaveFile)
        {
            var loadSuccesful = LoadPersistentData();

            if (!loadSuccesful)
            {
                LoadDefaults();
            }
        } 
        else
        {
            LoadDefaults();
        }

        view.InitBottomButtonsEvent();
        view.SwitchWindowButtons(null, "UpgradeWindow");
        ConnectBallBarsWithEvents();

        UpdateSettings();
        
    }

    private void Update()
    {
        DisplayFPS();      
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SavePersistentData();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SavePersistentData();
        }
    }

    private void OnApplicationQuit()
    {
        
    }

    private void ConnectToUpgradesEvents()
    {
        foreach (var upgrade in upgradesModel.upgrades)
        {
            upgrade.AddOnTryUpgrade(TryBuyUpgrade);
        }
    }

    public void ConnectToResourceManagerEvents()
    {
        resourcesManager.onMoneyChange += view.SetMoneyDisplay;
    }

    private void ConnectToOfflineManagerEvents()
    {
        offlineManager.onReturnFromOffline += OnReturnFromOffline;
    }

    private void ConnectToBlocksManagerEvents()
    {
        blocksManager.onBlockDestroyed += OnBlockDestroyed;
    }

    private void ConnectToGameModelEvents()
    {
        model.onDepthChange += OnDepthChange;  
    }

    private void ConnectToSettingsModelEvents()
    {
        SettingsModel.Instance.onSettingsChange += UpdateSettings;
    }

    private void ConnectToViewElements()
    {
        view.InitButtons();

        view.offlineConfirmButton.onClick += delegate {
            resourcesManager.IncreaseMoney(resourcesModel.offlineMoney);
            view.offlineGetBonusButton.Activate();
            view.offlinePopup.SetActive(false); 
        };

        view.offlineGetBonusButton.onClick += delegate {
            //TODO-FEATURE: Play AD, and do bonus if successful
            view.offlineGetBonusButton.Deactivate();
            resourcesModel.offlineMoney *= 2;
            view.SetOfflineMoney(resourcesModel.offlineMoney); 
        };

        view.is60fps.onValueChanged.AddListener(Set60FPS);
        Set60FPS(view.is60fps.isOn);
    }

    private void UpdateSettings()
    {
        view.debugWindow.SetActive(SettingsModel.Instance.showDebugWindow);

        if (SettingsModel.Instance.changeTimeScale)
        {
            Time.timeScale = SettingsModel.Instance.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void Set60FPS(bool value)
    {
        if (value)
        {
            Application.targetFrameRate = 60;
        } else
        {
            Application.targetFrameRate = 30;
        }
    }

    private void CreateBallBars()
    {
        foreach(var ballType in ballsModel.ballsDataList)
        {
            view.CreateBallBar(ballType);
        }
    }

    private void ConnectBallBarsWithEvents()
    {
        foreach(var ballBar in view.ballBars)
        {
            foreach(var buttonUpgrade in ballBar.buttonUpgrades)
            {
                var upgrade = upgradesModel.GetUpgrade(buttonUpgrade.upgradeName);
                if(upgrade != null)
                {
                    buttonUpgrade.Init();
                    buttonUpgrade.onClick += upgrade.TryUpgrade;    //Button -> Tries to upgrade an Upgrade

                    buttonUpgrade.SetUpgradeCost(upgrade);   //Set initial values
                    resourcesManager.onMoneyChange += buttonUpgrade.ChangeStateBasedOnMoney;

                    upgrade.AddDoUpgrade(buttonUpgrade.SetUpgradeCost);  //Upgrade -> Updates Button values
                    upgrade.onValueUpdate += buttonUpgrade.SetUpgradeValue;  //TODO-FUTURE-BUG: There should be check if the button uses upgrade internal value or universal value, if universal then it should connect to not yet existing system of sending event on value change
                } 
                else
                {
                    Debug.LogWarningFormat("ButtonUpgrade needs: \"{0}\" upgrade, but it couldn't find it in upgrades collection.\n " +
                        "Maybe UpgradesModel misses scriptable upgrade instance or there is a typo in upgrade name?",buttonUpgrade.upgradeName);
                }   
            }
        }
    }

    private void OnBlockDestroyed(double money)
    {
        resourcesManager.IncreaseMoney(money);
        //TODO-FEATURE: Count destroyed blocks for upgrades/rewards

    }

    private void OnReturnFromOffline(double seconds)
    {
        resourcesModel.offlineMoney = 420 * seconds;    //TODO-FEATURE: CalculateOfflineMoney()
        view.ShowOfflineTimePopup(seconds, resourcesModel.offlineMoney);
    }
    
    private void OnDepthChange(double depth)
    {
        view.depthMeter.SetDepth(depth);
        view.SetBlocksHpDisplay(blocksManager.GetDepthBlocksHealth());
    }

    private void TryBuyUpgrade(Upgrade upgrade)
    {
        if (!resourcesManager.TryDecreaseMoney(upgrade.cost))
        {
            return;
        }

        upgrade.DoUpgrade();
    }

    private void LoadDefaults()
    {
        resourcesManager.LoadInspectorMoney();
    }

    private void SavePersistentData()
    {
        PersistentData persistentData = new();

        resourcesManager.SavePersistentData(persistentData);

        savingManager.SavePersistentData(persistentData);
    }

    private bool LoadPersistentData()
    {
        PersistentData persistentData = savingManager.LoadPersistentData();

        if(persistentData == null)
        {
            return false;
        }

        resourcesManager.LoadPersistentData(persistentData);

        return true;
    }

    private int fpsRefreshCounter = 0;
    private void DisplayFPS()
    {
        float current;
        if (fpsRefreshCounter == 0)
        {
            fpsRefreshCounter = 100;
            current = Time.frameCount / Time.time;
            view.avg_FpsDisplay.text = $"avg FPS: {Mathf.Round(current * 10) / 10f}";
            view.fpsDisplay.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";
        }
        else
        {
            fpsRefreshCounter--;
        }
    }
}
