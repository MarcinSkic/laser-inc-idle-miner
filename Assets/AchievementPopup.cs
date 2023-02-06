using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    [SerializeField] private float disappearStartTime;
    [SerializeField] private float growEndTime;
    private float currentTime = 0f;
    public RectTransform rt;
    private VerticalLayoutGroup vlg;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image image;
    private float currentProportion;

    public void SetAchievement(Achievement achievement)
    {
        titleText.text = achievement.name;
        descriptionText.text = achievement.description;
        image.sprite = achievement.sprite;
    }

    private void Start()
    {
        vlg = gameObject.GetComponentInParent<VerticalLayoutGroup>();
        SetProportion(0f);
    }

    private void SetProportion(float proportion)
    {
        gameObject.transform.localScale = new Vector3(proportion, proportion, proportion);
        currentProportion = proportion;
        // something must be changed in VerticalLayoutGroup to force it to move its ass
        vlg.spacing = 1;
        vlg.spacing = 0;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > destroyTime)
        {
            Destroy(gameObject);
        } else if (currentTime > disappearStartTime)
        {
            SetProportion((destroyTime - currentTime) / (destroyTime - disappearStartTime));
        } else if (currentTime < growEndTime)
        {
            SetProportion(1f-((currentTime- growEndTime) / (-growEndTime)));
        } else if (currentProportion != 1)
        {
            SetProportion(1);
        }
    }
}