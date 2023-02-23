using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class RequirementsManager : MonoBehaviour
{
    #region Singleton
    public static RequirementsManager Instance;
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

    [SerializeField] private GameModel gameModel;
    [SerializeField] private BlocksModel blocksModel;
    [SerializeField] private ResourcesManager resourcesManager;
    [SerializeField] private UpgradesModel upgradesModel;

    public void ConnectRequirementToValueEvent(Requirement requirement)
    {
        switch (requirement.dependsOn)
        {
            case DependsOn.Depth:
                gameModel.onDepthChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.DestroyedBlocksCount:
                blocksModel.onDestroyedBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.Money:
                resourcesManager.onMoneyChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedMoney:
                resourcesManager.onMoneyEarned += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedNormalBlocks:
                StatisticsModel.Instance.onMinedNormalBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedCoalBlocks:
                StatisticsModel.Instance.onMinedCoalBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedCopperBlocks:
                StatisticsModel.Instance.onMinedCopperBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedIronBlocks:
                StatisticsModel.Instance.onMinedIronBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedGoldBlocks:
                StatisticsModel.Instance.onMinedGoldBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedSapphireBlocks:
                StatisticsModel.Instance.onMinedSapphireBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedRubyBlocks:
                StatisticsModel.Instance.onMinedRubyBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedDiamondBlocks:
                StatisticsModel.Instance.onMinedDiamondBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedUraniumBlocks:
                StatisticsModel.Instance.onMinedUraniumBlocksChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.PrestigeCurrency:
                resourcesManager.onPrestigeCurrencyChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedPrestigeCurrency:
                resourcesManager.onPrestigeCurrencyEarned += requirement.CheckIfFulfilled;
                break;
            case DependsOn.PremiumCurrency:
                resourcesManager.onPremiumCurrencyChange += requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedPremiumCurrency:
                resourcesManager.onPremiumCurrencyEarned += requirement.CheckIfFulfilled;
                break;
            case DependsOn.PowerUpTimeLeft:
                resourcesManager.onPowerUpTimeChanged += requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedPowerUpTime:
                resourcesManager.onPowerUpTimeEarned += requirement.CheckIfFulfilled;
                break;
            case DependsOn.UpgradeLevel:
                upgradesModel.upgrades[requirement.upgradeScriptable.Upgrade.GenerateName()].doUpgrade += requirement.CheckUpgradeLevel;
                break;
        }
    }

    public void DisconnectRequirementFromValueEvent(Requirement requirement)
    {
        switch (requirement.dependsOn)
        {
            case DependsOn.Depth:
                gameModel.onDepthChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.DestroyedBlocksCount:
                blocksModel.onDestroyedBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.Money:
                resourcesManager.onMoneyChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedMoney:
                resourcesManager.onMoneyEarned -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedNormalBlocks:
                StatisticsModel.Instance.onMinedNormalBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedCoalBlocks:
                StatisticsModel.Instance.onMinedCoalBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedCopperBlocks:
                StatisticsModel.Instance.onMinedCopperBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedIronBlocks:
                StatisticsModel.Instance.onMinedIronBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedGoldBlocks:
                StatisticsModel.Instance.onMinedGoldBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedSapphireBlocks:
                StatisticsModel.Instance.onMinedSapphireBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedRubyBlocks:
                StatisticsModel.Instance.onMinedRubyBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedDiamondBlocks:
                StatisticsModel.Instance.onMinedDiamondBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.MinedUraniumBlocks:
                StatisticsModel.Instance.onMinedUraniumBlocksChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.PrestigeCurrency:
                resourcesManager.onPrestigeCurrencyChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedPrestigeCurrency:
                resourcesManager.onPrestigeCurrencyEarned -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.PremiumCurrency:
                resourcesManager.onPremiumCurrencyChange -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedPremiumCurrency:
                resourcesManager.onPremiumCurrencyEarned -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.PowerUpTimeLeft:
                resourcesManager.onPowerUpTimeChanged -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.EarnedPowerUpTime:
                resourcesManager.onPowerUpTimeEarned -= requirement.CheckIfFulfilled;
                break;
            case DependsOn.UpgradeLevel:
                upgradesModel.upgrades[requirement.upgradeScriptable.Upgrade.GenerateName()].doUpgrade -= requirement.CheckUpgradeLevel;
                break;
        }
    }
}

public enum DependsOn
{
    Depth,
    DestroyedBlocksCount,
    Money,
    EarnedMoney,
    MinedNormalBlocks,
    MinedCopperBlocks,
    MinedIronBlocks,
    MinedGoldBlocks,
    MinedDiamondBlocks,
    MinedUraniumBlocks,
    PrestigeCurrency,
    EarnedPrestigeCurrency,
    PremiumCurrency,
    EarnedPremiumCurrency,
    PowerUpTimeLeft,
    EarnedPowerUpTime,
    UpgradeLevel,
    MinedCoalBlocks,
    MinedRubyBlocks,
    MinedSapphireBlocks,
}
public enum Comparison
{
    GT,
    GTEQ,
    EQ,
    LTEQ,
    LT,
}

[System.Serializable]
public class Requirement
{
    public DependsOn dependsOn;
    public Comparison comparison;
    public double requiredValue;
    [ConditionalField(nameof(dependsOn), false, DependsOn.UpgradeLevel)]
    public UpgradeScriptable upgradeScriptable;

    private bool isFulfilled = false;
    private bool IsFulfilled
    {
        set
        {
            if (value != isFulfilled)
            {
                isFulfilled = value;
                onStateChanged?.Invoke(isFulfilled);
            }
        }
    }
    public UnityAction<bool> onStateChanged;

    public void CheckIfFulfilled(double value)
    {
        double result = value - requiredValue;

        if (result <= 0 && comparison == Comparison.GT)
        {
            IsFulfilled = false;
            return;
        }
        if (result < 0 && comparison == Comparison.GTEQ)
        {
            IsFulfilled = false;
            return;
        }
        if (result != 0 && comparison == Comparison.EQ)
        {
            IsFulfilled = false;
            return;
        }
        if (result > 0 && comparison == Comparison.LTEQ)
        {
            IsFulfilled = false;
            return;
        }
        if (result >= 0 && comparison == Comparison.LT)
        {
            IsFulfilled = false;
            return;
        }

        IsFulfilled = true;
    }

    public void CheckUpgradeLevel(Upgrade upgrade)
    {
        CheckIfFulfilled(upgrade.currentLevel);
    }
}