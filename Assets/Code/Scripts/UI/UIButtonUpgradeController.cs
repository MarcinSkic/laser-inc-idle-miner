using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButtonUpgradeController : UIButtonController
{
    [Header("BUTTON UPGRADE")]
    public string upgradeName;  //TODO-FT-DICTIONARIES: Should be enum
    [SerializeField]
    private TMP_Text costText;  //TODO-FT-RESOURCES

    private double cost;    //TODO-FT-RESOURCES

    public override void Init()
    {
        base.Init();
        Deactivate();
    }

    public void SetUpgradeCost(Upgrade upgrade)
    {
        cost = upgrade.cost;
        costText.text = string.Format("{0:f0}$", upgrade.cost);

        if (upgrade.currentLevel == upgrade.maxLevel)
        {
            costText.text = "Maxed";
            RemoveAllEvents();
        }
    }

    public void ChangeStateBasedOnMoney(double money)
    {
        if(money < cost)
        {
            Deactivate();
        } 
        else
        {
            Activate();
        }
    }

    public void SetUpgradeValue(string value)
    {
        ChangeText(value);
    }
}
