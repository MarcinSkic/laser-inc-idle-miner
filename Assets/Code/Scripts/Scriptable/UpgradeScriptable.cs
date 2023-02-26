using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class UpgradeScriptable : ScriptableObject
{
    [SerializeField]
    private Upgrade upgrade;
    public Upgrade Upgrade { 
        get
        {
            var up = Functions.GetObjectCopy(upgrade);
            return up;
        }
        set => upgrade = value;
    }
}
