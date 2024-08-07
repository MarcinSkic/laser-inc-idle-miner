using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingModel : MonoBehaviour
{
}

[System.Serializable]
public class PersistentData
{
    #region General
    public DateTime lastSaveTime;
    #endregion

    #region ResourceModel
    public double money;
    public double earnedMoney;
    public double undecreasableEarnedMoney;
    public double offlineEarnedMoney;
    public double afkGainPerSec;
    public List<double> lastOnlineEarnedMoneyStates;

    public double premiumCurrency;
    public double earnedPremiumCurrency;
    public double prestigeCurrency;
    public double earnedPrestigeCurrency;

    public double powerUpTime;
    public double earnedPowerUpTime;
    #endregion

    #region BlocksModel
    public Vector2[] blocksPositions;
    public string[] blocksMaterials;
    #endregion

    #region UpgradesModel
    public PersistentUpgrade[] upgrades;
    #endregion

    #region GameModel
    public double depth;
    public bool visitedDyson;
    #endregion

    #region SettingsModel
    public bool playMusic;
    public bool playSounds;
    public bool is60fps;
    public bool displayFloatingText;
    public bool useAlternativeNotation;
    public string username;
    #endregion

    #region StatisticsModel
    public int minedNormalBlocks;
    public int minedCoalBlocks;
    public int minedCopperBlocks;
    public int minedIronBlocks;
    public int minedGoldBlocks;
    public int minedSapphireBlocks;
    public int minedRubyBlocks;
    public int minedDiamondBlocks;
    public int minedUraniumBlocks;
    public int caughtBats;
    public int executedPrestigesCount;
    #endregion

    #region AchievementsModel
    public PersistentAchievement[] unlockedAchievements;
    #endregion

    #region TutorialModel
    public bool finishedTutorial;
    #endregion

    #region Daily
    public string last_reward_time;
    public int consecutive_rewards_count;
    #endregion

    public PersistentData()
    {

    }
}