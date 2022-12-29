using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBallBar : MonoBehaviour
{
    public Image ballIcon;
    public List<UIButtonUpgradeController> buttonUpgrades;

    public void SetUpgradesName(UpgradeableObjects type)
    {
        buttonUpgrades[0].upgradeName = type.ToString() + "Count";
        buttonUpgrades[1].upgradeName = type.ToString() + UpgradeableValues.Speed.ToString();
        buttonUpgrades[2].upgradeName = type.ToString() + UpgradeableValues.Damage.ToString();
    }
}
