using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeBar : MonoBehaviour
{
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private UIButtonUpgradeController upgradeButton;
    public UIButtonUpgradeController UpgradeButton => upgradeButton;

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    public void SetLevel(Upgrade upgrade)
    {
        if (upgrade.maxLevel == 1)
        {
            levelText.gameObject.SetActive(false);
        }
        else
        {
            levelText.text = string.Format(upgrade.maxLevel == -1 ? "Lv {0}" : "Lv {0}/{1}", upgrade.currentLevel,upgrade.maxLevel);
        }
    }
}
