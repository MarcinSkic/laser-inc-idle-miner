using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class AchievementManager : MonoBehaviour
{

    public GameModel gameModel;
    public BlocksModel blocksModel;
    public ResourcesManager resourcesManager;
    public ResourcesModel resourcesModel;

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
        public bool isCompleted = false;
        [SerializeField]
        public AchievementRequirement[] requirements;
        public void checkIfCompleted(double _)
        {
            for (int i=0; i<requirements.Length; i++)
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
            // TAK, COMPLETED!!!
            isCompleted = true;
            Debug.Log($"COMPLETED ACHIEVEMENT: {name} - {description}");

            for (int j = 0; j < requirements.Length; j++)
            {
                DependsOn requirement = requirements[j].dependsOn;
                if (requirement == DependsOn.Depth)
                {
                    achievementManager.gameModel.onDepthChange -= checkIfCompleted;
                    Debug.Log($"odpiento onDepthChange do {name}");
                }
                if (requirement == DependsOn.DestroyedBlocksCount)
                {
                    achievementManager.blocksModel.onDestroyedBlocksCountChange -= checkIfCompleted;
                    Debug.Log($"odpiento onDestroyedBlocksCountChange do {name}");
                }
                if (requirement == DependsOn.Money || requirement == DependsOn.EarnedMoney)
                {
                    achievementManager.resourcesManager.onMoneyChange -= checkIfCompleted;
                    Debug.Log($"odpiento onMoneyChange do {name}");
                }
            }
            achievementManager.onAchievementUnlocked?.Invoke(this);
        }
    }

    public Achievement[] achievements;
    public UnityAction<Achievement> onAchievementUnlocked;

    void Start()
    {
        for (int i=0; i<achievements.Length; i++)
        {
            achievements[i].achievementManager = this;
            for (int j=0; j<achievements[i].requirements.Length; j++)
            {
                DependsOn requirement = achievements[i].requirements[j].dependsOn;
                if (requirement == (DependsOn.Depth))
                {
                    gameModel.onDepthChange += achievements[i].checkIfCompleted;
                    Debug.Log($"podpiento onDepthChange do {achievements[i].name}");
                }
                if (requirement == (DependsOn.DestroyedBlocksCount))
                {
                    blocksModel.onDestroyedBlocksCountChange += achievements[i].checkIfCompleted;
                    Debug.Log($"podpiento onDestroyedBlocksCountChange do {achievements[i].name}");
                }
                if (requirement == (DependsOn.Money) || requirement == (DependsOn.EarnedMoney))
                {
                    resourcesManager.onMoneyChange += achievements[i].checkIfCompleted;
                    Debug.Log($"podpiento onMoneyChange do {achievements[i].name}");
                }
            }
        }
        // blocksModel.onDestroyedBlocksCountChange += updateAchievementsState;
        // resourcesManager.onMoneyChange += Costam;
    }
}
