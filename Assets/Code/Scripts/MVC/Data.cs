using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using MyBox;

public class Data : MonoBehaviour
{
    [Header("BALLS STATS")]

    [Header("Universal")]

    public List<BallData> ballsDataList;
    public Dictionary<UpgradeableObjects, BallData> ballsData;

    [Space(10)]

    [Header("PLAYER STATS")]
    public double legacyMoney;
    public double legacyEarnedMoney;

    [Space(10)]

    [Header("GAME STATS")]

    //TODO-FT-SAVEFILE: change to property with set protection that while it is lower than active pool balls it should spawn more
    [ReadOnly]
    public int basicBallCount;
    [ReadOnly]
    public int bombBallCount;
    [ReadOnly]
    public int sniperBallCount;

    [Header("Rounds stats")]

    public double depthPerWave;
    public double depth;
    public int roundNumber;
    public double roundTime;

    [Space(10)]

    [Header("DEBUG")]
    public double depthOffset = -18;
    [ReadOnly]
    public double realDepth = 0;
    
    [Foldout("UNIMPLEMENTED",true)]
    public int additionalStartingRound;
    public int additionalStartingBalls;

    void Awake()
    {
        CreateListOfBalls();
        //realDepth += depthOffset;

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

    private void CreateListOfBalls()
    {
        ballsData = new Dictionary<UpgradeableObjects, BallData>() { };

        foreach (var data in ballsDataList)
        {
            data.Init();
            ballsData.Add(data.type, data);
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

    public Dictionary<string, LegacyUpgrade> legacyUpgrades = new Dictionary<string, LegacyUpgrade>() { };



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
}
