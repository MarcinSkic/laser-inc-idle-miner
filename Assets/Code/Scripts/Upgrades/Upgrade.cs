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
    public UnityAction<Upgrade> onTryUpgrade;
    public Upgrade initialUpgrade;
    public string textValue;

    /// <summary>
    /// This method is to streamline assigning functions to events
    /// </summary>
    /// <param name="actions"></param>
    public void AddOnUpgrade(params UnityAction<Upgrade>[] actions)
    {
        foreach (var action in actions)
        {
            onUpgrade += action;
        }
    }

    public void AddOnTryUpgrade(params UnityAction<Upgrade>[] actions)
    {
        foreach (var action in actions)
        {
            onTryUpgrade += action;
        }
    }

    /// <summary>
    /// This should be called by UI and event onTryUpgrade should be connected to further validations
    /// </summary>
    /// <returns></returns>
    public void TryUpgrade()
    {
        if (currentLevel == maxLevel)
        {
            return;
        }

        onTryUpgrade?.Invoke(this);
    }

    /// <summary>
    /// This should be called after final validation
    /// </summary>
    public void DoUpgrade()
    {
        cost = cost * costMultiplier + costIncremental;
        currentLevel++;

        onUpgrade?.Invoke(this);
    }
}
