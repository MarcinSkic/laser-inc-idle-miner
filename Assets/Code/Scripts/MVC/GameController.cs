using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Math;
using UnityEngine.Events;

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
    public BasicBall basicBall;
    public BasicBall bombBall;
    public BasicBall sniperBall;
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

    private void Update()
    {
        displayFPS();
        checkIfWaveFinished();  //TODO: Make it event activated for each block broken
    }

    private void checkIfWaveFinished(){
        var blocks = _dynamic_blocks.GetComponentsInChildren<BasicBlock>(false);    //TODO: Very Temp
        if (blocks.Length == 0)
        {
            data.wave++;
            statsDisplay.SetWaveDisplay();
            DisplayWave();
            for (int i=0; i<Random.Range(80, 120); i++)
            {
                var block = blockSpawner.Spawn();
                block.AssignEvents(OnBlockDestroyed);
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
        foreach (KeyValuePair<string, Data.Upgrade> entry in data.upgrades)
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

    public void BuyUpgrade(string name)
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

            var balls = _dynamic_balls.GetComponentsInChildren<BasicBall>(true);    //TODO-HOTFIX
            foreach (var ball in balls)
            {
                ball.UpgradeBall(data.GetSpd(), data.GetBallDamage());
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
        waveDisplay.text = $"Wave: {data.wave}";
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
