using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;
using System.Linq;

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
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private UpgradesManager upgradesManager;
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private ResourcesManager resourcesManager;
    [SerializeField] private OfflineManager offlineManager;
    [SerializeField] private BlocksManager blocksManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private AchievementManager achievementManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;

    //KEEP MONOBEHAVIOUR METHODS (Start, Update etc.) ON TOP
    /// <summary>
    /// This Start should be considered root Start of game, all inits where order of operations is important should originate from here
    /// </summary>
    void Start()
    {
        #region UI Creation
        CreateBallBars();
        #endregion

        #region UI Initialization
        view.InitBottomButtonsEvent();
        #endregion

        #region UI Default Execution
        view.SwitchWindowButtons(null, "UpgradeWindow");
        #endregion

        #region Event connections where loading data triggers event
        ConnectToResourceManagerEvents();
        ConnectToGameModelEvents();
        ConnectToSettingsModelEvents();
        ConnectToViewElements();
        #endregion

        #region Loading Saved Data
        if (SettingsModel.Instance.saveAndLoadFile)
        {
            savingManager.loadedProperly = LoadPersistentData();

            if (!savingManager.loadedProperly)
            {
                LoadDefaults();
            }
        }
        else
        {
            LoadDefaults();
        }
        #endregion

        #region Methods that require loaded data
        ConnectToUpgradesEvents();  //TODO-FT-CURRENT: Move this functionality to upgrade manager?
        ConnectBallBarsWithEvents();
        UpdateSettingsViewBySavedData();
        achievementManager.SetupAchievements();
        upgradesManager.ConnectUpgrades();  //Order important
        achievementManager.ConnectUpgrades();
        upgradesManager.ExecuteLoadedUpgrades(); //Order important
        CreateUpgradesUI();
        #endregion

        #region Methods independent from calling order
        ConnectToOfflineManagerEvents();
        ConnectToBlocksManagerEvents();
        ConnectToAchievementsManager();
        UpdateSettings();
        MoveBorderToMatchScreenSize();
        #endregion

        onSetupFinished?.Invoke();

    }
    public UnityAction onSetupFinished;

    private void Update()
    {
        DisplayFPS();      
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && SettingsModel.Instance.saveAndLoadFile)
        {
            SavePersistentData();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && SettingsModel.Instance.saveAndLoadFile)
        {
            SavePersistentData();
        }
    }

    private void OnApplicationQuit()
    {
        
    }

    private void ConnectToUpgradesEvents()
    {
        foreach (var upgrade in upgradesModel.upgrades.Values)
        {
            upgrade.doTryUpgrade += TryBuyUpgrade;
        }
    }

    public void ConnectToResourceManagerEvents()
    {
        resourcesManager.onMoneyChange += view.SetMoneyDisplay;
        onSetupFinished += () => { resourcesManager.Money = resourcesManager.Money; }; //Welp ¯\_(ツ)_/¯
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

        view.is60fps.onValueChanged.AddListener(value => { SettingsModel.Instance.Is60fps = value; });
        SettingsModel.Instance.Is60fps = view.is60fps.isOn;

        view.displayFloatingDamage.onValueChanged.AddListener(value => { SettingsModel.Instance.DisplayFloatingText = value; });
        SettingsModel.Instance.DisplayFloatingText = view.displayFloatingDamage.isOn;
    }

    private void ConnectToAchievementsManager()
    {
        achievementManager.onAchievementUnlocked += view.CreateAchievementPopup;
    }

    private void UpdateSettingsViewBySavedData()
    {
        view.is60fps.isOn = SettingsModel.Instance.Is60fps;
        view.displayFloatingDamage.isOn = SettingsModel.Instance.DisplayFloatingText;
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

        if (SettingsModel.Instance.Is60fps)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = 30;
        }
    }


    private void MoveBorderToMatchScreenSize()
    {
        model.bottomBorder.position = Camera.main.ScreenToWorldPoint(new Vector3(0f,model.heightOfBottomBar,-Camera.main.transform.position.z));
    }

    private void CreateBallBars()
    {
        foreach(var ballType in ballsModel.ballsData.Values)
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
                if (upgradesModel.upgrades.ContainsKey(buttonUpgrade.upgradeName)){
                    var upgrade = upgradesModel.upgrades[buttonUpgrade.upgradeName];
                    buttonUpgrade.Init();
                    buttonUpgrade.onClick += upgrade.TryUpgrade;    //Button -> Tries to upgrade an Upgrade

                    buttonUpgrade.SetUpgradeCost(upgrade);   //Set initial values
                    resourcesManager.onMoneyChange += buttonUpgrade.ChangeStateBasedOnMoney;

                    upgrade.doUpgrade += buttonUpgrade.SetUpgradeCost; //Upgrade -> Updates Button values
                    upgrade.onValueUpdate += buttonUpgrade.SetText;  //TODO-FUTURE-BUG: There should be check if the button uses upgrade internal value or universal value, if universal then it should connect to not yet existing system of sending event on value change
                } 
                else
                {
                    Debug.LogWarningFormat("Ball ButtonUpgrade needs: \"{0}\" upgrade, but it couldn't find it in upgrades collection.\n " +
                        "Maybe UpgradesModel misses scriptable upgrade instance or there is a typo in upgrade name?",buttonUpgrade.upgradeName);
                }   
            }
        }
    }

    private void CreateUpgradesUI()
    {
        foreach (var upgrade in upgradesModel.upgrades.Values.Where(upgrade => upgrade.whereToGenerate == UISection.UpgradesOther).OrderBy(upgrade => upgrade.order))
        {
            var upgradeBar = view.CreateUpgradeBar(upgrade);
            ConnectUpgradeBar(upgradeBar, upgrade);
        }
    }

    private void ConnectUpgradeBar(UIUpgradeBar upgradeBar, Upgrade upgrade)
    {
        upgradeBar.UpgradeButton.Init();
        upgradeBar.UpgradeButton.onClick += upgrade.TryUpgrade;

        upgradeBar.UpgradeButton.SetUpgradeCost(upgrade);
        resourcesManager.onMoneyChange += upgradeBar.UpgradeButton.ChangeStateBasedOnMoney;

        upgrade.doUpgrade += upgradeBar.UpgradeButton.SetUpgradeCost;
        upgrade.doUpgrade += upgradeBar.SetLevel;
    }

    private void OnBlockDestroyed(double money)
    {
        resourcesManager.IncreaseMoney(money);
        blocksManager.incrementDestroyedBlocksCount();
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
        SettingsModel.Instance.SavePersistentData(persistentData);
        StatisticsModel.Instance.SavePersistentData(persistentData);
        persistentData.depth = model.Depth;
        upgradesManager.SavePersistentData(persistentData);
        blocksManager.SavePersistentData(persistentData);
        achievementManager.SavePersistentData(persistentData);

        savingManager.SavePersistentData(persistentData);
    }

    private bool LoadPersistentData()
    {
        PersistentData persistentData = savingManager.LoadPersistentData();

        if(persistentData == null)
        {
            return false;
        }

        #region AnyOrder
        resourcesManager.LoadPersistentData(persistentData);
        achievementManager.LoadPersistentData(persistentData);
        SettingsModel.Instance.LoadPersistentData(persistentData);
        StatisticsModel.Instance.LoadPersistentData(persistentData);
        model.Depth = persistentData.depth;
        #endregion
        
        #region OrderImportant
        blocksManager.LoadPersistentData(persistentData);
        upgradesManager.LoadPersistentData(persistentData);
        #endregion

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
