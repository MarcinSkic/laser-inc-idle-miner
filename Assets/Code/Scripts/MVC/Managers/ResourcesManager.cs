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
        model.lastOnlineEarnedMoneyStates.Add(model.earnedMoney - model.offlineEarnedMoney);
        if (model.lastOnlineEarnedMoneyStates.Count > secs+1)
        {
            model.lastOnlineEarnedMoneyStates.RemoveAt(0);
        }
        model.earnedOverSecs = model.earnedMoney - model.lastOnlineEarnedMoneyStates[0];
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

    public void IncreaseMoneyForOffline(double value)
    {
        Money += value;
        model.earnedMoney += value;
        model.offlineEarnedMoney += value;
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
    }

    public void LoadPersistentData(PersistentData data)
    {
        Money = data?.money ?? 0;
        model.earnedMoney = data?.earnedMoney ?? 0;
        model.offlineEarnedMoney = data?.offlineEarnedMoney ?? 0;
    }
}
