using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSquare : MonoBehaviour
{
    [SerializeField] Achievement achievement;
    [SerializeField] AchievementTooltip achievementTooltip;
    [SerializeField] Image image;

    public void SetAchievementAndTooltip(Achievement achievement, AchievementTooltip achievementTooltip)
    {
        this.achievement = achievement;
        this.achievementTooltip = achievementTooltip;
        image.sprite = achievement.sprite;
    }
}
