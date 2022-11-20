using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/UpgradeAction", order = 1)]
public class UpgradeActionScriptable : ScriptableObject
{
    public UpgradeAction upgrade;
    public static implicit operator UpgradeAction(UpgradeActionScriptable scriptable) => scriptable.upgrade;
}
