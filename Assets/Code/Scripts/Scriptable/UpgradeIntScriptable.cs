using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/UpgradeInt", order = 1)]
public class UpgradeIntScriptable : ScriptableObject
{
    public UpgradeInt upgrade;
    public static implicit operator UpgradeInt(UpgradeIntScriptable scriptable) => scriptable.upgrade;
}
