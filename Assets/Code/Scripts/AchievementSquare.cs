using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSquare : MonoBehaviour
{
    [SerializeField] Achievement achievement;
    [SerializeField] AchievementTooltip achievementTooltip;
    [SerializeField] Image image;
    [SerializeField] Color lockedColor;

    public void SetAchievementAndTooltip(Achievement achievement, AchievementTooltip achievementTooltip)
    {
        this.achievement = achievement;
        this.achievementTooltip = achievementTooltip;
        image.sprite = achievement.sprite;
    }

    public void ActivateTooltip()
    {
        achievementTooltip.DisplayAchievement(achievement);
    }

    public void SetColor(Achievement _)
    {
        if (!achievement.isCompleted)
        {
            image.color = lockedColor;
        } else
        {
            image.color = new Color(1, 1, 1);
        }
    }
}
