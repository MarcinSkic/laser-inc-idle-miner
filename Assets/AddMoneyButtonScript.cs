using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMoneyButtonScript : MonoBehaviour
{
    [SerializeField] ResourcesManager resourcesManager;

    public void AddALotOfMoney()
    {
        resourcesManager.IncreaseMoneyForOfflineByValue(1e99);
    }
}
