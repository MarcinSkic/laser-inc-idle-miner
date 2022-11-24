using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/UpgradeAction", order = 1)]
public class UpgradeScriptable : ScriptableObject
{
    public Upgrade upgrade;
    public static implicit operator Upgrade(UpgradeScriptable scriptable) => scriptable.upgrade;
}
