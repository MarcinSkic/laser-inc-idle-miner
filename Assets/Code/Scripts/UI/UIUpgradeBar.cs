using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeBar : MonoBehaviour
{
    [SerializeField] private GameObject lockedUpgradeHider;
    [SerializeField] private TMP_Text lockedTitle;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TMP_Text levelOrValueText;
    [SerializeField] private GameObject levelOrValueDisplay;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private UIButtonUpgradeController upgradeButton;
    [SerializeField] private TMP_Text toUnlockText;

    public UIButtonUpgradeController UpgradeButton => upgradeButton;

    public void SetTitle(string value)
    {
        UpgradeButton.SetText(value);
        lockedTitle.text = value;
    }

    public void Lock()
    {
        lockedUpgradeHider.SetActive(true);
    }

    public void Unlock(Upgrade _)
    {
        lockedUpgradeHider.SetActive(false);
    }

    public void OnMaxed(Upgrade _)
    {
        upgradeButton.SetHardDeactivate(true);
        upgradeButton.Deactivate();

        //TODO-@FILIP: It should disappear?
        //gameObject.SetActive(false);
    }

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    public void SetCurrencySprite(Sprite sprite)    //TODO-UGLY: With current UI this method is useless
    {
        if(currencyIcon != null)
        {
            currencyIcon.sprite = sprite;
        }
    }

    public void SetLevel(Upgrade upgrade)
    {
        if (upgrade.maxLevel == 1)
        {
            levelOrValueDisplay.SetActive(false);
        }
        else
        {
            levelOrValueDisplay.SetActive(true);
            levelOrValueText.text = string.Format(upgrade.maxLevel == -1 ? "Lv {0}" : "Lv {0}/{1}", upgrade.currentLevel,upgrade.maxLevel);
        }
    }

    public void SetValue(string value)
    {
        levelOrValueDisplay.SetActive(true);
        levelOrValueText.text = value;
    }

    public void SetToUnlockDescription(string description)
    {
        toUnlockText.text = description;
    }
}
