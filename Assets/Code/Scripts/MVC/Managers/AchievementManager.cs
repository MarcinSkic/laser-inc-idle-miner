using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    private void Awake()
    {
        achievements = new List<Achievement>();
        
        for (int i=0; i<achievementsScriptable.Length; i++)
        {
            achievements.Add(achievementsScriptable[i].Achievement);
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

                    RequirementsManager.Instance.ConnectRequirementToValueEvent(requirement);
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
            RequirementsManager.Instance.DisconnectRequirementFromValueEvent(requirement);
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
