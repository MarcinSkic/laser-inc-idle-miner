using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ResourcesModel : MonoBehaviour
{
    [AutoProperty(AutoPropertyMode.Scene)]
    [SerializeField] ResourcesManager manager;

    public double money = 0;
    [ReadOnly]
    public double earnedMoney = 0;
    [ReadOnly]
    public double offlineMoney = 0;

    private void OnValidate()
    {
        manager.onMoneyChange?.Invoke(money);
    }
}
