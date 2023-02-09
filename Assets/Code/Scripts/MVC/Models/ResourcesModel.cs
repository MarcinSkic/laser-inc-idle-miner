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


    #region AFK gain test
    [ReadOnly]
    public List<double> lastEarnedMoneyStates;
    [SerializeField] public double earnedOver5;
    [SerializeField] public double earnedOver10;
    [SerializeField] public double earnedOver15;
    [SerializeField] public double maxEarnedOver5;
    [SerializeField] public double maxEarnedOver10;
    [SerializeField] public double maxEarnedOver15;
    [SerializeField] public double maxPerSecOver15;
    [SerializeField] public double maxPerSecOver10;
    [SerializeField] public double maxPerSecOver5;
    [SerializeField] public double afkGainPerSec15;
    [SerializeField] public double afkGainPerSec10;
    [SerializeField] public double afkGainPerSec5;
    [SerializeField] public double maxToAfkProportion15;
    [SerializeField] public double maxToAfkProportion10;
    [SerializeField] public double maxToAfkProportion5;
    [SerializeField] public double currentPerSecOver15;
    [SerializeField] public double currentPerSecOver10;
    [SerializeField] public double currentPerSecOver5;
    [SerializeField] public double afkToCurrentProportion15;
    [SerializeField] public double afkToCurrentProportion10;
    [SerializeField] public double afkToCurrentProportion5;
    #endregion

    private void OnValidate()
    {
        manager.onMoneyChange?.Invoke(money);
    }
}
