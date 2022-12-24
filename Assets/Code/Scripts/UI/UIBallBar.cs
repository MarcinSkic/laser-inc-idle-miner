using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBallBar : MonoBehaviour
{
    public Image ballIcon;
    public List<UIButtonUpgradeController> buttonUpgrades;

    public void SetUpgradesName(string ballType)
    {
        buttonUpgrades[0].upgradeName = ballType + "Count"; //TODO: Get name from gameobject name
        buttonUpgrades[1].upgradeName = ballType + "Speed";
        buttonUpgrades[2].upgradeName = ballType + "Damage";
    }
}
