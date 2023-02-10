using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private ResourcesModel model;

    public UnityAction<double> onPremiumCurrencyChange;
    public UnityAction<double> onPrestigeCurrencyChange;
    public UnityAction<double> onMoneyChange;

    private void Update()
    {
        DecreasePowerUpTimeLeft(Time.deltaTime);
    }

    public double PowerUpTimeLeft
    {
        get
        {
            return model.powerUpTimeLeft;
        }
        set
        {
            model.powerUpTimeLeft = value;
        }
    }

    public void IncreasePowerUpTimeLeft(double value)
    {
        PowerUpTimeLeft += value;
        model.earnedPowerUpTime += value;
    }

    public void DecreasePowerUpTimeLeft(double value)
    {
        PowerUpTimeLeft -= value;
        if(PowerUpTimeLeft < 0)
        {
            PowerUpTimeLeft = 0;
        }
    }

    #region PremiumCurrency
    public double PremiumCurrency
    {
        get
        {
            return model.premiumCurrency;
        }
        set
        {
            model.premiumCurrency = value;
            onPremiumCurrencyChange?.Invoke(value);
        }
    }

    public void IncreasePremiumCurrency(double value)
    {
        if (value < 1)
        {
            value = 1;
        }
        PremiumCurrency += value;
        model.earnedPremiumCurrency += value;
    }
    #endregion

    #region PrestigeCurrency
    public double PrestigeCurrency
    {
        get
        {
            return model.prestigeCurrency;
        }
        set
        {
            model.prestigeCurrency = value;
            onPrestigeCurrencyChange?.Invoke(value);
        }
    }

    public void IncreasePrestigeCurrency(double value)
    {
        if (value < 1)
        {
            value = 1;
        }
        PrestigeCurrency += value;
        model.earnedPrestigeCurrency += value;
    }
    #endregion


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
        // should this be here???
        PremiumCurrency = model.premiumCurrency;
        PrestigeCurrency = model.prestigeCurrency;
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

        data.premiumCurrency = PremiumCurrency;
        data.earnedPremiumCurrency = model.earnedPremiumCurrency;

        data.prestigeCurrency = PrestigeCurrency;
        data.earnedPrestigeCurrency = model.earnedPrestigeCurrency;

        data.powerUpTime = model.powerUpTimeLeft;
        data.earnedPowerUpTime = model.earnedPowerUpTime;
}

public void LoadPersistentData(PersistentData data)
    {
        Money = data?.money ?? 0;
        model.earnedMoney = data?.earnedMoney ?? 0;
        model.offlineEarnedMoney = data?.offlineEarnedMoney ?? 0;
        model.afkGainPerSec = data?.afkGainPerSec ?? 0;
        model.lastOnlineEarnedMoneyStates = data?.lastOnlineEarnedMoneyStates ?? new List<double>();

        model.premiumCurrency = data?.premiumCurrency ?? 0;
        model.earnedPremiumCurrency = data?.earnedPremiumCurrency ?? 0;
        model.prestigeCurrency = data?.prestigeCurrency ?? 0;
        model.earnedPrestigeCurrency = data?.earnedPrestigeCurrency ?? 0;

        model.powerUpTimeLeft = data?.powerUpTime ?? 0;
        model.earnedPowerUpTime = data?.earnedPowerUpTime ?? 0;
    }
}
