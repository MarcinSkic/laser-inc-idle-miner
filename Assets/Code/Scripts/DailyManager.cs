using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum RewardType
{
    premium,
    powerup,
    money,
    batFrenzy,
}

[Serializable]
class DailyReward
{
    public float baseAmount;
    public float bonusPerLap;
    public RewardType rewardType;

    public DailyReward(float baseAmount, float bonusPerLap, RewardType rewardType)
    {
        this.baseAmount = baseAmount;
        this.bonusPerLap = bonusPerLap;
        this.rewardType = rewardType;
    }
}

public class DailyManager : MonoBehaviour
{
    public DateTime last_reward_time = DateTime.Now.Subtract(TimeSpan.FromDays(1));
    public int consecutive_rewards_count;
    public ResourcesManager resourcesManager;
    public PremiumStoreManager premiumStoreManager;

    DailyReward[] dailyRewards = {
        new DailyReward(5, 1, RewardType.premium),
        new DailyReward(1800, 300, RewardType.powerup),
        new DailyReward(2700, 600, RewardType.money),
        new DailyReward(2, 0.5f, RewardType.batFrenzy),
        new DailyReward(10, 2, RewardType.premium),
        new DailyReward(3600, 600, RewardType.powerup),
        new DailyReward(3600, 900, RewardType.money),
    };


    private void Start()
    {
        
        
    }

    public bool CheckDailyState(out int rewardIndex)
    {
        
        DateTime lastDate = last_reward_time.Date;
        DateTime nowDate = DateTime.Now.Date;
        TimeSpan diff = nowDate.Subtract(lastDate);
        Debug.Log("diff in days: "+diff.TotalDays);
        Debug.Log($"lastDate: {lastDate}, nowDate: {nowDate}");
        if (diff.TotalDays == 0) {
            // juz dzis odebrane, nic ciekawego
            rewardIndex = GetRewardIndex();
            Debug.Log("juz dzis odebrane, nic ciekawego");
            return false;
        } else if (diff.TotalDays == 1) {
            // odebrane wczoraj, mozna odebrac dzisiaj, nie ma resetu
            rewardIndex = GetRewardIndex();
            Debug.Log("odebrane wczoraj, mozna odebrac dzisiaj, nie ma resetu");
            return true;
        } else {
            // nie odebrano dzisiaj ani wczoraj, mozna odebrac, ale jest reset
            consecutive_rewards_count = 0;
            rewardIndex = GetRewardIndex();
            Debug.Log("nie odebrano dzisiaj ani wczoraj, mozna odebrac, ale jest reset");
            return true;
        }
    }

    public float CalculateReward(int rewardIndex)
    {
        int laps = consecutive_rewards_count / dailyRewards.Length;
        return dailyRewards[rewardIndex].baseAmount + laps * dailyRewards[rewardIndex].bonusPerLap;
    }

    public TimeSpan CalculateWaitTimeForReward(int rewardIndex)
    {
        int diff;
        bool rewardToBeCollected = CheckDailyState(out int currentRewardIndex);

        if (rewardIndex >= currentRewardIndex)
        {
            diff = rewardIndex - currentRewardIndex + (rewardToBeCollected ? 0 : 1);
        } else
        {
            diff = dailyRewards.Length - (currentRewardIndex - rewardIndex) + (rewardToBeCollected ? 0 : 1);
        }

        return DateTime.Now.Date.AddDays(diff).Subtract(DateTime.Now);
    }

    public float CalculateRewardForDisplay(int rewardIndex)
    {
        bool rewardToBeCollected = CheckDailyState(out int currentRewardIndex);

        float rewardAmount = CalculateReward(rewardIndex);
        if (rewardIndex  >= currentRewardIndex)
        {
            return rewardAmount;
        }
        else
        {
            return rewardAmount + dailyRewards[rewardIndex].bonusPerLap;
        }
    }

    public int GetRewardIndex()
    {
        return consecutive_rewards_count % dailyRewards.Length;
    }

    public void GetReward()
    {
        last_reward_time = DateTime.Now;
        
        int rewardIndex = GetRewardIndex();
        float rewardAmount = CalculateReward(rewardIndex);

        switch (dailyRewards[rewardIndex].rewardType)
        {
            case RewardType.money:
                resourcesManager.IncreaseMoneyForOfflineByTime(rewardAmount);
                AudioManager.Instance.Play("caught_coins");
                break;
            case RewardType.powerup:
                resourcesManager.IncreasePowerUpTimeLeft(rewardAmount);
                AudioManager.Instance.Play("caught_p_up");
                break;
            case RewardType.premium:
                resourcesManager.IncreasePremiumCurrency(rewardAmount);
                AudioManager.Instance.Play("caught_crystal");
                break;
            case RewardType.batFrenzy:
                if (premiumStoreManager.batFrenzy != null)
                {
                    StopCoroutine(premiumStoreManager.batFrenzy);
                }
                premiumStoreManager.batFrenzy = StartCoroutine(premiumStoreManager.BatFrenzy(rewardAmount));
                break;
        }

        consecutive_rewards_count++;
    }

    public void SavePersistentData(PersistentData data)
    {
        data.last_reward_time = last_reward_time.ToBinary().ToString();
        data.consecutive_rewards_count = consecutive_rewards_count;
    }

    public void LoadPersistentData(PersistentData data)
    {
        if (data?.last_reward_time == null || long.Parse(data.last_reward_time) == 0)
        {
            
            last_reward_time = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        } 
        else
        {
            last_reward_time = DateTime.FromBinary(long.Parse(data.last_reward_time));
        }

        consecutive_rewards_count = data?.consecutive_rewards_count ?? 0;
    }
}
