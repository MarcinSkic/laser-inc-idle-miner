using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeableData<T>
{
    public T value;
    [HideInInspector]
    public double upgradeBaseCost;
    [HideInInspector]
    public double upgradeCostMultiplier;
    [HideInInspector]
    public int upgradeMaxLevel;
    [HideInInspector]
    public int upgradeLevel = 0;

    public static implicit operator T(UpgradeableData<T> data) => data.value;
}
