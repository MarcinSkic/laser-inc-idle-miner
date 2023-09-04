using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIDailyRewardsWindow : MonoBehaviour
{
    [SerializeField] DailyManager manager;
    [SerializeField] UIDayReward[] rewards;
    [SerializeField] ResourcesManager resourcesManager;
    [SerializeField] string powerUpTitle = "Double damage for {0}";
    [SerializeField] string offlineMoneyTitle = "Money equal to {0} of offline gain";
    [SerializeField] string batFrenzyTitle = "Bat frenzy for {0}";


    private int rewardIndex = 0;
    private float prescaler = 0;
    private float rewardDay3 = 0;
    private float rewardDay7 = 0;

    private void Start()
    {
        InitDisplays();
    }

    private void OnEnable()
    {
        ConfigureRewards();       
    }

    public void ConfigureRewards()
    {
        foreach (var reward in rewards)
        {
            reward.Disable();
        }

        if (manager.CheckDailyState(out rewardIndex))
        {
            rewards[rewardIndex].Enable(OnRewardClick);

        }

        foreach (var (reward, index) in rewards.Select((value, index) => (value, index)))
        {
            reward.SetTime(manager.CalculateWaitTimeForReward(index).TotalSeconds);
        }

        SetDisplays();
    }

    private void InitDisplays()
    {
        resourcesManager.onAfkGainChange += (gainPerSec) => { rewards[2].value.text = NumberFormatter.Format(gainPerSec * rewardDay3); };
        resourcesManager.onAfkGainChange += (gainPerSec) => { rewards[6].value.text = NumberFormatter.Format(gainPerSec * rewardDay7); };
    }

    private void SetDisplays()
    {        
        rewards[0].value.text = manager.CalculateRewardForDisplay(0).ToString();
        rewards[1].title.text = string.Format(powerUpTitle, NumberFormatter.FormatSecondsToHours(manager.CalculateRewardForDisplay(1)));

        rewardDay3 = manager.CalculateRewardForDisplay(2);
        rewards[2].title.text = string.Format(offlineMoneyTitle, NumberFormatter.FormatSecondsToHours(rewardDay3));
        rewards[3].title.text = string.Format(batFrenzyTitle, $"{manager.CalculateRewardForDisplay(3)}s");
        rewards[4].value.text = manager.CalculateRewardForDisplay(4).ToString();
        rewards[5].title.text = string.Format(powerUpTitle, NumberFormatter.FormatSecondsToHours(manager.CalculateRewardForDisplay(5)));

        rewardDay7 = manager.CalculateRewardForDisplay(6);
        rewards[6].title.text = string.Format(offlineMoneyTitle, NumberFormatter.FormatSecondsToHours(rewardDay7));

    }

    private void OnRewardClick()
    {
        manager.GetReward();
        ConfigureRewards();
    }
    
    private void Update()
    {
        prescaler += Time.deltaTime;
        if(prescaler > 1f)
        {
            prescaler = 0;

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        foreach (var reward in rewards)
        {
            reward.TickTime();
        }
    }
}
