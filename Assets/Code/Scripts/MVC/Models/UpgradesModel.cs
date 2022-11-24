using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpgradesModel : MonoBehaviour
{
    //TODO-DISCUSS: Keep it in some collection? Also they probably should have some internal string name
    public List<UpgradeScriptable> upgrades;    //TODO: target - assign here only scriptable, on game launch it will connect / transform into a pair or copy it starting values... somewhere :D

    public UpgradePair universalSpeed;
    public UpgradePair universalDamage;
    public UpgradePair basicBallCount;
    public UpgradePair bombBallCount;
    public UpgradePair sniperBallCount;
}

[System.Serializable]
public class UpgradePair
{
    public UpgradeScriptable scriptable;
    public Upgrade upgrade;

    public static implicit operator Upgrade(UpgradePair upgradePair) => upgradePair.upgrade;

    public void LoadStartData()
    {
        upgrade = Functions.GetObjectCopy(scriptable.upgrade);
    }
}