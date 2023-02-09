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
        // for debug
        // IncreaseMoney(1);

        model.lastEarnedMoneyStates.Add(model.earnedMoney);
        if (model.lastEarnedMoneyStates.Count > 16)
        {
            model.lastEarnedMoneyStates.RemoveAt(0);
        }
        model.earnedOver15 = model.earnedMoney - model.lastEarnedMoneyStates[0];
        model.currentPerSecOver15 = model.earnedOver15 / 15f;
        model.afkToCurrentProportion15 = model.afkGainPerSec15 / model.currentPerSecOver15;
        if (model.lastEarnedMoneyStates.Count >= 6)
        {
            model.earnedOver10 = model.earnedMoney - model.lastEarnedMoneyStates[5];
            model.currentPerSecOver10 = model.earnedOver10 / 10f;
            model.afkToCurrentProportion10 = model.afkGainPerSec10 / model.currentPerSecOver10;
            if (model.lastEarnedMoneyStates.Count >= 11)
            {
                model.earnedOver5 = model.earnedMoney - model.lastEarnedMoneyStates[10];
                model.currentPerSecOver5 = model.earnedOver5 / 5f;
                model.afkToCurrentProportion5 = model.afkGainPerSec5/ model.currentPerSecOver5;
            }
        }
        if (model.earnedOver15 > model.maxEarnedOver15)
        {
            model.maxEarnedOver15 = model.earnedOver15;
            model.maxPerSecOver15 = model.maxEarnedOver15 / 5f;
            model.afkGainPerSec15 = model.maxPerSecOver15 / model.maxToAfkProportion15;
        }
        if (model.earnedOver10 > model.maxEarnedOver10)
        {
            model.maxEarnedOver10 = model.earnedOver10;
            model.maxPerSecOver10 = model.maxEarnedOver10 / 5f;
            model.afkGainPerSec10 = model.maxPerSecOver10 / model.maxToAfkProportion10;
        }
        if (model.earnedOver5 > model.maxEarnedOver5)
        {
            model.maxEarnedOver5 = model.earnedOver5;
            model.maxPerSecOver5 = model.maxEarnedOver5 / 5f;
            model.afkGainPerSec5 = model.maxPerSecOver5 / model.maxToAfkProportion5;
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
    }

    public void LoadPersistentData(PersistentData data)
    {
        Money = data?.money ?? 0;
    }
}
