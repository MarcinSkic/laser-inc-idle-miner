using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementTooltip : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] Vector2 offset;

    IEnumerator Display(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        gameObject.SetActive(false);
    }

    public void DisplayAchievement(Achievement achievement)
    {
        nameText.text = achievement.name;
        descriptionText.text = achievement.description;

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Display(5f));
    }
}
