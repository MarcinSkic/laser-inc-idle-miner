using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Data : MonoBehaviour
{
    [Header("BALLS STATS")]

    [Header("Universal")]
    [HideInInspector]
    public List<BaseBallData> ballsData;    //Every new ball must be added here (CreateListOfBalls method)

    public BasicBallData basicBallData;
    public BombBallData bombBallData;
    public SniperBallData sniperBallData;


    [Space(10)]

    [Header("PLAYER STATS")]
    public double money;
    public double earnedMoney;

    [Space(10)]

    [Header("GAME STATS")]

    //TODO-FT-SAVEFILE: change to property with set protection that while it is lower than active pool balls it should spawn more
    public int basicBallCount;  
    public int bombBallCount;
    public int sniperBallCount;

    [Header("Rounds stats")]

    public double depthPerWave;
    public double depth;
    public int roundNumber;
    public double roundTime;

    [Space(10)]

    [Header("SETTINGS")]
    public bool displayFloatingText;

    [Space(10)]

    [Header("DEBUG")]
    public bool debugSettings;
    public double additionalStartingMoney;
    

    [Header("UNIMPLEMENTED")]
    public int additionalStartingRound;
    public int additionalStartingBalls;
    public bool displayFpsStats;


    private void Start()
    {
        CreateListOfBalls();

        if (PlayerPrefs.HasKey("Show floating damage text"))
        {
            displayFloatingText = IntToBool(PlayerPrefs.GetInt("Show floating damage text"));
        }
        if (debugSettings)
        {
            money += additionalStartingMoney;
            roundNumber += additionalStartingRound;
            basicBallCount += additionalStartingBalls;
        }
    }

    private void CreateListOfBalls()
    {
        ballsData = new List<BaseBallData>{ basicBallData,bombBallData,sniperBallData};
    }

    public Dictionary<string, bool> settings = new Dictionary<string, bool>() { };

    public class LegacyUpgrade
    {
        public double upgradeBaseCost;
        public double upgradeMultCost;
        public int upgradeMaxLevel;
        public int upgradeLevel = 0;
    }

    public Dictionary<string, LegacyUpgrade> legacyUpgrades = new Dictionary<string, LegacyUpgrade>() { };

    void Awake()
    {
        legacyUpgrades.Add("Damage", new LegacyUpgrade() { upgradeBaseCost = 1, upgradeMultCost = 1.4, upgradeMaxLevel = 0 });
        legacyUpgrades.Add("Bullet speed", new LegacyUpgrade() { upgradeBaseCost = 1, upgradeMultCost = 1.4, upgradeMaxLevel = 0 });
        legacyUpgrades.Add("Bullet count", new LegacyUpgrade() { upgradeBaseCost = 5, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        legacyUpgrades.Add("Bomb count", new LegacyUpgrade() { upgradeBaseCost = 20, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        legacyUpgrades.Add("Sniper count", new LegacyUpgrade() { upgradeBaseCost = 20, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        string[] settingsArray = { "Show maxed upgrades", "Show floating damage text", "Display 60 FPS" };
        foreach (string setting in settingsArray)
        {
            if (PlayerPrefs.HasKey(setting))
            {
                settings.Add(setting, IntToBool(PlayerPrefs.GetInt(setting)));
            }
            else
            {
                settings.Add(setting, true);
            }
        }
    }

    private bool IntToBool(int value)
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
    public double GetDepthBlocksHealth()
    {
        /*
        if (wave < 4)
        {
            return wave;
        }
        if (wave == 4)
        {
            return 5;
        }
        return Math.Ceiling(wave*Math.Pow(1.07, wave));
        */
        return depth*Math.Pow(1.03, depth);
    }
    public double GetBallDamage()
    {
        return basicBallData.damage + legacyUpgrades["Damage"].upgradeLevel * 1;
    }
    public double GetSpd()
    {
        return basicBallData.speed + 1 * legacyUpgrades["Bullet speed"].upgradeLevel;
    }
}
