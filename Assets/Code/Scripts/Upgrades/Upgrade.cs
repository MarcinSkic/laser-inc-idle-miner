using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public enum UpgradeType { ValuesUpgrade, SpawnUpgrade, CustomFunction}
public enum ValueUpgradeFormula { Add, Multiply };
public enum UISection { AutoOrNone, Normal, Choice, Prestige}

[System.Serializable]
public class Upgrade
{
    public UpgradeType type;

    [ConditionalField(nameof(type), false, UpgradeType.CustomFunction)]
    public string name;
    [Tooltip("Required when upgrades change the same stats")]
    public string identifier;

    #region Requirements
    [HideInInspector]
    public bool isUnlocked = false;
    [HideInInspector]
    public int leftRequirements = 0;
    public Requirement[] requirements;
    #endregion

    #region UI
    public UISection whereToGenerate;
    [ConditionalField(nameof(whereToGenerate), true,UISection.AutoOrNone)]
    public int order;
    [ConditionalField(nameof(whereToGenerate), true, UISection.AutoOrNone)]
    public string title;
    [ConditionalField(nameof(whereToGenerate), true, UISection.AutoOrNone)]
    public string description;
    public string toUnlockDescription;
    //Sprite?
    #endregion

    [Tooltip("Set to -1 for infinite levels")]
    public int maxLevel = -1;

    #region Cost
    public Currency currency;
    public double cost;
    [ConditionalField(nameof(maxLevel), true, 1)]
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costIncremental;
    [ConditionalField(nameof(maxLevel), true, 1)]
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costMultiplier = 1;
    #endregion

    public bool showUpgradedValueInsteadOfLevel = false;
    [Tooltip("Invocation: string.Format(<this string>,NumberFormatter.Format(value));")]
    [ConditionalField(nameof(showUpgradedValueInsteadOfLevel))]
    public string upgradedValueFormatedString = "{0}";

    public bool useInternalValueForUI = false;

    [ConditionalField(nameof(useInternalValueForUI))]
    public double internalValue = 0;

    [Space(5)]

    [ConditionalField(nameof(type),false,UpgradeType.ValuesUpgrade, UpgradeType.SpawnUpgrade)]
    public UpgradeableObjects upgradedObjects;

    [ConditionalField(nameof(type), false, UpgradeType.ValuesUpgrade)]
    public UpgradeableValues upgradedValues;

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
    public UnityAction<Upgrade> onMaxedUpgrade;
    public UnityAction<Upgrade> onUnlock;

    public string GenerateName()
    {
        switch (type)
        {
            case UpgradeType.ValuesUpgrade:
                name = $"{upgradedObjects}{upgradedValues}";
                break;
            case UpgradeType.SpawnUpgrade:
                name = $"{upgradedObjects}Count";
                break;
            case UpgradeType.CustomFunction:
                break;
        }

        name += identifier;
        return name;
    }

    public void CheckIfUnlocked(bool newStateOfRequirement)
    {
        if (!isUnlocked)
        {
            leftRequirements += newStateOfRequirement ? -1 : 1;

            if (leftRequirements <= 0)
            {
                isUnlocked = true;
                onUnlock?.Invoke(this);
            }
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

        if (!isUnlocked)
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

        if(currentLevel == maxLevel)
        {
            onMaxedUpgrade?.Invoke(this);
        }

        doUpgrade?.Invoke(this);
    }

    public void DoLoadedUpgrade()
    {
        cost = cost * costMultiplier + costIncremental;
        doUpgrade?.Invoke(this);
    }
}

[System.Serializable]
public class PersistentUpgrade
{
    public string name;
    public int currentLevel;
    public bool isUnlocked;

    public PersistentUpgrade(string name, int currentLevel, bool isUnlocked)
    {
        this.name = name;
        this.currentLevel = currentLevel;
        this.isUnlocked = isUnlocked;
    }

    public override string ToString()
    {
        return $"{name}|{currentLevel}";
    }
}
