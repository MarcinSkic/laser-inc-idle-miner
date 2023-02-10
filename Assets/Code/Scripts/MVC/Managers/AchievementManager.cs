using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    powerUpTimeLeft,
    earnedPowerUpTime,
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
public class AchievementRequirement
{
    public DependsOn dependsOn;
    public Comparison comparison;
    public double requiredValue;
}

[System.Serializable]
public class Achievement
{
    public AchievementManager achievementManager;
    public string name;
    public string description;
    public Sprite sprite;
    public bool isCompleted = false;
    [SerializeField]
    public AchievementRequirement[] requirements;

    public void checkIfCompleted(double _)
    {
        for (int i = 0; i < requirements.Length; i++)
        {
            double comparedValue = 0;
            switch (requirements[i].dependsOn)
            {
                case DependsOn.Depth:
                    comparedValue = achievementManager.gameModel.Depth;
                    break;
                case DependsOn.DestroyedBlocksCount:
                    comparedValue = achievementManager.blocksModel.destroyedBlocksCount;
                    break;
                case DependsOn.EarnedMoney:
                    comparedValue = achievementManager.resourcesModel.earnedMoney;
                    break;
                case DependsOn.Money:
                    comparedValue = achievementManager.resourcesModel.money;
                    break;
                case DependsOn.MinedNormalBlocks:
                    comparedValue = StatisticsModel.Instance.MinedNormalBlocks;
                    break;
                case DependsOn.MinedCopperBlocks:
                    comparedValue = StatisticsModel.Instance.MinedCopperBlocks;
                    break;
                case DependsOn.MinedIronBlocks:
                    comparedValue = StatisticsModel.Instance.MinedIronBlocks;
                    break;
                case DependsOn.MinedGoldBlocks:
                    comparedValue = StatisticsModel.Instance.MinedGoldBlocks;
                    break;
                case DependsOn.MinedDiamondBlocks:
                    comparedValue = StatisticsModel.Instance.MinedDiamondBlocks;
                    break;
                case DependsOn.MinedUraniumBlocks:
                    comparedValue = StatisticsModel.Instance.MinedUraniumBlocks;
                    break;
                case DependsOn.PrestigeCurrency:
                    comparedValue = achievementManager.resourcesModel.prestigeCurrency;
                    break;
                case DependsOn.EarnedPrestigeCurrency:
                    comparedValue = achievementManager.resourcesModel.earnedPrestigeCurrency;
                    break;
                case DependsOn.PremiumCurrency:
                    comparedValue = achievementManager.resourcesModel.premiumCurrency;
                    break;
                case DependsOn.EarnedPremiumCurrency:
                    comparedValue = achievementManager.resourcesModel.earnedPremiumCurrency;
                    break;
                case DependsOn.powerUpTimeLeft:
                    comparedValue = achievementManager.resourcesModel.powerUpTimeLeft;
                    break;
                case DependsOn.earnedPowerUpTime:
                    comparedValue = achievementManager.resourcesModel.earnedPowerUpTime;
                    break;
            }
            double result = comparedValue - requirements[i].requiredValue;

            if (result <= 0 && requirements[i].comparison == Comparison.GT)
            {
                return;
            }
            if (result < 0 && requirements[i].comparison == Comparison.GTEQ)
            {
                return;
            }
            if (result != 0 && requirements[i].comparison == Comparison.EQ)
            {
                return;
            }
            if (result > 0 && requirements[i].comparison == Comparison.LTEQ)
            {
                return;
            }
            if (result >= 0 && requirements[i].comparison == Comparison.LT)
            {
                return;
            }
        }
        isCompleted = true;
        achievementManager.onAchievementUnlocked?.Invoke(this);

        for (int j = 0; j < requirements.Length; j++)
        {
            DependsOn requirement = requirements[j].dependsOn;
            if (requirement == DependsOn.Depth)
            {
                achievementManager.gameModel.onDepthChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.DestroyedBlocksCount)
            {
                achievementManager.blocksModel.onDestroyedBlocksCountChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.Money || requirement == DependsOn.EarnedMoney)
            {
                achievementManager.resourcesManager.onMoneyChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.MinedNormalBlocks)
            {
                StatisticsModel.Instance.onMinedNormalBlocksChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.MinedCopperBlocks)
            {
                StatisticsModel.Instance.onMinedCopperBlocksChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.MinedIronBlocks)
            {
                StatisticsModel.Instance.onMinedIronBlocksChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.MinedGoldBlocks)
            {
                StatisticsModel.Instance.onMinedGoldBlocksChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.MinedDiamondBlocks)
            {
                StatisticsModel.Instance.onMinedDiamondBlocksChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.MinedUraniumBlocks)
            {
                StatisticsModel.Instance.onMinedUraniumBlocksChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.PrestigeCurrency || requirement == DependsOn.EarnedPrestigeCurrency)
            {
                achievementManager.resourcesManager.onPrestigeCurrencyChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.PremiumCurrency || requirement == DependsOn.EarnedPremiumCurrency)
            {
                achievementManager.resourcesManager.onPremiumCurrencyChange -= checkIfCompleted;
            }
            if (requirement == DependsOn.powerUpTimeLeft || requirement == DependsOn.earnedPowerUpTime)
            {
                achievementManager.resourcesManager.onPowerUpTimeAdded -= checkIfCompleted;
            }
        }
    }
}

[System.Serializable]
public class PersistentAchievement
{
    public string name;
    public bool isCompleted;

    public PersistentAchievement(string name, bool isCompleted)
    {
        this.name = name;
        this.isCompleted = isCompleted;
    }
}

public class AchievementManager : MonoBehaviour
{

    public GameModel gameModel;
    public BlocksModel blocksModel;
    public ResourcesManager resourcesManager;
    public ResourcesModel resourcesModel;

    public AchievementScriptable[] achievementsScriptable;
    public List<Achievement> achievements;
    public UnityAction<Achievement> onAchievementUnlocked;

    public GameObject achievementGrid;
    public AchievementSquare achievementSquare;
    public AchievementTooltip achievementTooltip;

    private List<AchievementSquare> achievementSquares;

    [SerializeField] UpgradeScriptable achievementReward;
    [SerializeField] UpgradeScriptable rowReward;

    [SerializeField] UpgradesModel upgradesModel;

    [SerializeField] int achievementsInRow;
    [SerializeField] GridLayoutGroup glp;

    private void SetSquaresWidth()
    {
        int squareWidth = Mathf.RoundToInt(1120 / (achievementsInRow+0.5f));
        int squareSpacing = (1120 - achievementsInRow * squareWidth) / (achievementsInRow - 1);
        glp.spacing = new Vector2(squareSpacing, squareSpacing);
        glp.cellSize = new Vector2(squareWidth, squareWidth);
    }

    private void Awake()
    {
        achievements = new List<Achievement>();
        achievementSquares = new List<AchievementSquare>();
        for (int i=0; i<achievementsScriptable.Length; i++)
        {
            achievements.Add(achievementsScriptable[i].Achievement);
            // TODO: move this to a better place
            AchievementSquare achievementSquareInstance = Instantiate(achievementSquare, achievementGrid.transform);
            achievementSquareInstance.SetAchievementAndTooltip(achievements[i], achievementTooltip);
            achievementSquares.Add(achievementSquareInstance);
            // TODO: there must be a better way...
            onAchievementUnlocked += achievementSquareInstance.SetColor;
        }
        SetSquaresWidth();
    }

    public void ConnectUpgrades()
    {
        onAchievementUnlocked += (Achievement achievement) => {
            Upgrade upgrade = upgradesModel.upgrades[achievementReward.Upgrade.GenerateName()];
            upgrade.DoUpgrade();

            int index = achievements.FindIndex(a => a.name == achievement.name);
            int row = index / achievementsInRow;
            bool rowCompleted = true;

            for (int i=0; i<achievementsInRow; i++)
            {
                if (i + row * achievementsInRow < achievements.Count && !achievements[i + row * achievementsInRow].isCompleted)
                {
                    rowCompleted = false;
                }
            }

            if (rowCompleted)
            {
                Upgrade rowUpgrade = upgradesModel.upgrades[rowReward.Upgrade.GenerateName()];
                rowUpgrade.DoUpgrade();
            }
        } ;
    }

    public void SetupAchievements()
    {
        for (int i=0; i<achievements.Count; i++)
        {
            if (!achievements[i].isCompleted)
            {
                achievements[i].achievementManager = this;
                for (int j = 0; j < achievements[i].requirements.Length; j++)
                {
                    DependsOn requirement = achievements[i].requirements[j].dependsOn;
                    if (requirement == (DependsOn.Depth))
                    {
                        gameModel.onDepthChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == (DependsOn.DestroyedBlocksCount))
                    {
                        blocksModel.onDestroyedBlocksCountChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == (DependsOn.Money) || requirement == (DependsOn.EarnedMoney))
                    {
                        resourcesManager.onMoneyChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.MinedNormalBlocks)
                    {
                        StatisticsModel.Instance.onMinedNormalBlocksChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.MinedCopperBlocks)
                    {
                        StatisticsModel.Instance.onMinedCopperBlocksChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.MinedIronBlocks)
                    {
                        StatisticsModel.Instance.onMinedIronBlocksChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.MinedGoldBlocks)
                    {
                        StatisticsModel.Instance.onMinedGoldBlocksChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.MinedDiamondBlocks)
                    {
                        StatisticsModel.Instance.onMinedDiamondBlocksChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.MinedUraniumBlocks)
                    {
                        StatisticsModel.Instance.onMinedUraniumBlocksChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.PrestigeCurrency || requirement == DependsOn.EarnedPrestigeCurrency)
                    {
                        resourcesManager.onPrestigeCurrencyChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.PremiumCurrency || requirement == DependsOn.EarnedPremiumCurrency)
                    {
                        resourcesManager.onPremiumCurrencyChange += achievements[i].checkIfCompleted;
                    }
                    if (requirement == DependsOn.powerUpTimeLeft || requirement == DependsOn.earnedPowerUpTime)
                    {
                        resourcesManager.onPowerUpTimeAdded += achievements[i].checkIfCompleted;
                    }
                }
            }
        }
    }

    public void SavePersistentData(PersistentData persistentData)
    {
        persistentData.unlockedAchievements = achievements
            .Where(achievement => achievement.isCompleted)
            .Select(ach => new PersistentAchievement(ach.name, ach.isCompleted))
            .ToArray();
    }

    public void LoadPersistentData(PersistentData persistentData)
    {
        if(persistentData.unlockedAchievements != null)
        {
            foreach (var unlockedAch in persistentData.unlockedAchievements)
            {
                for (int i = 0; i < achievements.Count; i++)
                {
                    if (unlockedAch.name == achievements[i].name)
                    {
                        achievements[i].isCompleted = true;
                        break;
                    }
                }
            }
        }
        
    }
}
