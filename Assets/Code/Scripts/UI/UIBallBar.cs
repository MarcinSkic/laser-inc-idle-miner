using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBallBar : MonoBehaviour
{
    public List<UIButtonUpgradeController> buttonUpgrades;

    public void SetUpgradesName(string ballType)
    {
        buttonUpgrades[0].upgradeName = ballType + "Count"; //TODO: Get name from gameobject name
        buttonUpgrades[1].upgradeName = ballType + "Speed";
        buttonUpgrades[2].upgradeName = ballType + "Damage";
    }
}
