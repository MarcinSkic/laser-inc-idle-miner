using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ResourcesModel : MonoBehaviour
{
    [AutoProperty(AutoPropertyMode.Scene)]
    [SerializeField] ResourcesManager manager;

    [Header("CURRENT MONEY DATA")]
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
    [ReadOnly]
    [Tooltip("Current reward for offline time")]
    public double afkGainPerSec;

    [Header("OFFLINE REWARD PARAMETERS")]
    [Tooltip("Amount of seconds to calculate offline rewards")]
    public int secondsForOfflineRewardCalculation;
    [SerializeField] public double maxToAfkProportion;


    [Header("OFFLINE REWARD CALCULATIONS")]
    [ReadOnly]
    public List<double> lastOnlineEarnedMoneyStates;
    [ReadOnly]
    public double earnedOverSecs;
    [ReadOnly]
    public double maxEarnedOverSecs;
    [ReadOnly]
    public double maxPerSecOverSecs;
    [ReadOnly]
    public double currentPerSecOverSecs;
    [ReadOnly]
    public double afkToCurrentProportion;

    private void OnValidate()
    {
        manager.onMoneyChange?.Invoke(money);
    }
}
