using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpgradesModel : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeScriptable> upgradesScriptable;

    [Header("Debug")]
    public List<Upgrade> upgrades;

    public Upgrade getUpgrade(string name)
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
            upgrades.Add(scriptable.Upgrade);
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