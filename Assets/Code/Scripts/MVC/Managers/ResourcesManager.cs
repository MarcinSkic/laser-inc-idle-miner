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
        model.lastEarnedMoneyStates.Add(model.earnedMoney);
        if (model.lastEarnedMoneyStates.Count > 16)
        {
            model.lastEarnedMoneyStates.RemoveAt(0);
        }
        model.earnedOver15 = model.earnedMoney - model.lastEarnedMoneyStates[0];
        model.currentPerSecOver15 = model.earnedOver15 / 15f;
        model.afkToCurrentProportion15 = model.afkGainPerSec15 / model.currentPerSecOver15;
        if (model.earnedOver15 > model.maxEarnedOver15)
        {
            model.maxEarnedOver15 = model.earnedOver15;
            model.maxPerSecOver15 = model.maxEarnedOver15 / 5f;
            model.afkGainPerSec15 = model.maxPerSecOver15 / model.maxToAfkProportion15;
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
