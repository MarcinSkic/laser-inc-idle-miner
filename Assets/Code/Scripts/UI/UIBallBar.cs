using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBallBar : MonoBehaviour
{
    [SerializeField] private GameObject lockedUpgradeHider;
    public Image ballIcon;
    public List<UIButtonUpgradeController> buttonUpgrades;
    public UpgradeableObjects ballType;
    public TMP_Text ballTitle;
    public TMP_Text toUnlockText;

    public void Lock()
    {
        lockedUpgradeHider.SetActive(true);
    }

    public void Unlock(Upgrade _)
    {
        lockedUpgradeHider.SetActive(false);
    }

    public void SetUpgradesName(UpgradeableObjects type)
    {
        buttonUpgrades[0].upgradeName = type.ToString() + "Count";
        buttonUpgrades[1].upgradeName = type.ToString() + UpgradeableValues.Speed.ToString();
        buttonUpgrades[2].upgradeName = type.ToString() + UpgradeableValues.Damage.ToString();
    }
}
