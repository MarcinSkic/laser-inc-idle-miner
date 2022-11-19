using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UpgradeAction : Upgrade<UnityAction>
{
    protected override void UpgradeValues()
    {
        for (int i = 0; i < upgradeableDatas.Length; i++)
        {
            UpgradeableData<UnityAction> data = upgradeableDatas[i];
            data.value.Invoke();
        }
    }
}
