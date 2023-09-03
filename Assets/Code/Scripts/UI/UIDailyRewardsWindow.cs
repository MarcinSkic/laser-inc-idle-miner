using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyRewardsWindow : MonoBehaviour
{
    [SerializeField] DailyManager manager;
    [SerializeField] UIDayReward[] rewards;

    private int rewardIndex = 0;
    private float prescaler = 0;

    private void OnEnable()
    {
        if(manager.CheckDailyState(out rewardIndex))
        {
            ConfigureRewards();
        }
    }

    private void ConfigureRewards()
    {
        foreach(var reward in rewards)
        {
            reward.Disable();
        }

        rewards[rewardIndex].Enable();
    }

    private void OnDisable()
    {
        
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

    }
}
