using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum Currency { Money, Prestige, Premium, IAP }
public class ResourcesModel : MonoBehaviour
{
    [AutoProperty(AutoPropertyMode.Scene)]
    [SerializeField] ResourcesManager manager;

    [Header("POWER-UP")]
    [ReadOnly]
    [Tooltip("Current power-up time")]
    public double powerUpTimeLeft = 0;
    [ReadOnly]
    [Tooltip("Total of power-up time earned")]
    public double earnedPowerUpTime = 0;

    [Header("PREMIUM CURRENCY")]
    [Tooltip("Current premium currency")]
    public double premiumCurrency = 0;
    [ReadOnly]
    [Tooltip("Total of premium currency earned")]
    public double earnedPremiumCurrency = 0;

    [Header("PRESTIGE CURRENCY")]
    [Tooltip("Current prestige currency")]
    public double prestigeCurrency = 0;
    [ReadOnly]
    [Tooltip("Total of prestige currency earned")]
    public double earnedPrestigeCurrency = 0;
    public double prestigeCurrencyMultiplierWorthThreshold = 5;

    [Header("CURRENT MONEY DATA")]
    public double startMoney = 1;
    [Tooltip("Current money")]
    public double money = 0;
    public double cheatMoney = 1e99;
    [ReadOnly]
    [Tooltip("Total of money earned")]
    public double earnedMoney = 0;
    [ReadOnly]
    [Tooltip("Total of money earned, including over prestiges")]
    public double undecreasableEarnedMoney = 0;
    [ReadOnly]
    [Tooltip("Calculated reward for last offline time")]
    public double offlineMoney = 0;
    [ReadOnly]
    [Tooltip("Total of money earned for offline rewards")]
    public double offlineEarnedMoney = 0;
    [ReadOnly]
    [Tooltip("Current reward for offline time")]
    public double afkGainPerSec;
    [Tooltip("Only affects reward for breaking blocks")]
    public double moneyGainMultiplier;

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
        manager.onPrestigeCurrencyChange?.Invoke(prestigeCurrency);
    }
}
