using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


public class UpgradesModel : MonoBehaviour
{
    [SerializeField]
    [InitializationField]
    private List<UpgradeScriptable> upgradesScriptable;

    [Header("Debug")]
    public List<Upgrade> upgrades;

    public Upgrade GetUpgrade(string name)
    {
        return upgrades.Find(upgrade => upgrade.name == name);
    }

    private void Awake()
    {
        TransformScriptablesIntoUpgrades();
    }

    private void TransformScriptablesIntoUpgrades()
    {
        upgrades = new List<Upgrade>();

        foreach(var scriptable in upgradesScriptable)
        {
            var upgrade = scriptable.Upgrade;
            upgrade.GenerateName();
            upgrades.Add(upgrade);
        }
    }

    [ContextMenu("Full upgrades prestige")]
    private void TestFullPrestige()
    {
        for(int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i] = Functions.GetObjectCopy(upgrades[i].initialUpgrade);
        }
    }
}