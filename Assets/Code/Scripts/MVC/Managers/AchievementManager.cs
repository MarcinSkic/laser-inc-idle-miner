using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DependsOn
{
    Depth,
    DestroyedBlocksCount,
    Money,
    EarnedMoney,
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
        Debug.Log($"COMPLETED ACHIEVEMENT: {name} - {description}");
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
