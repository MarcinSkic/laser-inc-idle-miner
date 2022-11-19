using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpgradesModel : MonoBehaviour
{
    //TODO-DISCUSS: Keep it in some collection? Also they probably should have some internal string name
    public UpgradePairDouble universalSpeed;
    public UpgradePairDouble universalDamage;
    public UpgradePairAction basicBallCount;
    public UpgradePairAction bombBallCount;
    public UpgradePairAction sniperBallCount;
}

[System.Serializable]
public class UpgradePairInt 
{
    public UpgradeIntScriptable scriptable;
    public UpgradeInt upgrade;

    public static implicit operator UpgradeInt(UpgradePairInt upgradePair) => upgradePair.upgrade;

    public void LoadStartData()
    {
        upgrade = Functions.GetObjectCopy(scriptable.upgrade);
    }
}

[System.Serializable]
public class UpgradePairDouble
{
    public UpgradeDoubleScriptable scriptable;
    public UpgradeDouble upgrade;

    public static implicit operator UpgradeDouble(UpgradePairDouble upgradePair) => upgradePair.upgrade;

    public void LoadStartData()
    {
        upgrade = Functions.GetObjectCopy(scriptable.upgrade);
    }
}

[System.Serializable]
public class UpgradePairAction
{
    public UpgradeActionScriptable scriptable;
    public UpgradeAction upgrade;

    public static implicit operator UpgradeAction(UpgradePairAction upgradePair) => upgradePair.upgrade;

    public void LoadStartData()
    {
        upgrade = Functions.GetObjectCopy(scriptable.upgrade);
    }
}