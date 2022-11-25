using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UpgradeType { ValuesUpgrade, SpawnUpgrade, CustomFunction}
public enum UpgradedObjects { AllBalls, SpecifiedBalls}
public enum ValueUpgradeFormula { Add, Multiply };

[System.Serializable]
public class Upgrade
{
    [Header("Fill: Always")]
    public string name;
    public UpgradeType type;

    public double cost;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costIncremental;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costMultiplier;

    [Tooltip("Set to -1 for infinite levels")]
    public int maxLevel = -1;

    [Space(5)]

    [Header("Fill: Values Upgrade & Spawn Upgrade")]
    public UpgradedObjects upgradedObjects;
    public List<string> specifiedObjects;
    [Header("Fill: Values Upgrade")]
    public List<string> upgradedValuesNames;
    [Tooltip("Effect depends on used formula")]
    public double changeValue;
    public ValueUpgradeFormula formula;

    [Header("Debug")]
    public int currentLevel = 0;

    public UnityAction<Upgrade> onUpgrade;
    public Upgrade initialUpgrade;

    public void AddOnUpgrade(params UnityAction<Upgrade>[] actions)
    {
        foreach (var action in actions)
        {
            onUpgrade += action;
        }
    }

    public bool TryUpgrade(out double newCost)
    {
        if (currentLevel == maxLevel)
        {
            newCost = cost;
            return false;
        }

        cost = cost * costMultiplier + costIncremental;
        currentLevel++;
        newCost = cost;

        onUpgrade?.Invoke(this);
        return true;
    }
}
