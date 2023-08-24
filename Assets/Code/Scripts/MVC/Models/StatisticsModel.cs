using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class StatisticsModel : MonoBehaviour
{
    #region Singleton
    public static StatisticsModel Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion


    #region minedNormalBlocks
    [ReadOnly] [SerializeField] private int minedNormalBlocks;
    public int MinedNormalBlocks
    {
        get
        {
            return minedNormalBlocks;
        }
        set
        {
            minedNormalBlocks = value;
            onMinedNormalBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedNormalBlocksChange;
    #endregion
    #region minedCoalBlocks
    [ReadOnly] [SerializeField] private int minedCoalBlocks;
    public int MinedCoalBlocks
    {
        get
        {
            return minedCoalBlocks;
        }
        set
        {
            minedCoalBlocks = value;
            onMinedCoalBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedCoalBlocksChange;
    #endregion
    #region minedCopperBlocks
    [ReadOnly] [SerializeField] private int minedCopperBlocks;
    public int MinedCopperBlocks
    {
        get
        {
            return minedCopperBlocks;
        }
        set
        {
            minedCopperBlocks = value;
            onMinedCopperBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedCopperBlocksChange;
    #endregion
    #region minedIronBlocks
    [ReadOnly] [SerializeField] private int minedIronBlocks;
    public int MinedIronBlocks
    {
        get
        {
            return minedIronBlocks;
        }
        set
        {
            minedIronBlocks = value;
            onMinedIronBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedIronBlocksChange;
    #endregion
    #region minedGoldBlocks
    [ReadOnly] [SerializeField] private int minedGoldBlocks;
    public int MinedGoldBlocks
    {
        get
        {
            return minedGoldBlocks;
        }
        set
        {
            minedGoldBlocks = value;
            onMinedGoldBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedGoldBlocksChange;
    #endregion
    #region minedSapphireBlocks
    [ReadOnly] [SerializeField] private int minedSapphireBlocks;
    public int MinedSapphireBlocks
    {
        get
        {
            return minedSapphireBlocks;
        }
        set
        {
            minedSapphireBlocks = value;
            onMinedSapphireBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedSapphireBlocksChange;
    #endregion
    #region minedRubyBlocks
    [ReadOnly] [SerializeField] private int minedRubyBlocks;
    public int MinedRubyBlocks
    {
        get
        {
            return minedRubyBlocks;
        }
        set
        {
            minedRubyBlocks = value;
            onMinedRubyBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedRubyBlocksChange;
    #endregion
    #region minedDiamondBlocks
    [ReadOnly] [SerializeField] private int minedDiamondBlocks;
    public int MinedDiamondBlocks
    {
        get
        {
            return minedDiamondBlocks;
        }
        set
        {
            minedDiamondBlocks = value;
            onMinedDiamondBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedDiamondBlocksChange;
    #endregion
    #region minedUraniumBlocks
    [ReadOnly] [SerializeField] private int minedUraniumBlocks;
    public int MinedUraniumBlocks
    {
        get
        {
            return minedUraniumBlocks;
        }
        set
        {
            minedUraniumBlocks = value;
            onMinedUraniumBlocksChange?.Invoke(value);
        }
    }
    public UnityAction<double> onMinedUraniumBlocksChange;
    #endregion
    #region caughtBats
    [ReadOnly] [SerializeField] private int caughtBats;
    public int CaughtBats
    {
        get
        {
            return caughtBats;
        }
        set
        {
            caughtBats = value;
            onCaughtBatsChange?.Invoke(value);
        }
    }
    public UnityAction<double> onCaughtBatsChange;
    #endregion
    #region achievementsCount
    [ReadOnly] [SerializeField] private int achievementsCount;
    public int AchievementsCount
    {
        get
        {
            return achievementsCount;
        }
        set
        {
            achievementsCount = value;
            onAchievementsCountChange?.Invoke(value);
        }
    }
    public UnityAction<double> onAchievementsCountChange;
    #endregion

    public void SavePersistentData(PersistentData data)
    {
        data.minedNormalBlocks = MinedNormalBlocks;
        data.minedCoalBlocks = MinedCoalBlocks;
        data.minedCopperBlocks = MinedCopperBlocks;
        data.minedIronBlocks = MinedIronBlocks;
        data.minedGoldBlocks = MinedGoldBlocks;
        data.minedSapphireBlocks = MinedSapphireBlocks;
        data.minedRubyBlocks = MinedRubyBlocks;
        data.minedDiamondBlocks = MinedDiamondBlocks;
        data.minedUraniumBlocks = MinedUraniumBlocks;
        data.caughtBats = CaughtBats;
    }

    public void LoadPersistentData(PersistentData data)
    {
        MinedNormalBlocks = (int)(data?.minedNormalBlocks);
        MinedCoalBlocks = (int)(data?.minedCoalBlocks);
        MinedCopperBlocks = (int)(data?.minedCopperBlocks);
        MinedIronBlocks = (int)(data?.minedIronBlocks);
        MinedGoldBlocks = (int)(data?.minedGoldBlocks);
        MinedSapphireBlocks = (int)(data?.minedSapphireBlocks);
        MinedRubyBlocks = (int)(data?.minedRubyBlocks);
        MinedDiamondBlocks = (int)(data?.minedDiamondBlocks);
        MinedUraniumBlocks = (int)(data?.minedUraniumBlocks);
        CaughtBats = (int)(data?.caughtBats);
        AchievementsCount = data?.unlockedAchievements.Length ?? 0;
        Debug.Log(AchievementsCount);
    }
}
