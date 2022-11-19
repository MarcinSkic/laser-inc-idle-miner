using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeInt : Upgrade<int>
{
    protected override void UpgradeValues()
    {
        for (int i = 0; i < upgradeableDatas.Length; i++)
        {
            UpgradeableData<int> data = upgradeableDatas[i];
            data.value++;
        }
    }
}
