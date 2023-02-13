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

    public void SavePersistentData(PersistentData data)
    {
        data.minedNormalBlocks = MinedNormalBlocks;
        data.minedCopperBlocks = MinedCopperBlocks;
        data.minedIronBlocks = MinedIronBlocks;
        data.minedGoldBlocks = MinedGoldBlocks;
        data.minedDiamondBlocks = MinedDiamondBlocks;
        data.minedUraniumBlocks = MinedUraniumBlocks;
    }

    public void LoadPersistentData(PersistentData data)
    {
        MinedNormalBlocks = (int)(data?.minedNormalBlocks);
        MinedCopperBlocks = (int)(data?.minedCopperBlocks);
        MinedIronBlocks = (int)(data?.minedIronBlocks);
        MinedGoldBlocks = (int)(data?.minedGoldBlocks);
        MinedDiamondBlocks = (int)(data?.minedDiamondBlocks);
        MinedUraniumBlocks = (int)(data?.minedUraniumBlocks);
    }
}
