using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/UpgradeDouble", order = 1)]
public class UpgradeDoubleScriptable : ScriptableObject
{
    public UpgradeDouble upgrade;
    public static implicit operator UpgradeDouble(UpgradeDoubleScriptable scriptable) => scriptable.upgrade;
}
