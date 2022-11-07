using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static System.Math;

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

    private void Update()
    {
        displayFPS();
        if (_dynamic_blocks.childCount == 0)
        {
            data.wave++;
            statsDisplay.SetWaveDisplay();
            DisplayWave();
            for (int i=0; i<Random.Range(200, 250); i++)
            {
                SpawnNormalBlock();
            }
        }
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
        //ClearPlayerPrefs();
        HideMenus();
        GenerateBuyingBars();
        GenerateSettingBars();
    }

    void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("Show maxed upgrades");
        PlayerPrefs.DeleteKey("Show floating damage text");
        PlayerPrefs.Save();
    }

    void HideMenus()
    {
        ShopMenu.SetActive(false);
        SettingsMenu.SetActive(false);
    }

    public void AddMoney(double amount)
    {
        data.money += amount;
        data.accumulated += amount;
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
        //print(buyingBars.Count);
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
        //print($"you tried to buy {name} upgrade");
        if (data.money >= Cost(name) && (data.upgrades[name].upgradeLevel < data.upgrades[name].upgradeMaxLevel || data.upgrades[name].upgradeMaxLevel == 0))
        {
            //print($"you have enough money to buy {name}");
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
            //if (name == "Health")
            //{
                //data.health += data.healthPerUpgrade;
              //  data.maxHealth += data.healthPerUpgrade;
            //}
            if (name.Contains("count"))
            {
                statsDisplay.SetBallCountDisplay();
                Vector3 spawnLocation = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
                BasicBall balltype = basicBall;
                if (name == "Bullet count") {
                    data.basicBulletCount++;
                } else if (name == "Bomb count") {
                    data.bombBulletCount++;
                    balltype = bombBall;
                } else if (name == "Sniper count") {
                    data.sniperBulletCount++;
                    balltype = sniperBall;
                }
                statsDisplay.SetBallCountDisplay();
                BasicBall spawning = Instantiate(balltype, spawnLocation, Quaternion.identity, _dynamic_balls);
                spawning.gameController = this;
                spawning.data = data;
                spawning.rb = spawning.GetComponent<Rigidbody>();
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

    /*    private void SpawnNormalEnemy()
        {
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnLocation = new Vector3(spawnDirection.x * data.spawnDistance, 0, spawnDirection.y * data.spawnDistance);
            BasicEnemy spawning = Instantiate(basicEnemy, spawnLocation, Quaternion.LookRotation(tower.transform.position - spawnLocation, new Vector3(0, 1, 0)), _dynamic);
            spawning.speed = data.GetWaveEnemiesSpeed();
            spawning.dps = data.GetWaveEnemiesDps();
            spawning.hp = data.GetWaveEnemiesHealth();
            spawning.moneyGiven = data.GetWaveEnemiesMoney();
            spawning.SetData(data);
            spawning.SetGameController(this);
        }*/

    // CO JA TU POROBIŁEM XD

    /*private void SpawnNormalBlocks()
    {
        Vector3 spawnLocation;
        while (true)
        {
            bool tooClose = false;
            spawnLocation = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
            for (int i = 0; i < _dynamic_blocks.childCount; i++)
            {
                BasicBlock block = _dynamic_blocks.GetChild(i).GetComponent<BasicBlock>();
                if (Vector3.Distance(block.GetComponent<BoxCollider>().ClosestPoint(spawnLocation), spawnLocation) < 3)
                {
                    tooClose = true;
                    break;
                }
            }
            if (!tooClose)
            {
                break;
            }
        }
        Vector3[] offsetVectors = { Vector3.down, Vector3.right };
        int vectorNumber = Random.Range(0, 2);
        Vector3 offsetVector = offsetVectors[vectorNumber];
        float offsetDistance = Random.Range(0.69f, 1f);
        if (vectorNumber == 1)
        {
            offsetDistance *= 1.62f;
        }
        float clusteredBlocksCount = Random.Range(4, 11);
        if ((spawnLocation + offsetVector * offsetDistance * clusteredBlocksCount).x >= -spawnArea.x && (spawnLocation + offsetVector * offsetDistance * clusteredBlocksCount).y >= -spawnArea.y)
        {
            spawnLocation -= (offsetVector * offsetDistance * (clusteredBlocksCount));
        }
        Debug.LogWarning(vectorNumber);
        Debug.Log(offsetVector * offsetDistance * (clusteredBlocksCount));
        for (int i=0; i<clusteredBlocksCount; i++)
        {
            BasicBlock spawning = Instantiate(basicBlock, spawnLocation+(offsetVector*offsetDistance*i), Quaternion.identity, _dynamic_blocks);
            spawning.hp = data.GetWaveEnemiesHealth();
            spawning.maxHp = spawning.hp;
            spawning.data = data;
            spawning.gameController = this;
        }
    }*/

    private void SpawnNormalBlock()
    {

        Vector3 spawnLocation = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
        BasicBlock spawning = Instantiate(basicBlock, spawnLocation, Quaternion.identity, _dynamic_blocks);
        spawning.hp = data.GetWaveEnemiesHealth();
        spawning.maxHp = spawning.hp;
        spawning.data = data;
        spawning.gameController = this;
    }

    public void DisplayWave()
    {
        waveDisplay.text = $"Wave: {data.wave}";
    }

/*    public void DealDamage(double damage)
    {
        data.health -= damage;
    }*/

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
            //print($"menu shown: {menu}");
        }
        else
        {
            //print($"menu: {menu} is now hidden");
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
    }
}
