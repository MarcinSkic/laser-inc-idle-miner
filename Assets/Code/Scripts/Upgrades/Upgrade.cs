using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public enum UpgradeType { ValuesUpgrade, SpawnUpgrade, CustomFunction}
public enum ValueUpgradeFormula { Add, Multiply };

[System.Serializable]
public class Upgrade
{
    public string name;
    public UpgradeType type;

    public double cost;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costIncremental;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costMultiplier;

    [Tooltip("Set to -1 for infinite levels")]
    public int maxLevel = -1;

    public bool onUpgradeButtonsShowUpgradeInternalValue = false;

    [ConditionalField(nameof(onUpgradeButtonsShowUpgradeInternalValue))]
    public double upgradeValue = 0;

    [Space(5)]

    [ConditionalField(nameof(type),false,UpgradeType.ValuesUpgrade, UpgradeType.SpawnUpgrade)]
    public UpgradeableObjects upgradedObjects;


    [ConditionalField(nameof(type), false, UpgradeType.ValuesUpgrade)]
    public List<string> upgradedValuesNames;

    [ConditionalField(nameof(type), false, UpgradeType.ValuesUpgrade)]
    [Tooltip("Effect depends on used formula")]
    public double changeValue;

    [ConditionalField(nameof(type), false, UpgradeType.ValuesUpgrade)]
    public ValueUpgradeFormula formula;


    [Header("Debug")]
    [ReadOnly]
    public int currentLevel = 0;

    public UnityAction<Upgrade> doTryUpgrade;
    public UnityAction<Upgrade> doUpgrade;
    public UnityAction<string> onValueUpdate;
    public Upgrade initialUpgrade;


    /// <summary>
    /// This method is to streamline assigning functions to events
    /// </summary>
    /// <param name="actions"></param>
    public void AddDoUpgrade(params UnityAction<Upgrade>[] actions)
    {
        foreach (var action in actions)
        {
            doUpgrade += action;
        }
    }

    public void AddOnTryUpgrade(params UnityAction<Upgrade>[] actions)
    {
        foreach (var action in actions)
        {
            doTryUpgrade += action;
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

        doTryUpgrade?.Invoke(this);
    }

    /// <summary>
    /// This should be called after final validation
    /// </summary>
    public void DoUpgrade()
    {
        cost = cost * costMultiplier + costIncremental;
        currentLevel++;

        doUpgrade?.Invoke(this);
    }
}
