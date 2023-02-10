using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private ResourcesModel model;

    public UnityAction<double> onMoneyChange;

    #region AFK gain test

    void updateLastEarnedMoneyStates()
    {
        int secs = model.secondsForOfflineRewardCalculation;

        // save states
        double currentOnlineEarnedMoney = model.earnedMoney - model.offlineEarnedMoney;
        model.lastOnlineEarnedMoneyStates.Add(currentOnlineEarnedMoney);
        if (model.lastOnlineEarnedMoneyStates.Count > secs+1)
        {
            model.lastOnlineEarnedMoneyStates.RemoveAt(0);
        }
        model.earnedOverSecs = currentOnlineEarnedMoney - model.lastOnlineEarnedMoneyStates[0];
        model.currentPerSecOverSecs = model.earnedOverSecs / secs;
        model.afkToCurrentProportion = model.afkGainPerSec / model.currentPerSecOverSecs;
        if (model.earnedOverSecs > model.maxEarnedOverSecs)
        {
            model.maxEarnedOverSecs = model.earnedOverSecs;
            model.maxPerSecOverSecs = model.maxEarnedOverSecs / 5f;
            model.afkGainPerSec = model.maxPerSecOverSecs / model.maxToAfkProportion;
        }
    }

    private void Start()
    {
        InvokeRepeating("updateLastEarnedMoneyStates", 1f, 1f);
    }

    #endregion

    public double Money
    {
        get
        {
            return model.money;
        }
        set
        {
            model.money = value;
            onMoneyChange?.Invoke(value);
        }
    }

    public void LoadInspectorMoney()
    {
        Money = model.money;
    }

    public void IncreaseMoney(double value)
    {
        if (value < 1)
        {
            value = 1;
        }
        Money += value;
        model.earnedMoney += value;
    }

    // add X money as offline/reward money
    public void IncreaseMoneyForOfflineByValue(double value)
    {
        IncreaseMoney(value);
        model.offlineEarnedMoney += value;
    }

    // get amount of money due for X offline seconds
    public double CalculateOfflineMoney(double seconds)
    {
        return seconds * model.afkGainPerSec;
    }

    // add money equal to offline reward for X seconds
    public void IncreaseMoneyForOfflineByTime(double seconds)
    {
        IncreaseMoneyForOfflineByValue(CalculateOfflineMoney(seconds));
    }

    public bool TryDecreaseMoney(double value)
    {
        if(Money - value < 0)
        {
            return false;
        } 
        else
        {
            Money -= value;
            return true;
        }
    }

    public void SavePersistentData(PersistentData data)
    {
        data.money = Money;
        data.earnedMoney = model.earnedMoney;
        data.offlineEarnedMoney = model.offlineEarnedMoney;
        data.afkGainPerSec = model.afkGainPerSec;
        data.lastOnlineEarnedMoneyStates = model.lastOnlineEarnedMoneyStates;
    }

    public void LoadPersistentData(PersistentData data)
    {
        Money = data?.money ?? 0;
        model.earnedMoney = data?.earnedMoney ?? 0;
        model.offlineEarnedMoney = data?.offlineEarnedMoney ?? 0;
        model.afkGainPerSec = data?.afkGainPerSec ?? 0;
        model.lastOnlineEarnedMoneyStates = data?.lastOnlineEarnedMoneyStates ?? new List<double>();
    }
}
