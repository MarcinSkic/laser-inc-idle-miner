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

    public void SetUpgradesValues(Upgrade upgrade)
    {
        text.text = "0"; //TODO-CURRENT upgrade.value.toString()? I mean even if upgrade changes a lot stats it should display smth
        cost = upgrade.cost;
        costText.text = upgrade.cost.ToString();

        if(upgrade.currentLevel == upgrade.maxLevel)
        {
            //TODO-CURRENT: Do smth
        }
    }
}
