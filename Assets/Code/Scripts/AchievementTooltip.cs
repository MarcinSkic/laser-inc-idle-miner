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
    float timeLeft = 0f;

    private void FixedUpdate()
    {
        if (timeLeft < 0)
        {
            gameObject.SetActive(false);
        } else
        {
            timeLeft -= Time.deltaTime;
        }
    }

    public void DisplayAchievement(Achievement achievement)
    {
        nameText.text = achievement.name;
        descriptionText.text = achievement.description;

/*        Vector2 newPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform,
        Input.mousePosition, parentCanvas.worldCamera, out newPos);

        GetComponent<RectTransform>().anchoredPosition = newPos+offset;*/

        gameObject.SetActive(true);
        timeLeft = 5f;
    }
}
