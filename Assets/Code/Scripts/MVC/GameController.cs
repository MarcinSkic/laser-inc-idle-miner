using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Math;
using UnityEngine.Events;

using System; // TODO@MARTIN w czym jest Math??

/// <summary>
/// Methods from this object should not be called by other objects. When such action direction is needed (for example UI or world events) it should connect methods to events HERE.
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private Data data;
    [SerializeField] private StatsDisplay statsDisplay;
    public Data NewData => data;

    public TMP_Text healthDisplay;
    public TMP_Text waveDisplay;
    public TMP_Text moneyDisplay;
    public TMP_Text avgFpsDisplay;
    public TMP_Text fpsDisplay;
    public BasicBlock basicBlock;
    public Slider healthBar;
    public GameObject ShopMenu;
    public GameObject SettingsMenu;
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

    private void Update()
    {
        displayFPS();
        var blocks = _dynamic_blocks.GetComponentsInChildren<BasicBlock>(false); //TODO: Very Temp
        MoveBlocks(blocks); // TODO: not optimal
        checkIfWaveFinished(blocks); // TODO: not optimal
    }

    private void MoveBlocks(BasicBlock[] blocks)
    {
        if (!CheckForBlocksAboveY(blocks))
        {
            foreach (BasicBlock block in blocks)
            {
                block.transform.position += new Vector3(0, 5.0f, 0) * Time.deltaTime; // TODO: temp
            }
        }
    }

    bool CheckForBlocksAboveY(BasicBlock[] blocks, float y = 5)
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


    bool CheckForBlocksBelowY(BasicBlock[] blocks, float y = -12)
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

    private void checkIfWaveFinished(BasicBlock[] blocks)
    {
        if (!CheckForBlocksBelowY(blocks))
        {
            data.depth += data.depthPerWave;
            statsDisplay.SetWaveDisplay();
            DisplayWave();

            blockSpawner.SpawnBlockRow(out BasicBlock[] spawnedBlocks);

            for (int i=0; i<spawnedBlocks.Length; i++)
            {
                spawnedBlocks[i].AssignEvents(OnBlockDestroyed);
            }
        }
    }

    private void OnBlockDestroyed(double maxHp)
    {
        AddMoney(maxHp);
        //TODO-FEATURE: Count destroyed blocks for upgrades/rewards

    }

    private int avgFrameRate;
    private int fpsRefreshCounter = 0;

    void displayFPS()
    {
        float current = 0;
        if (fpsRefreshCounter == 0)
        {
            fpsRefreshCounter = 100;
            current = Time.frameCount / Time.time;
            avgFpsDisplay.text = $"avg FPS: {Mathf.Round(current*10)/10f}";
            fpsDisplay.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";
        } else
        {
            fpsRefreshCounter--;
        }
    }

    void Start()
    {
        HideMenus();
        GenerateBuyingBars();
        GenerateSettingBars();
        SetupTestUpgrade();
    }

    void SetupTestUpgrade()
    {
        //data.speedUpgrade = data.speedUpgradeScriptable.GetObjectCopy();
        //data.speedUpgrade.AddUpgradeable(data.basicBallData.speed);


    }

    void HideMenus()
    {
        ShopMenu.SetActive(false);
        SettingsMenu.SetActive(false);
    }

    public void AddMoney(double amount)
    {
        data.money += amount;
        data.earnedMoney += amount;
        moneyDisplay.text = $"Money: {Round(data.money)}";
    }

    void GenerateBuyingBars()
    {
        foreach (KeyValuePair<string, Data.LegacyUpgrade> entry in data.upgrades)
        {
            UpgradeBuyingBar buying = Instantiate(upgradeBuyingBar, ShopContent);
            buying.upgradeName = entry.Key;
            buyingBars.Add(entry.Key, buying);
            SetBuyingBarTexts(entry.Key);
            buying.GetComponentInChildren<ButtonBuyUpgrade>().gameController = this;
            
        }
        foreach (KeyValuePair<string, UpgradeBuyingBar> entry in buyingBars)
        {
            //print(entry.Key);
        }
    }

    void GenerateSettingBars()
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

    private double Cost(string name)
    {
        return data.upgrades[name].upgradeBaseCost * System.Math.Pow(data.upgrades[name].upgradeMultCost, data.upgrades[name].upgradeLevel);
    }

    public void SetBuyingBarTexts(string name)
    {
        buyingBars[name].upgradeNameText.text = name;
        if (data.upgrades[name].upgradeMaxLevel == 0)
        {
            buyingBars[name].upgradeLevelsText.text = $"{data.upgrades[name].upgradeLevel}";
        }
        else
        {
            buyingBars[name].upgradeLevelsText.text = $"{data.upgrades[name].upgradeLevel}/{data.upgrades[name].upgradeMaxLevel}";
        }
        if (data.upgrades[name].upgradeMaxLevel > 0 && data.upgrades[name].upgradeLevel == data.upgrades[name].upgradeMaxLevel) //max level reached
        {
            if (!data.settings["Show maxed upgrades"])
            {
                buyingBars[name].gameObject.SetActive(false);
            }
            buyingBars[name].upgradeButton.SetActive(false);
        }
        else //can still be bought
        {
            buyingBars[name].upgradeButtonText.text = $"+1 level for {System.Math.Round(Cost(name), 2)}";
        }
    }

    private string boughtUpgradeName;   //TODO-FT-MVC
    public void BuyUpgradeByName(string name)
    {
        boughtUpgradeName = name;
        switch (name)
        {
            case "Damage":
                TryBuyUpgrade(upgradesModel.universalDamage.upgrade);
                break;
            case "Bullet speed":
                TryBuyUpgrade(upgradesModel.universalSpeed.upgrade);
                break;
            case var x when x.Contains("count"):
                switch (x)
                {
                    case "Bullet count":
                        TryBuyUpgrade(upgradesModel.basicBallCount.upgrade);
                        break;
                    case "Bomb count":
                        TryBuyUpgrade(upgradesModel.bombBallCount.upgrade);
                        break;
                    case "Sniper count":
                        TryBuyUpgrade(upgradesModel.sniperBallCount.upgrade);
                        break;
                }
                break;
        }
    }

    public bool TryBuyUpgrade<T>(Upgrade<T> upgrade)
    {
        var upgradeCost = upgrade.currentCost;

        if(upgradeCost > data.money)
        {
            return false;
        }

        if(!upgrade.TryUpgrade(out var newCost))
        {
            return false;
        }
        data.money -= upgradeCost;

        SetBuyingBarTexts(boughtUpgradeName);    //TODO-FT-MVC
        moneyDisplay.text = $"Money: {Round(data.money)}";  //TODO-FT-MVC
        statsDisplay.SetBallCountDisplay(); //TODO-FT-MVC

        return true;
    }

    public void LegacyBuyUpgrade(string name)
    {

        if (data.money >= Cost(name) && (data.upgrades[name].upgradeLevel < data.upgrades[name].upgradeMaxLevel || data.upgrades[name].upgradeMaxLevel == 0))
        {
            data.money -= Cost(name);
            data.upgrades[name].upgradeLevel += 1;
            SetBuyingBarTexts(name);
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

            var balls = _dynamic_balls.GetComponentsInChildren<BaseBall<BaseBallData>>(true);    //TODO-FT-UPGRADES
            foreach (var ball in balls)
            {
                ball.Upgrade();
            }
        }
        else if (data.money < Cost(name))
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
        waveDisplay.text = $"Depth: {Math.Round(data.depth,1)}m";
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
                if (data.upgrades[entry.Key].upgradeMaxLevel > 0 && data.upgrades[entry.Key].upgradeLevel == data.upgrades[entry.Key].upgradeMaxLevel)
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
