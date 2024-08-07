﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;
// for Math.Log10 for progression debugging
using System;
using UnityEngine.Video;
using Fyber;

/// <summary>
/// Methods from this object should not be called by other objects. When such action direction is needed (for example UI or world events) it should connect methods to events HERE.
/// </summary>
public class GameController : BaseController<GameView>
{
    [Header("GAME CONTROLLER")]
    [Header("Models")]
    [SerializeField] private GameModel model;
    [SerializeField] private BallsModel ballsModel;
    [SerializeField] private UpgradesModel upgradesModel;
    [SerializeField] private ResourcesModel resourcesModel;
    [SerializeField] private SettingsModel settingsModel;
    [SerializeField] private GameModel gameModel;
    [SerializeField] private AdManager adManager;
    [SerializeField] private DailyManager dailyManager;

    [Header("Managers")]
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private UpgradesManager upgradesManager;
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private ResourcesManager resourcesManager;
    [SerializeField] private OfflineManager offlineManager;
    [SerializeField] private BlocksManager blocksManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private AchievementManager achievementManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private TutorialManager tutorialManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private PremiumStoreManager premiumStoreManager;

    [Header("Bats!")]
    [SerializeField] RewardBat rewardBat;
    [SerializeField] Transform batParent;

    [Header("Dyson swarm")]
    [SerializeField] GameObject[] cavernCameras;
    [SerializeField] GameObject dysonCamera;
    [SerializeField] Worlds currentWorld = Worlds.Cavern;
    public bool visitedDyson = false;

    [Header("ProgressionDebug")]
    [SerializeField] float previousMoneyProgressionDebugTime = 0;
    [SerializeField] double previousEarnedMoney;
    public List<string> earnedMoneyMessages;
    [SerializeField] float previousDepthProgressionDebugTime = 0;
    [SerializeField] double previousDepth;
    public List<string> depthMessages;

    [Header("Integrations and other shit")]
    private Firebase.FirebaseApp app;

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
        tutorialManager.Init();
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
        ConfigureDysonSwarm();
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
        ConnectToPremiumStoreManagerEvents();
        UpdateSettings();
        MoveBorderToMatchScreenSize();
        ManageTutorial();
        #endregion

        onSetupFinished?.Invoke();

        if (settingsModel.unlockCheatWindow)
        {
            InvokeRepeating(nameof(DebugProgression), 0f, 1f);
        }

        AudioManager.Instance.Play("theme");
        AudioManager.Instance.IncreaseVolumeOverTime();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log(app);
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

    }
    public UnityAction onSetupFinished;

    private void Update()
    {
        if (SettingsModel.Instance.ShowDebugWindow)
        {
            DisplayFPS();
        }
    }

    private void FixedUpdate()
    {
        TrySpawnBat();
    }
    private void TrySpawnBat()
    {
        if (settingsModel.spawnBats && UnityEngine.Random.Range(0, 10000) < gameModel.batsPer10000FixedUpdates)
        {
            RewardBat newBat = Instantiate(rewardBat, batParent);
            newBat.Init(resourcesManager,adManager, !gameModel.batFrenzyActive);
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
        if (SettingsModel.Instance.saveAndLoadFile)
        {
            SavePersistentData();
        }
    }

    private void ConfigureDysonSwarm()
    {
        var atLeastOnePrestige = resourcesManager.ExecutedPrestigesCount != 0;

        view.dysonSwarmButton.gameObject.SetActive(atLeastOnePrestige);

        if (atLeastOnePrestige && !visitedDyson)
        {
            view.flashingDysonSwarmButtonCoroutine = StartCoroutine(FlashingDysonSwarmButton());

            view.dysonSwarmButton.onClick += StopFlashingOfDysonSwarm;
        }
    }

    private void SwitchWorld(UIButtonController button, string parameter)
    {
        var switchingToCaverns = currentWorld != Worlds.Cavern;
        if (switchingToCaverns)
        {
            currentWorld = Worlds.Cavern;
            dysonCamera.SetActive(false);   // if more worlds then change it to dictionary or something
            cavernCameras.ForEach((c) => { c.SetActive(true); });
            view.depthMeter.gameObject.SetActive(true);
            view.dysonSwarmStory.Hide();
            view.dysonSwarmDescription.SetActive(false);
            view.dysonSwarmStoryOpenButton.gameObject.SetActive(false);
            view.dysonSwarmTitle.gameObject.SetActive(false);

            FloatingTextSpawner.Instance.disableSpawning = false;

            button.Deselect();
            
        } 
        else
        {
            view.depthMeter.gameObject.SetActive(false);
            FloatingTextSpawner.Instance.disableSpawning = true;
            FloatingTextSpawner.Instance.DisableHanging();

            cavernCameras.ForEach((c) => { c.SetActive(false); });
            button.Select();
            

            switch (parameter)
            {
                case "dyson":
                    var atLeastOnePrestige = resourcesManager.ExecutedPrestigesCount != 0;
                    if (atLeastOnePrestige && !visitedDyson)
                    {
                        view.dysonSwarmStory.Show();
                    }

                    visitedDyson = true;
                    view.dysonSwarmDescription.SetActive(true);
                    view.dysonSwarmStoryOpenButton.gameObject.SetActive(true);
                    view.dysonSwarmTitle.gameObject.SetActive(true);
                    dysonCamera.SetActive(true);
                    currentWorld = Worlds.DysonSwarm;
                    break;
            }
        }
    }


    private void ConnectToUpgradesEvents()
    {
        foreach (var upgrade in upgradesModel.upgrades.Values)
        {
            upgrade.doTryUpgrade += TryBuyUpgrade;
        }
    }

    private void ManageTutorial()
    {
        if (!tutorialManager.finishedTutorial)
        {
            tutorialManager.BeginTutorial();
        }
    }

    public void ConnectToResourceManagerEvents()
    {
        resourcesManager.onMoneyChange += view.SetMoneyDisplay;
        resourcesManager.onPrestigeCurrencyChange += view.SetPrestigeCurrencyDisplay;
        resourcesManager.onPremiumCurrencyChange += view.SetPremiumCurrencyDisplay;

        onSetupFinished += () => { 
            resourcesManager.Money = resourcesManager.Money;
            resourcesManager.PrestigeCurrency = resourcesManager.PrestigeCurrency;
            resourcesManager.PremiumCurrency = resourcesManager.PremiumCurrency;
            StatisticsModel.Instance.AchievementsCount = StatisticsModel.Instance.AchievementsCount;
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

    private void ConnectToPremiumStoreManagerEvents()
    {
        premiumStoreManager.Setup();
        premiumStoreManager.onPremiumBuy += SavePersistentData;
    }

    private IEnumerator FlashingDysonSwarmButton()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            view.dysonSwarmButton.Select();
            yield return new WaitForSeconds(0.5f);
            view.dysonSwarmButton.Deselect();
        }
    }

    private void StopFlashingOfDysonSwarm(UIButtonController button, string _)
    {
        StopCoroutine(view.flashingDysonSwarmButtonCoroutine);
        view.dysonSwarmButton.onClick -= StopFlashingOfDysonSwarm;
        view.dysonSwarmButton.Select();
    }

    public void HandleDoubleOfflineReward()
    {
        // TODO dla marcina - jakiś check czy już nie było x2? choć w sumie nie wiem czy potrzebne
        view.offlineGetBonusButton.Deactivate();
        resourcesModel.offlineMoney *= 2;
        view.SetOfflineMoney(resourcesModel.offlineMoney);
    }

    public void AcceptOfflineReward(int limit = 0){
        if (limit == 0 || limit >= offlineManager.currentlyInvokedOfflineTime)
        {
            offlineManager.offlineRewardWasReceived = true;
            resourcesManager.IncreaseMoneyForOfflineByValue(resourcesModel.offlineMoney);
            view.offlineGetBonusButton.Activate();
            view.offlinePopup.SetActive(false);
            offlineManager.currentlyInvokedOfflineTime = 0;
        }
    }

    IEnumerator TryActivateOfflineGetBonusButton()
    {
        while (true)
        {
            if (!view.offlineGetBonusButton.enabled)
            {
                yield break;
            }
            if (adManager.RewardAvailable())
            {
                view.offlineGetBonusButton.Activate();
                view.offlineGetBonusButton.GetComponentInChildren<Spinner>().gameObject.SetActive(false);
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    private void ConnectToViewElements()
    {
        view.InitButtons();
        view.offlineGetBonusButton.Deactivate();

        #region PowerUps
        resourcesManager.onPowerUpTimeIncrease += _ => { view.damagePowerUp.StartTimer(); };
        #endregion

        #region OfflineTimePopup
        view.offlineConfirmButton.onClick += delegate {
            AcceptOfflineReward();
        };

        view.offlineGetBonusButton.onClick += delegate {
            adManager.TryShowDoubleOfflineGainAd();
        };
        StartCoroutine(TryActivateOfflineGetBonusButton());
        #endregion

        #region SettingsWindow
        view.is60fps.onValueChanged += value => { SettingsModel.Instance.Is60fps = value; };
        SettingsModel.Instance.Is60fps = view.is60fps.IsOn;

        view.displayFloatingDamage.onValueChanged += value => { SettingsModel.Instance.DisplayFloatingText = value; };
        SettingsModel.Instance.DisplayFloatingText = view.displayFloatingDamage.IsOn;

        view.useAlternativeNotation.onValueChanged += value => { SettingsModel.Instance.UseAlternativeNotation = value; };
        SettingsModel.Instance.UseAlternativeNotation = view.useAlternativeNotation.IsOn;

        view.playMusic.onValueChanged += value => { SettingsModel.Instance.PlayMusic = value; };
        SettingsModel.Instance.PlayMusic = view.playMusic.IsOn;

        view.playSounds.onValueChanged += value => { SettingsModel.Instance.PlaySounds = value; };
        SettingsModel.Instance.PlaySounds = view.playSounds.IsOn;

        view.username.onEndEdit.AddListener(value => { SettingsModel.Instance.Username = value; } );
        SettingsModel.Instance.Username = view.username.text;

        view.eraseSaveFile.onClick += TryEraseSaveFile;
        #endregion

        #region CheatsWindow
        view.cheatMoney.onClick += resourcesManager.CheatMoney;
        view.cheatMoney.SetText($"Cheat {NumberFormatter.Format(resourcesModel.cheatMoney)} money");

        view.cheatDepth.onClick += delegate { model.Depth += model.cheatDepth; };
        view.cheatDepth.SetText($"Dive {NumberFormatter.Format(model.cheatDepth)}m");

        view.forcePrestige.onClick += TryExecutePrestige;

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

        if (settingsModel.spawnBats)
        {
            view.cheatToggleBatsSpawn.SetText("turn bats off");
        }
        else
        {
            view.cheatToggleBatsSpawn.SetText("turn bats on");
        }
        view.cheatToggleBatsSpawn.onClick += delegate
        {
            settingsModel.spawnBats = !settingsModel.spawnBats;
            if (settingsModel.spawnBats)
            {
                view.cheatToggleBatsSpawn.SetText("turn bats off");
            } else
            {
                view.cheatToggleBatsSpawn.SetText("turn bats on");
            }
        };

        view.showDebugWindow.onValueChanged += v => {SettingsModel.Instance.ShowDebugWindow = v; };
        SettingsModel.Instance.ShowDebugWindow = view.showDebugWindow.IsOn;
        #endregion

        view.dysonSwarmButton.onClick += SwitchWorld;

    }

    private void ConnectToAchievementsManager()
    {
        achievementManager.onAchievementUnlocked += view.CreateAchievementPopup;
    }

    private void UpdateSettingsViewBySavedData()
    {
        view.is60fps.IsOn = SettingsModel.Instance.Is60fps;
        view.displayFloatingDamage.IsOn = SettingsModel.Instance.DisplayFloatingText;
        view.useAlternativeNotation.IsOn = SettingsModel.Instance.UseAlternativeNotation;
        view.playMusic.IsOn = SettingsModel.Instance.PlayMusic;
        view.playSounds.IsOn = SettingsModel.Instance.PlaySounds;
        view.username.text = SettingsModel.Instance.Username;
    }

    private void UpdateSettings()
    {
        view.debugWindow.SetActive(SettingsModel.Instance.ShowDebugWindow);
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


        if (SettingsModel.Instance.PlayMusic)
        {
            AudioManager.Instance.SetMusicPlaying(true);
        }
        else
        {
            AudioManager.Instance.SetMusicPlaying(false);
        }
        if (SettingsModel.Instance.PlaySounds)
        {
            AudioManager.Instance.SetSoundPlaying(true);
        }
        else
        {
            AudioManager.Instance.SetSoundPlaying(false);
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
        view.InitAchievementsWindow(achievementManager.achievementsInRow);
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
                        if(upgrade.currentLevel >= upgrade.maxLevel)
                        {
                            buttonUpgrade.MaxLock();
                        } else
                        {
                            upgrade.onMaxedUpgrade += delegate (Upgrade _) { buttonUpgrade.MaxLock(); };
                        }
                        if (!upgrade.isUnlocked)
                        {
                            ballBar.Lock();
                            upgrade.onUnlock += ballBar.Unlock; //Must be connected always because default state is Locked
                            ballBar.toUnlockText.text = upgrade.toUnlockDescription;
                        } else
                        {
                            ballBar.Unlock(upgrade);
                        }

                        ballsModel.ballsCount[upgrade.upgradedObjects].onValueChange += v => buttonUpgrade.SetText(v.ToString());
                        buttonUpgrade.SetText("0");
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
    
    // TODO - ugly?
    public void RedrawDepthMeter()
    {
        OnDepthChange(model.Depth);
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
        persistentData.visitedDyson = visitedDyson;
        upgradesManager.SavePersistentData(persistentData);
        blocksManager.SavePersistentData(persistentData);
        tutorialManager.SavePersistentData(persistentData);

        SettingsModel.Instance.SavePersistentData(persistentData);
        achievementManager.SavePersistentData(persistentData);
        dailyManager.SavePersistentData(persistentData);

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
        tutorialManager.LoadPersistentData(persistentData);
        model.Depth = persistentData.depth;
        visitedDyson = persistentData?.visitedDyson ?? false;
        dailyManager.LoadPersistentData(persistentData);
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

    public void TryExecutePrestige()
    {
        if (resourcesManager.IsPrestigeWorth())
        {
            ExecutePrestige();
        }
        else
        {
            MessagesManager.Instance.DisplayConfirmQuestion("Are you certain?", "You will not earn much prestige currency",
            () =>
            {
                ExecutePrestige();
            });
        }          
    }

    private void ExecutePrestige()
    {
        resourcesManager.IncreasePrestigeCurrency(resourcesManager.PrestigeCurrencyForNextPrestige);
        resourcesManager.ExecutedPrestigesCount += 1;

        PersistentData persistentData = new();

        persistentData.visitedDyson = visitedDyson;
        SettingsModel.Instance.SavePersistentData(persistentData);
        achievementManager.SavePersistentData(persistentData);  
        resourcesManager.SavePrestigePersistentData(persistentData);
        upgradesManager.SavePrestigePersistentData(persistentData);
        dailyManager.SavePersistentData(persistentData);
        tutorialManager.SavePersistentData(persistentData);

        savingManager.SavePersistentData(persistentData);

        ClearBeforePrestige();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TryEraseSaveFile()
    {
        MessagesManager.Instance.DisplayConfirmQuestion("Are you certain?", "All progress will be lost! (this includes premium purchases)", //TODO-FIXME: In the future exclude premium purchases from file erasing
        () => {
            SavingManager.EraseSaveFile();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
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
    private void DebugProgression()
    {
        if (Math.Floor(Math.Log10(resourcesModel.earnedMoney)) > Math.Floor(Math.Log10(previousEarnedMoney)))
        {
            string s = string.Format("e{0:000} {1,6:N0}s {2,7:N0}s", Math.Floor(Math.Log10(resourcesModel.earnedMoney)), Math.Round(Time.time - previousMoneyProgressionDebugTime), Math.Round(Time.time));
            earnedMoneyMessages.Add(s);
            //earnedMoneyMessages.Add($"e{Math.Floor(Math.Log10(resourcesModel.earnedMoney))} - {Time.time - previousMoneyProgressionDebugTime} - {Time.time}");
            if (settingsModel.showProgressionDebugMessages)
            {
                Debug.Log(s);
                //Debug.Log($"e{Math.Floor(Math.Log10(resourcesModel.earnedMoney))} - {Time.time - previousMoneyProgressionDebugTime} - {Time.time}");
            }
            previousMoneyProgressionDebugTime = Time.time;
        }
        previousEarnedMoney = resourcesModel.earnedMoney;

        if (Math.Floor(gameModel.Depth / 100f) > Math.Floor(previousDepth) / 100f)
        {
            string s = string.Format("{0,5:N0}m {1,6:N0}s {2,7:N0}s", gameModel.Depth, Time.time - previousDepthProgressionDebugTime, Time.time);
            //depthMessages.Add($"{Math.Floor(gameModel.Depth)}d - {Time.time - previousDepthProgressionDebugTime} - {Time.time}");
            depthMessages.Add(s);
            if (settingsModel.showProgressionDebugMessages)
            {
                Debug.Log(s);
                //Debug.Log($"{Math.Floor(gameModel.Depth)}d - {Time.time - previousDepthProgressionDebugTime} - {Time.time}");
            }
            previousDepthProgressionDebugTime = Time.time;
        }
        previousDepth = gameModel.Depth;
    }
}
