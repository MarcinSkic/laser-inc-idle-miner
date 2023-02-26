using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;

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

    [SerializeField] RewardBat rewardBat;
    [SerializeField] Transform batParent;
    [SerializeField] int batsPer10000FixedUpdates;

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
        //view.InitBottomButtonsEventLegacy();
        view.InitWindows();
        #endregion

        #region UI Default Execution
        //view.SwitchWindowButtons(null, "UpgradeWindow");
        view.DisableAllWindows();
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
        #endregion

        #region Methods that require loaded data
        CreateAchievementsWindow();
        ConnectToUpgradesEvents();
        UpdateSettingsViewBySavedData();
        achievementManager.SetupAchievements();
        upgradesManager.SetupUpgrades();  //Order important #beforeUI
        SetupBallBars();    //Order important #UI
        CreateUpgradesUI(); //Order important #UI
        upgradesManager.SetInitialUpgradesValues(); //Order important #beforeExecuteLoaded
        upgradesManager.ExecuteLoadedUpgrades(); //Order important #last
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


    private void FixedUpdate()
    {
        SpawnBatOrNot();
    }
    private void SpawnBatOrNot()
    {
        if (Random.Range(0, 10000) < batsPer10000FixedUpdates)
        {
            RewardBat newBat = Instantiate(rewardBat, batParent);
            newBat.resourcesManager = resourcesManager;
        }
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
        resourcesManager.onPrestigeCurrencyChange += view.SetPrestigeCurrencyDisplay;

        onSetupFinished += () => { 
            resourcesManager.Money = resourcesManager.Money;
            resourcesManager.PrestigeCurrency = resourcesManager.PrestigeCurrency;
        }; //Welp ¯\_(ツ)_/¯
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
            resourcesManager.IncreaseMoneyForOfflineByValue(resourcesModel.offlineMoney);
            view.offlineGetBonusButton.Activate();
            view.offlinePopup.SetActive(false); 
        };

        view.offlineGetBonusButton.onClick += delegate {
            //TODO-FEATURE: Play AD, and do bonus if successful
            view.offlineGetBonusButton.Deactivate();
            resourcesModel.offlineMoney *= 2;
            view.SetOfflineMoney(resourcesModel.offlineMoney); 
        };

        view.cheatMoney.onClick += resourcesManager.CheatMoney;
        view.cheatMoney.SetText($"Cheat {NumberFormatter.Format(resourcesModel.cheatMoney)} money");

        view.cheatDepth.onClick += delegate { model.Depth += model.cheatDepth; };
        view.cheatDepth.SetText($"Dive {NumberFormatter.Format(model.cheatDepth)}m");

        view.forcePrestige.onClick += ExecutePrestige;

        view.eraseSaveFile.onClick += EraseSaveFile;

        view.cheatSpeedUp.SetText($"speed x 2 (now: { Time.timeScale})");
        view.cheatSpeedUp.onClick += delegate {
            Time.timeScale *= 2f;
            view.cheatSpeedUp.SetText($"speed x 2 (now: {Time.timeScale})");
            view.cheatSlowDown.SetText($"speed x 0.5 (now: {Time.timeScale})");
        };

        view.cheatSlowDown.SetText($"speed x 0.5 (now: { Time.timeScale})");
        view.cheatSlowDown.onClick += delegate {
            Time.timeScale *= 0.5f;
            view.cheatSpeedUp.SetText($"speed x 2 (now: {Time.timeScale})");
            view.cheatSlowDown.SetText($"speed x 0.5 (now: {Time.timeScale})");
        };

        view.is60fps.onValueChanged += value => { SettingsModel.Instance.Is60fps = value; };
        SettingsModel.Instance.Is60fps = view.is60fps.IsOn;

        view.displayFloatingDamage.onValueChanged += value => { SettingsModel.Instance.DisplayFloatingText = value; };
        SettingsModel.Instance.DisplayFloatingText = view.displayFloatingDamage.IsOn;
    }

    private void ConnectToAchievementsManager()
    {
        achievementManager.onAchievementUnlocked += view.CreateAchievementPopup;
    }

    private void UpdateSettingsViewBySavedData()
    {
        view.is60fps.IsOn = SettingsModel.Instance.Is60fps;
        view.displayFloatingDamage.IsOn = SettingsModel.Instance.DisplayFloatingText;
    }

    private void UpdateSettings()
    {
        view.debugWindow.SetActive(SettingsModel.Instance.showDebugWindow);
        view.cheatWindowButton.SetActive(SettingsModel.Instance.unlockCheatWindow);

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

    private void CreateAchievementsWindow()
    {
        view.InitAchievementsWindow();
        foreach(var achievement in achievementManager.achievements)
        {
            view.CreateAchievement(achievement);
        }
    }

    private void SetupBallBars()
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

                    if (upgrade.type == UpgradeType.SpawnUpgrade)
                    {
                        if (!upgrade.isUnlocked)
                        {
                            ballBar.Lock();
                            upgrade.onUnlock += ballBar.Unlock; //Must be connected always because default state is Locked
                            ballBar.toUnlockText.text = upgrade.toUnlockDescription;
                        } else
                        {
                            ballBar.Unlock(upgrade);
                        }

                        upgrade.onValueUpdate += buttonUpgrade.SetText;
                    } else
                    {
                        ballsModel.ballsData[upgrade.upgradedObjects].values[upgrade.upgradedValues].onValueChange += v => buttonUpgrade.SetText(NumberFormatter.Format(v));
                        buttonUpgrade.SetText(NumberFormatter.Format(ballsModel.ballsData[upgrade.upgradedObjects].values[upgrade.upgradedValues].Value));
                    }
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
        foreach (var upgrade in upgradesModel.upgrades.Values.Where(upgrade => upgrade.whereToGenerate != UISection.AutoOrNone).OrderBy(upgrade => upgrade.order))
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

        if (upgrade.currentLevel == upgrade.maxLevel)
        {
            upgradeBar.OnMaxed(upgrade);
        }
        else
        {
            switch (upgrade.currency)
            {
                case Currency.Money:
                    resourcesManager.onMoneyChange += upgradeBar.UpgradeButton.ChangeStateBasedOnMoney;
                    break;
                case Currency.Prestige:
                    resourcesManager.onPrestigeCurrencyChange += upgradeBar.UpgradeButton.ChangeStateBasedOnMoney;
                    break;
                case Currency.Premium:
                    resourcesManager.onPremiumCurrencyChange += upgradeBar.UpgradeButton.ChangeStateBasedOnMoney;
                    break;
            }

            if (upgrade.showUpgradedValueInsteadOfLevel)
            {
                upgrade.onValueUpdate += upgradeBar.SetValue;
            } else
            {
                upgrade.doUpgrade += upgradeBar.SetLevel;
            }
            
            upgrade.doUpgrade += upgradeBar.UpgradeButton.SetUpgradeCost;
            upgrade.onMaxedUpgrade += upgradeBar.OnMaxed;
        }

        if (upgrade.isUnlocked)
        {
            upgradeBar.Unlock(upgrade);
        }
        else
        {
            upgradeBar.Lock();
            upgrade.onUnlock += upgradeBar.Unlock;
        }
    }



    private void OnBlockDestroyed(double money)
    {
        resourcesManager.IncreaseMoney(money, true);
        blocksManager.IncrementDestroyedBlocksCount();
        //TODO-UGLY: It should be here but is in BasicBlock: count destroyed blocks for upgrades/rewards

    }

    private void OnReturnFromOffline(double seconds)
    {
        resourcesModel.offlineMoney = resourcesManager.CalculateOfflineMoney(seconds);
        view.ShowOfflineTimePopup(seconds, resourcesModel.offlineMoney);
    }
    
    private void OnDepthChange(double depth)
    {
        view.depthMeter.SetDepth(depth);
        view.SetBlocksHpDisplay(blocksManager.GetDepthBlocksHealth());
    }

    private void TryBuyUpgrade(Upgrade upgrade)
    {
        if (!resourcesManager.TryDecreaseCurrency(upgrade.cost,upgrade.currency))
        {
            return;
        }

        upgrade.DoUpgrade();

        resourcesManager.Money = resourcesManager.Money; //Welp (again) ¯\_(ツ)_/¯
        resourcesManager.PrestigeCurrency = resourcesManager.PrestigeCurrency;
        resourcesManager.PremiumCurrency = resourcesManager.PremiumCurrency;
    }

    private void LoadDefaults()
    {
        resourcesManager.LoadInspectorMoney();

    }

    private void SavePersistentData()
    {
        PersistentData persistentData = new();

        resourcesManager.SavePersistentData(persistentData);
        StatisticsModel.Instance.SavePersistentData(persistentData);
        persistentData.depth = model.Depth;
        upgradesManager.SavePersistentData(persistentData);
        blocksManager.SavePersistentData(persistentData);

        SettingsModel.Instance.SavePersistentData(persistentData);
        achievementManager.SavePersistentData(persistentData);

        savingManager.SavePersistentData(persistentData);
    }

    private bool LoadPersistentData()
    {
        PersistentData persistentData = savingManager.LoadPersistentData();

        if(persistentData == null)
        {
            savingManager.loadedProperly = true;
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

        savingManager.loadedProperly = true;
        return true;
    }

    private void ClearBeforePrestige()
    {
        FloatingTextSpawner.Instance.ClearBeforePrestige();
    }

    public void ExecutePrestige()
    {
        PersistentData persistentData = new();

        SettingsModel.Instance.SavePersistentData(persistentData);
        achievementManager.SavePersistentData(persistentData);
        persistentData.prestigeCurrency += resourcesManager.PrestigeCurrency + resourcesManager.PrestigeCurrencyForNextPrestige;

        savingManager.SavePersistentData(persistentData);

        ClearBeforePrestige();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EraseSaveFile()
    {
        SavingManager.EraseSaveFile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
