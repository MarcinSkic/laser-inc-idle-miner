using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private ResourcesModel model;

    [SerializeField] GameModel gameModel;

    public UnityAction<double> onPowerUpTimeIncrease;
    public UnityAction<double> onPowerUpTimeEarned;
    public UnityAction<double> onPremiumCurrencyChange;
    public UnityAction<double> onPremiumCurrencyEarned;
    public UnityAction<double> onPrestigeCurrencyChange;
    public UnityAction<double> onPrestigeCurrencyEarned;
    public UnityAction<double> onMoneyChange;
    public UnityAction<double> onMoneyEarned;
    public UnityAction<double> onAfkGainChange;


    private void Start()
    {
        InvokeRepeating(nameof(UpdateLastEarnedMoneyStates), 1f, 1f);
    }
    private void Update()
    {
        DecreasePowerUpTimeLeft(Time.deltaTime);
    }

    #region PowerUpTime
    public double PowerUpTimeLeft
    {
        get
        {
            return model.powerUpTimeLeft;
        }
        set
        {
            if(model.powerUpTimeLeft < value)
            {
                onPowerUpTimeIncrease?.Invoke(value);
            }
            model.powerUpTimeLeft = value;
        }
    }

    public double EarnedPowerUpTime
    {
        get
        {
            return model.earnedPowerUpTime;
        }
        set
        {
            model.earnedPowerUpTime = value;
            onPowerUpTimeEarned?.Invoke(value);
        }
    }

    public void IncreasePowerUpTimeLeft(double value)
    {
        EarnedPowerUpTime += value;
        PowerUpTimeLeft += value;
    }

    public void DecreasePowerUpTimeLeft(double value)
    {
        if(PowerUpTimeLeft - value < 0)
        {
            PowerUpTimeLeft = 0;
            return;
        }

        PowerUpTimeLeft -= value;
    }
    #endregion

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

    public double EarnedPremiumCurrency
    {
        get
        {
            return model.earnedPremiumCurrency;
        }
        set
        {
            model.earnedPremiumCurrency = value;
            onPremiumCurrencyEarned?.Invoke(value);
        }
    }

    public void IncreasePremiumCurrency(double value)
    {
        if (value < 1)
        {
            value = 1;
        }
        EarnedPremiumCurrency += value;
        PremiumCurrency += value;
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

    public double EarnedPrestigeCurrency
    {
        get
        {
            return model.earnedPrestigeCurrency;
        }
        set
        {
            model.earnedPrestigeCurrency = value;
            onPrestigeCurrencyEarned?.Invoke(value);
        }
    }

    public double PrestigeCurrencyForNextPrestige
    {
        get
        {
            //return model.prestigeCurrencyForNextPrestige;
            if (gameModel.Depth >= 500f)
            {
                return Math.Pow(2f, (gameModel.Depth/500f));
            }
            return 0f;
        }
    }

    public void IncreasePrestigeCurrency(double value)
    {
        if (value < 1)
        {
            value = 1;
        }
        EarnedPrestigeCurrency += value;    //TODO-@FILIP: Should it save beetwen prestiges? I guess Yes, but then i want ACCURATE and COMPREHENSIVE list of prestige persistent variables
        PrestigeCurrency += value;
    }

    public bool IsPrestigeWorth()
    {
        return PrestigeCurrency < (PrestigeCurrencyForNextPrestige + PrestigeCurrency) / model.prestigeCurrencyMultiplierWorthThreshold;
    }
    #endregion

    void UpdateLastEarnedMoneyStates()
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

        onAfkGainChange?.Invoke(model.afkGainPerSec);
    }

    #region Money
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

    public double EarnedMoney
    {
        get
        {
            return model.earnedMoney;
        }
        set
        {
            model.earnedMoney = value;
            onMoneyEarned?.Invoke(value);
        }
    }

    public void IncreaseMoney(double value, bool forBreakingBlocks = false)
    {
        if (value < 1)
        {
            value = 1;
        }

        if (forBreakingBlocks)
        {
            value *= model.moneyGainMultiplier;
        }

        Money += value;
        EarnedMoney += value;
    }

    public void CheatMoney()
    {
        Money += model.cheatMoney;
        //TODO-@Filip: Should it increase earnedmoney?
    }
    #endregion

    public double GetCurrency(Currency currency)
    {
        return currency switch
        {
            Currency.Money => Money,
            Currency.Prestige => PrestigeCurrency,
            Currency.Premium => PremiumCurrency,
            _ => -1,
        };
    }

    public bool TryDecreaseCurrency(double value, Currency currency)
    {
        switch (currency)
        {
            case Currency.Money:
                if (Money - value < 0)
                {
                    return false;
                }
                else
                {
                    Money -= value;
                    return true;
                }
            case Currency.Prestige:
                if (PrestigeCurrency - value < 0)
                {
                    return false;
                }
                else
                {
                    PrestigeCurrency -= value;
                    return true;
                }
            case Currency.Premium:
                if (PremiumCurrency - value < 0)
                {
                    return false;
                }
                else
                {
                    PremiumCurrency -= value;
                    return true;
                }
            default:
                return false;
        }
        
    }
    public void LoadInspectorMoney()
    {
        Money = model.money + model.startMoney;
        PremiumCurrency = model.premiumCurrency;
        PrestigeCurrency = model.prestigeCurrency;
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

    public void SavePrestigePersistentData(PersistentData data)
    {
        data.prestigeCurrency = PrestigeCurrency;
        data.earnedPrestigeCurrency = model.earnedPrestigeCurrency;
    }

    public void LoadPersistentData(PersistentData data)
    {
        Money = data?.money ?? model.startMoney;
        EarnedMoney = data?.earnedMoney ?? 0;
        model.offlineEarnedMoney = data?.offlineEarnedMoney ?? 0;
        model.afkGainPerSec = data?.afkGainPerSec ?? 0;
        model.lastOnlineEarnedMoneyStates = data?.lastOnlineEarnedMoneyStates ?? new List<double>();

        PremiumCurrency = data?.premiumCurrency ?? 0;
        EarnedPremiumCurrency = data?.earnedPremiumCurrency ?? 0;
        PrestigeCurrency = data?.prestigeCurrency ?? 0;
        EarnedPrestigeCurrency = data?.earnedPrestigeCurrency ?? 0;

        PowerUpTimeLeft = data?.powerUpTime ?? 0;
        EarnedPowerUpTime = data?.earnedPowerUpTime ?? 0;
    }
}
