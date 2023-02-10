using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ResourcesModel : MonoBehaviour
{
    [AutoProperty(AutoPropertyMode.Scene)]
    [SerializeField] ResourcesManager manager;

    [Tooltip("Current money")]
    public double money = 0;
    [ReadOnly]
    [Tooltip("Total of money earned")]
    public double earnedMoney = 0;
    [ReadOnly]
    [Tooltip("Calculated reward for last offline time")]
    public double offlineMoney = 0;
    [ReadOnly]
    [Tooltip("Total of money earned for offline rewards")]
    public double offlineEarnedMoney = 0;


    #region AFK gain test
    [ReadOnly]
    public List<double> lastEarnedMoneyStates;
    [SerializeField] public double earnedOver15;
    [SerializeField] public double maxEarnedOver15;
    [SerializeField] public double maxPerSecOver15;
    [SerializeField] public double afkGainPerSec15;
    [SerializeField] public double maxToAfkProportion15;
    [SerializeField] public double currentPerSecOver15;
    [SerializeField] public double afkToCurrentProportion15;
    #endregion

    private void OnValidate()
    {
        manager.onMoneyChange?.Invoke(money);
    }
}
