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
    public double offlineEarnedMoney;
    #endregion

    #region BlocksModel
    public Vector2[] blocksPositions;
    #endregion

    #region UpgradesModel
    public PersistentUpgrade[] upgrades;
    #endregion

    #region GameModel
    public double depth;
    #endregion

    #region SettingsModel
    public bool is60fps;
    public bool displayFloatingText;
    #endregion

    #region StatisticsModel
    public int minedNormalBlocks;
    public int minedCopperBlocks;
    public int minedIronBlocks;
    public int minedGoldBlocks;
    public int minedDiamondBlocks;
    public int minedUraniumBlocks;
    #endregion

    #region AchievementsModel
    public PersistentAchievement[] unlockedAchievements;
    #endregion

    public PersistentData()
    {

    }
}