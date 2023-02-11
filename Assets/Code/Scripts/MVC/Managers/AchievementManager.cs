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
    PowerUpTimeLeft,
    EarnedPowerUpTime,
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
    private bool isFulfilled = false;
    private bool IsFulfilled
    {
        set
        {
            if(value != isFulfilled)
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
}

[System.Serializable]
public class Achievement
{
    public string name;
    public string description;
    public Sprite sprite;
    public bool isCompleted = false;
    public int leftRequirements = 0;
    [SerializeField]
    public Requirement[] requirements;

    public UnityAction<Achievement> onAchievementUnlocked;

    //TODO-MAYBE-BUG: Order of events firing may impact some weird achievements where you need to have exactly this and this value etc.
    public void CheckIfAchieved(bool newStateOfRequirement)
    {
        if (!isCompleted)
        {
            leftRequirements += newStateOfRequirement ? -1 : 1;

            if (leftRequirements <= 0)
            {
                isCompleted = true;
                onAchievementUnlocked?.Invoke(this);
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

    public void DoAchievementsUpgrades(Achievement achievement)
    {
        Upgrade upgrade = upgradesModel.upgrades[achievementReward.Upgrade.GenerateName()];
        upgrade.DoUpgrade();

        int index = achievements.FindIndex(a => a.name == achievement.name);
        int row = index / achievementsInRow;
        bool rowCompleted = true;

        for (int i = 0; i < achievementsInRow; i++)
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
    }

    public void SetupAchievements()
    {
        foreach(Achievement achievement in achievements)
        {
            if (!achievement.isCompleted)
            {

                foreach (Requirement requirement in achievement.requirements)
                {

                    switch (requirement.dependsOn)
                    {
                        case DependsOn.Depth:
                            gameModel.onDepthChange += requirement.CheckIfFulfilled;
                            break;
                        case DependsOn.DestroyedBlocksCount:
                            blocksModel.onDestroyedBlocksCountChange += requirement.CheckIfFulfilled;
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
                        case DependsOn.MinedCopperBlocks:
                            StatisticsModel.Instance.onMinedCopperBlocksChange += requirement.CheckIfFulfilled;
                            break;
                        case DependsOn.MinedIronBlocks:
                            StatisticsModel.Instance.onMinedIronBlocksChange += requirement.CheckIfFulfilled;
                            break;
                        case DependsOn.MinedGoldBlocks:
                            StatisticsModel.Instance.onMinedGoldBlocksChange += requirement.CheckIfFulfilled;
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
                    }
                    requirement.onStateChanged += achievement.CheckIfAchieved;

                    achievement.leftRequirements++;
                }

                achievement.onAchievementUnlocked += OnAchievementUnlock;
            }         
        }
    }

    public void DisconnectAchievement(Achievement achievement)
    {
        foreach (Requirement requirement in achievement.requirements)
        {
            switch (requirement.dependsOn)
            {
                case DependsOn.Depth:
                    gameModel.onDepthChange -= requirement.CheckIfFulfilled;
                    break;
                case DependsOn.DestroyedBlocksCount:
                    blocksModel.onDestroyedBlocksCountChange -= requirement.CheckIfFulfilled;
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
                case DependsOn.MinedCopperBlocks:
                    StatisticsModel.Instance.onMinedCopperBlocksChange -= requirement.CheckIfFulfilled;
                    break;
                case DependsOn.MinedIronBlocks:
                    StatisticsModel.Instance.onMinedIronBlocksChange -= requirement.CheckIfFulfilled;
                    break;
                case DependsOn.MinedGoldBlocks:
                    StatisticsModel.Instance.onMinedGoldBlocksChange -= requirement.CheckIfFulfilled;
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
            }
            requirement.onStateChanged -= achievement.CheckIfAchieved;  //If we are lazy, this alone is enough to disconnect achievement, requirements will fire without anyone listening to    
        }

        achievement.onAchievementUnlocked -= OnAchievementUnlock;
    }

    public void OnAchievementUnlock(Achievement achievement)
    {
        DoAchievementsUpgrades(achievement);

        onAchievementUnlocked?.Invoke(achievement);

        DisconnectAchievement(achievement);
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
