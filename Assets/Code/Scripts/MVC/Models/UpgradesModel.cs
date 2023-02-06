using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


public class UpgradesModel : MonoBehaviour
{
    [SerializeField]
    [InitializationField]
    private List<UpgradeScriptable> upgradesScriptable;

    public Dictionary<string,Upgrade> upgrades;

    public void TransformScriptablesIntoUpgrades()
    {
        upgrades = new Dictionary<string, Upgrade>();

        foreach(var scriptable in upgradesScriptable)
        {
            var upgrade = scriptable.Upgrade;
            upgrade.GenerateName();
            upgrades[upgrade.name] = upgrade;
        }
    }
}