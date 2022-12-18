using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Math;
using UnityEngine.Events;

using System;

/// <summary>
/// Methods from this object should not be called by other objects. When such action direction is needed (for example UI or world events) it should connect methods to events HERE.
/// </summary>
public class GameController : BaseController<GameView>
{

    [Header("Junkyard")]
    [SerializeField] private Data data;
    [SerializeField] private StatsDisplay statsDisplay;
    public Data NewData => data;

    [Header("<For replacement>")]
    public TMP_Text depthDisplay;
    public TMP_Text moneyDisplay;
    [Header("</For replacement>")]

    [Header("<For removal>")]
    public GameObject ShopMenu;
    public GameObject SettingsMenu;
    [Header("</For removal>")]

    [SerializeField] private Transform _dynamic;
    public Transform Dynamic => _dynamic;
    public Transform _dynamic_balls;
    public Transform _dynamic_blocks;
    public Transform ShopContent;
    public Transform SettingsContent;
    public UpgradeBuyingBar upgradeBuyingBar;
    public SettingBar settingBar;

    public Dictionary<string, UpgradeBuyingBar> buyingBars = new Dictionary<string, UpgradeBuyingBar>() { };

    public Vector2 spawnArea;

    [Header("TEMP")]
    [SerializeField] private BlockSpawner blockSpawner;

    [SerializeField] private BasicBallSpawner basicBallSpawner;
    [SerializeField] private BombBallSpawner bombBallSpawner;
    [SerializeField] private SniperBallSpawner sniperBallSpawner;

    [SerializeField] private UpgradesModel upgradesModel;
    [SerializeField] private GameObject movingBorderTexturesParent;

    private bool isMoving = true;

    //KEEP MONOBEHAVIOUR METHODS (Start, Update etc.) ON TOP
    void Start()
    {
        ConnectUpgrades();  //TODO-FT-CURRENT: Move this functionality to upgrade manager?

        CreateBallBars();
        InitBallBars();
        // TEMP
        // | | |
        // v v v
        HideMenus();
        LegacyGenerateBuyingBars();
        LegacyGenerateSettingBars();

        if (data.debugSettings)
        {
            data.money += data.additionalStartingMoney;   //TODO-FT-RESOURCES
            onMoneyChange.Invoke(data.money);
            data.roundNumber += data.additionalStartingRound;
            data.basicBallCount += data.additionalStartingBalls;
        }
    }

    private void Update()
    {
        DisplayFPS();

        var blocks = _dynamic_blocks.GetComponentsInChildren<BasicBlock>(false); //TODO: Very Temp
        MoveBlocks(blocks); // TODO: not optimal
        CheckIfWaveFinished(blocks); // TODO: not optimal

        DebugInputSwitch();
    }

    private void DebugInputSwitch()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            view.debugWindow.SetActive(!view.debugWindow.activeSelf);
        }
    }

    private void CreateBallBars()
    {
        foreach(var ballType in data.ballsData)
        {
            view.CreateBallBar(ballType);
        }
    }

    private void InitBallBars()
    {
        foreach(var ballBar in view.ballBars)
        {
            foreach(var buttonUpgrade in ballBar.buttonUpgrades)
            {
                var upgrade = upgradesModel.getUpgrade(buttonUpgrade.upgradeName);
                if(upgrade != null)
                {
                    buttonUpgrade.Init();
                    buttonUpgrade.onClick += upgrade.TryUpgrade;    //Button -> Tries to upgrade an Upgrade

                    buttonUpgrade.SetUpgradeCost(upgrade);   //Set initial values
                    onMoneyChange += buttonUpgrade.ChangeStateBasedOnMoney;

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

    private void ConnectUpgrades()
    {
        foreach(var upgrade in upgradesModel.upgrades)
        {
            upgrade.AddOnTryUpgrade(TryBuyUpgrade);
        }
    }

    private void MoveBlocks(BasicBlock[] blocks)
    {
        bool condition;
        if (isMoving)
        {
            condition = CheckForBlocksAboveY(blocks, 5f);
        } else
        {
            condition = CheckForBlocksAboveY(blocks, 4.5f);
        }

        if (!condition)
        {
            foreach (BasicBlock block in blocks)
            {
                block.transform.position += new Vector3(0, 5.0f, 0) * Time.deltaTime; // TODO: temp
            }
            var bgTextures = movingBorderTexturesParent.GetComponentsInChildren<Transform>(false); //TODO: Very Temp
            for (int i=1; i< bgTextures.Length; i++) // i=1 żeby nie łapało parenta
            {
                // Translate byłby zły bo parent ma scale i rotation
                bgTextures[i].position += new Vector3(0, 5.0f, 0) * Time.deltaTime ; // TODO: temp
                if (bgTextures[i].position.y >= 28f)
                {
                    bgTextures[i].position -= new Vector3(0, 16.8f, 0);
                }
            }
            isMoving = true;
        } else
        {
            isMoving = false;
        }
    }

    bool CheckForBlocksAboveY(BasicBlock[] blocks, float y)
    {
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y > y)
            {
                return true;
            }
        }
        return false;
    }


    bool CheckForBlocksBelowY(BasicBlock[] blocks, float y = -21)
    {
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y < y)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckIfWaveFinished(BasicBlock[] blocks)
    {
        if (!CheckForBlocksBelowY(blocks))
        {
            data.depth += data.depthPerWave;
            statsDisplay.SetWaveDisplay();
            DisplayWave();

            blockSpawner.SpawnBlockRow(out List <BasicBlock> spawnedBlocks);

            for (int i=0; i<spawnedBlocks.Count; i++)
            {
                spawnedBlocks[i].AssignEvents(OnBlockDestroyed);
            }
        }
    }

    private void OnBlockDestroyed(double money)
    {
        AddMoney(money);
        //TODO-FEATURE: Count destroyed blocks for upgrades/rewards

    }

    private int fpsRefreshCounter = 0;

    void DisplayFPS()
    {
        float current;
        if (fpsRefreshCounter == 0)
        {
            fpsRefreshCounter = 100;
            current = Time.frameCount / Time.time;
            view.avg_FpsDisplay.text = $"avg FPS: {Mathf.Round(current*10)/10f}";
            view.fpsDisplay.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";
        } else
        {
            fpsRefreshCounter--;
        }
    }

    void HideMenus()
    {
        ShopMenu.SetActive(false);
        SettingsMenu.SetActive(false);
    }

    public UnityAction<double> onMoneyChange;
    public void AddMoney(double amount)
    {
        if (amount < 1)
        {
            amount = 1;
        }
        data.money += amount;
        data.earnedMoney += amount;
        moneyDisplay.text = $"Money: {Round(data.money)}";

        onMoneyChange?.Invoke(data.money);
    }

    void LegacyGenerateBuyingBars()
    {
        foreach (KeyValuePair<string, Data.LegacyUpgrade> entry in data.legacyUpgrades)
        {
            UpgradeBuyingBar buying = Instantiate(upgradeBuyingBar, ShopContent);
            buying.upgradeName = entry.Key;
            buyingBars.Add(entry.Key, buying);
            LegacySetBuyingBarTexts(entry.Key);
            buying.GetComponentInChildren<ButtonBuyUpgrade>().gameController = this;
            
        }
        foreach (KeyValuePair<string, UpgradeBuyingBar> entry in buyingBars)
        {
            //print(entry.Key);
        }
    }

    void LegacyGenerateSettingBars()
    {
        foreach (KeyValuePair<string, bool> entry in data.settings)
        {
            SettingBar setting = Instantiate(settingBar, SettingsContent);
            setting.gameController = this;
            setting.settingName = entry.Key;
            if (PlayerPrefs.HasKey(entry.Key))
            {
                if (!IntToBool(PlayerPrefs.GetInt(entry.Key)))
                {
                    setting.ignore = 1;
                    setting.GetComponentInChildren<Toggle>().isOn = false;
                }

                // so this thing actually uses 60 fps if it's supposed to
                if (setting.settingName == "Display 60 FPS" && IntToBool(PlayerPrefs.GetInt(entry.Key))){
                    Application.targetFrameRate = 60;
                }
            }
        }
    }

    private double LegacyGetUpgradeCost(string name)
    {
        return data.legacyUpgrades[name].upgradeBaseCost * System.Math.Pow(data.legacyUpgrades[name].upgradeMultCost, data.legacyUpgrades[name].upgradeLevel);
    }

    public void LegacySetBuyingBarTexts(string name)
    {
        buyingBars[name].upgradeNameText.text = name;
        if (data.legacyUpgrades[name].upgradeMaxLevel == 0)
        {
            buyingBars[name].upgradeLevelsText.text = $"{data.legacyUpgrades[name].upgradeLevel}";
        }
        else
        {
            buyingBars[name].upgradeLevelsText.text = $"{data.legacyUpgrades[name].upgradeLevel}/{data.legacyUpgrades[name].upgradeMaxLevel}";
        }
        if (data.legacyUpgrades[name].upgradeMaxLevel > 0 && data.legacyUpgrades[name].upgradeLevel == data.legacyUpgrades[name].upgradeMaxLevel) //max level reached
        {
            if (!data.settings["Show maxed upgrades"])
            {
                buyingBars[name].gameObject.SetActive(false);
            }
            buyingBars[name].upgradeButton.SetActive(false);
        }
        else //can still be bought
        {
            buyingBars[name].upgradeButtonText.text = $"+1 level for {System.Math.Round(LegacyGetUpgradeCost(name), 2)}";
        }
    }

    private string boughtUpgradeName;   //TODO-FT-MVC
    public void LegacyBuyUpgradeByName(string name)
    {
        boughtUpgradeName = name;
        switch (name)
        {
            case "Damage":
                upgradesModel.getUpgrade("UniversalDamage").TryUpgrade();
                break;
            case "Bullet speed":
                upgradesModel.getUpgrade("UniversalSpeed").TryUpgrade();
                break;
            case var x when x.Contains("count"):
                switch (x)
                {
                    case "Bullet count":
                        upgradesModel.getUpgrade("BasicCount").TryUpgrade();
                        break;
                    case "Bomb count":
                        upgradesModel.getUpgrade("BombCount").TryUpgrade();
                        break;
                    case "Sniper count":
                        upgradesModel.getUpgrade("SniperCount").TryUpgrade();
                        break;
                }
                break;
        }
    }

    public void TryBuyUpgrade(Upgrade upgrade)
    {
        var upgradeCost = upgrade.cost;

        if(upgradeCost > data.money)    //TODO-FT-RESOURCES: Subtracting money should be as function for control
        {
            return;
        }

        data.money -= upgradeCost;  //TODO-FT-RESOURCES: Subtracting money should be as function for control
        onMoneyChange?.Invoke(data.money);

        //SetBuyingBarTexts(boughtUpgradeName);    //TODO-FT-MVC
        moneyDisplay.text = $"Money: {Round(data.money)}";  //TODO-FT-MVC
        statsDisplay.SetBallCountDisplay(); //TODO-FT-MVC

        upgrade.DoUpgrade();
    }

    public void LegacyBuyUpgrade(string name)
    {

        if (data.money >= LegacyGetUpgradeCost(name) && (data.legacyUpgrades[name].upgradeLevel < data.legacyUpgrades[name].upgradeMaxLevel || data.legacyUpgrades[name].upgradeMaxLevel == 0))
        {
            data.money -= LegacyGetUpgradeCost(name);
            data.legacyUpgrades[name].upgradeLevel += 1;
            LegacySetBuyingBarTexts(name);
            moneyDisplay.text = $"Money: {Round(data.money)}";
            if (name == "Damage")
            {
                statsDisplay.SetDamageDisplay();
            }
            if (name == "Bullet speed")
            {
                statsDisplay.SetSpdDisplay();
            }
            if (name.Contains("count"))
            {
                statsDisplay.SetBallCountDisplay();

                if (name == "Bullet count") {   //TODO: Change it from string to enum probably 
                    basicBallSpawner.Spawn();
                } else if (name == "Bomb count") {
                    bombBallSpawner.Spawn();
                } else if (name == "Sniper count") {
                    sniperBallSpawner.Spawn();
                }

                statsDisplay.SetBallCountDisplay();
            }
        }
        else if (data.money < LegacyGetUpgradeCost(name))
        {
            //print($"not enough money to buy {name}!");
        }
        else
        {
            //print($"upgrade {name} already at max level!");
        }
    }

    public void DisplayWave()
    {
        depthDisplay.text = $"Depth: {Math.Round(data.depth,1)}m";
    }

    public void ShowMenu(GameObject menu)
    {
        bool shouldBeShown = true;
        if (menu.activeInHierarchy)
        {
            shouldBeShown = false;
        }
        HideMenus();
        if (shouldBeShown)
        {
            menu.SetActive(true);
        }
    }

    public int BoolToInt(bool value)
    {
        if (value)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public bool IntToBool(int value)
    {
        if (value == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChangeSetting(bool value, string settingName)
    {
        data.settings[settingName] = value;
        PlayerPrefs.SetInt(settingName, BoolToInt(value));
        PlayerPrefs.Save();
        if (settingName == "Show maxed upgrades")
        {
            foreach (KeyValuePair<string, UpgradeBuyingBar> entry in buyingBars)
            {
                if (data.legacyUpgrades[entry.Key].upgradeMaxLevel > 0 && data.legacyUpgrades[entry.Key].upgradeLevel == data.legacyUpgrades[entry.Key].upgradeMaxLevel)
                {
                    buyingBars[entry.Key].gameObject.SetActive(value);
                }
            }
        }
        if (settingName == "Show floating damage text")
        {
            data.displayFloatingText = value;
        }
        if (settingName == "Display 60 FPS")
        {
            if (value)
            {
                Application.targetFrameRate = 60;
            } else
            {
                Application.targetFrameRate = 30;
            }
        }
    }
}
