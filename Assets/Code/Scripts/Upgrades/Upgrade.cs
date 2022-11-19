using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Upgrade<T>
{
    public UpgradeableData<T>[] upgradeableDatas;

    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costIncremental;
    [Tooltip("Formula: currentCost = currentCost*costMultiplier+costIncremental")]
    public double costMultiplier;
    
    public double currentCost;

    public int currentLevel = 0;
    [Tooltip("Set to -1 for infinite levels")]
    public int maxLevel;

    public void AddUpgradeable(params UpgradeableData<T>[] datas)
    {
        upgradeableDatas = datas;
    }

    public bool TryUpgrade(out double newCost)
    {
        if (currentLevel == maxLevel)
        {
            newCost = currentCost;
            return false;
        }

        currentCost = currentCost * costMultiplier + costIncremental;
        currentLevel++;
        newCost = currentCost;

        UpgradeValues();
        return true;
    }

    protected abstract void UpgradeValues();
}
