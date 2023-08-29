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
    public string last_reward_timestring;
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
        // Invoke(nameof(CheckDailyState), 0.1f);
        // InvokeRepeating("GetReward", 3, 3);
    }

    public void CheckDailyState()
    {
        DateTime lastDate = DateTime.Parse(last_reward_timestring);
        DateTime nowDate = DateTime.Parse(DateTime.Now.Date.ToLongDateString());
        TimeSpan diff = nowDate.Subtract(lastDate);
        Debug.Log("diff in h: "+diff.TotalHours);
        bool odbieralne;
        if (diff.TotalHours == 0) {
            // juz dzis odebrane, nic ciekawego
            odbieralne = false;
            Debug.Log("juz dzis odebrane, nic ciekawego");
        } else if (diff.TotalHours == 24) {
            // odebrane wczoraj, mozna odebrac dzisiaj, nie ma resetu
            odbieralne = true;
            Debug.Log("odebrane wczoraj, mozna odebrac dzisiaj, nie ma resetu");
        } else {
            // nie odebrano dzisiaj ani wczoraj, mozna odebrac, ale jest reset
            consecutive_rewards_count = 0;
            odbieralne = true;
            Debug.Log("nie odebrano dzisiaj ani wczoraj, mozna odebrac, ale jest reset");
        }
    }

    public void GetReward()
    {
        last_reward_timestring = DateTime.Now.Date.ToLongDateString();
        consecutive_rewards_count++;
        int rewardIndex = consecutive_rewards_count % dailyRewards.Length;
        int laps = consecutive_rewards_count / dailyRewards.Length;
        float rewardAmount = dailyRewards[rewardIndex].baseAmount + laps * dailyRewards[rewardIndex].bonusPerLap;

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
        Debug.LogWarning("index: "+rewardIndex + ", laps: " +laps+ ", reward: " + dailyRewards[rewardIndex].rewardType + ", amount: " + rewardAmount);
    }

    public void SavePersistentData(PersistentData data)
    {
        data.last_reward_timestring = last_reward_timestring;
        data.consecutive_rewards_count = consecutive_rewards_count;
    }

    public void LoadPersistentData(PersistentData data)
    {
        last_reward_timestring = data?.last_reward_timestring ?? DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToLongDateString();
        consecutive_rewards_count = data?.consecutive_rewards_count ?? 0;
    }
}
