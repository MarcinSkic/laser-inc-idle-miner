using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementTooltip : MonoBehaviour
{
    TMP_Text nameText;
    TMP_Text descriptionText;

    public void DisplayAchievement(Achievement achievement)
    {
        nameText.text = achievement.name;
        descriptionText.text = achievement.description;

    }
}
