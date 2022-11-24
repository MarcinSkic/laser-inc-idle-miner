using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UpgradeType { ValuesUgprade, CustomFunction}
public enum UpgradedObjects { Balls, SpecifiedBelow}
public enum ValueUpgradeFormula { Add, Multiply };

[System.Serializable]
public class Upgrade
{
    [Header("Fill: Always")]
    public string upgradeName;
    public UpgradeType upgradeType;

    public double cost;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costIncremental;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costMultiplier;

    [Tooltip("Set to -1 for infinite levels")]
    public int maxLevel;

    [Space(5)]

    [Header("Fill: Values Upgrade")]
    public UpgradedObjects upgradedObjects;
    public List<string> upgradedObjectsNames;
    public List<string> upgradedValuesNames;
    [Tooltip("Effect depends on used formula")]
    public double changeValue;
    public ValueUpgradeFormula formula;

    [Header("Debug")]
    public int currentLevel = 0;

    [HideInInspector]
    public UnityAction onUpgrade;

    public void AddOnUpgrade(params UnityAction[] actions)
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

        onUpgrade?.Invoke();
        return true;
    }
}
