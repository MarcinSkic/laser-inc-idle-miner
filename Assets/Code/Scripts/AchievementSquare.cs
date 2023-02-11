using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AchievementSquare : MonoBehaviour
{
    [SerializeField] Achievement achievement;
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] Color lockedColor;

    public void Init(Achievement achievement)
    {
        this.achievement = achievement;
        image.sprite = achievement.sprite;
        SetColor(achievement);
        button.onClick.AddListener(() => { onAchievementClicked?.Invoke(achievement); });
    }

    public UnityAction<Achievement> onAchievementClicked;
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
