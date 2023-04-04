using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButtonUpgradeController : UIButtonController
{
    [Header("BUTTON UPGRADE")]
    public string upgradeName;
    [SerializeField]
    private TMP_Text costText;  //TODO-FT-RESOURCES

    private double cost;    //TODO-FT-RESOURCES

    public override void Init()
    {
        base.Init();
        Deactivate();
    }

    public void MaxLock()
    {
        SetHardDeactivate(true);
        Deactivate();
    }

    public void SetUpgradeCost(Upgrade upgrade)
    {
        cost = upgrade.cost;
        costText.text = NumberFormatter.Format(upgrade.cost);

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
}
