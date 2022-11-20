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
    public BasicBallData basicBallData;
    public double ballDamage;
    public double ballSpeed;

    public BombBallData bombBallData;

    public SniperBallData sniperBallData;

    [Space(10)]

    [Header("PLAYER STATS")]
    public double money;
    public double earnedMoney;

    [Header("Upgrades")]
    public double dmgPerUpgrade;
    public double bulletSpdPerUpgrade;

    [Space(10)]

    [Header("GAME STATS")]

    //TODO-FT-SAVEFILE: change to property with set protection that while it is lower than active pool balls it should spawn more
    public int basicBallCount;  
    public int bombBallCount;
    public int sniperBallCount;

    [Header("Rounds stats")]

    public double wave;
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



    public Dictionary<string, bool> settings = new Dictionary<string, bool>() { };

    public class LegacyUpgrade
    {
        public double upgradeBaseCost;
        public double upgradeMultCost;
        public int upgradeMaxLevel;
        public int upgradeLevel = 0;
    }

    public Dictionary<string, LegacyUpgrade> upgrades = new Dictionary<string, LegacyUpgrade>() { };

    void Awake()
    {
        upgrades.Add("Damage", new LegacyUpgrade() { upgradeBaseCost = 1, upgradeMultCost = 1.4, upgradeMaxLevel = 0 });
        upgrades.Add("Bullet speed", new LegacyUpgrade() { upgradeBaseCost = 1, upgradeMultCost = 1.4, upgradeMaxLevel = 0 });
        upgrades.Add("Bullet count", new LegacyUpgrade() { upgradeBaseCost = 5, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        upgrades.Add("Bomb count", new LegacyUpgrade() { upgradeBaseCost = 20, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        upgrades.Add("Sniper count", new LegacyUpgrade() { upgradeBaseCost = 20, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
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
    public double GetWaveEnemiesHealth()
    {
        if (wave < 4)
        {
            return wave;
        }
        if (wave == 4)
        {
            return 5;
        }
        return Math.Ceiling(wave*Math.Pow(1.07, wave));
    }
    public double GetBallDamage()
    {
        return ballDamage + upgrades["Damage"].upgradeLevel * dmgPerUpgrade;
    }
    public double GetSpd()
    {
        return ballSpeed + bulletSpdPerUpgrade * upgrades["Bullet speed"].upgradeLevel;
    }
}
