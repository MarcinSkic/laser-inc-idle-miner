using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Data : MonoBehaviour
{
    [Header("base stats")]
    public double bulletBaseDamage;
    public double bulletSpeed;
    public int basicBulletCount;
    public int bombBulletCount;
    public int sniperBulletCount;

    [Header("initial stats")]
    public double money;
    public double accumulated;
    public double wave;

    [Header("rounds stuff")]
    public int roundNumber;
    public double roundTime;


    [Header("Upgrade bonuses")]
    public double dmgPerUpgrade;
    public double bulletSpdPerUpgrade;

    [Header("Debug")]
    public bool debugSettings;
    public double additionalStartingMoney;
    public int additionalStartingRound;
    public int additionalStartingBalls;

    [Header("Settings?")]
    public bool displayFloatingText;


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
            basicBulletCount += additionalStartingBalls;
        }
    }

    public Dictionary<string, bool> settings = new Dictionary<string, bool>() { };

    public class Upgrade
    {
        public double upgradeBaseCost;
        public double upgradeMultCost;
        public int upgradeMaxLevel;
        public int upgradeLevel = 0;
    }

    public Dictionary<string, Upgrade> upgrades = new Dictionary<string, Upgrade>() { };

    void Awake()
    {
        upgrades.Add("Damage", new Upgrade() { upgradeBaseCost = 1, upgradeMultCost = 1.4, upgradeMaxLevel = 0 });
        upgrades.Add("Bullet speed", new Upgrade() { upgradeBaseCost = 1, upgradeMultCost = 1.4, upgradeMaxLevel = 0 });
        upgrades.Add("Bullet count", new Upgrade() { upgradeBaseCost = 5, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        upgrades.Add("Bomb count", new Upgrade() { upgradeBaseCost = 20, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
        upgrades.Add("Sniper count", new Upgrade() { upgradeBaseCost = 20, upgradeMultCost = 1.7, upgradeMaxLevel = 10 });
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
    public double GetWaveEnemiesSpeed()
    {
        return 4 + wave / 3;
    }
    public double GetWaveEnemiesMoney()
    {
        return wave;
    }
    public double GetBulletDamage()
    {
        return bulletBaseDamage + upgrades["Damage"].upgradeLevel * dmgPerUpgrade;
    }
    public double GetSpd()
    {
        return bulletSpeed + bulletSpdPerUpgrade * upgrades["Bullet speed"].upgradeLevel;
    }
}
