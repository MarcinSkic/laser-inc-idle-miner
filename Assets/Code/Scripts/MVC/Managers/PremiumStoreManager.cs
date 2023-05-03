using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using MyBox;
using UnityEngine.Events;
using TMPro;
using System;

public class PremiumStoreManager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float costOfEarnOfflineTime = 40;
    [SerializeField] int rewardedOfflineTime_Seconds = 36000;

    [Space(10)]
    [SerializeField] float costOfEarnPrestigeReward = 50;
    [SerializeField] [Range(0f, 1f)] float percentageOfPrestigeReward = 0.5f;

    [Header("Managers and Models")]
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private ResourcesManager resourcesManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private AdManager adManager;

    [Header("UI")]
    [SerializeField] private TMP_Text earnOffline_Title;
    [SerializeField] private UIButtonController earnOffline_Button;
    [SerializeField] private TMP_Text earnOffline_Display;

    [Space(10)]
    [SerializeField] private TMP_Text earnPrestige_Title;
    [SerializeField] private UIButtonController earnPrestige_Button;
    [SerializeField] private TMP_Text earnPrestige_Display;
    [SerializeField] List<UIPremiumElement> buttons;

    [Header("Other")]
    Dictionary<string, double> crystalPacksValues;
    float subTimer = 1;

    private void Update()
    {
        subTimer -= Time.deltaTime;
        if (subTimer <= 0)
        {
            subTimer = 15;
#if !UNITY_EDITOR
            CheckNoAdSub();
#endif
        }
    }

    public void Setup()
    {
        crystalPacksValues = new Dictionary<string, double>
        {
            {"liim.crystals1",100 },
            {"liim.crystals2",300 },
            {"liim.crystals3",600 },
            {"liim.crystals4",1800 },
            {"liim.crystals5",4000 },
            {"liim.crystals6",10000 }
        };

        foreach(var button in buttons)
        {
            button.iapButton.onPurchaseComplete.AddListener(OnPremiumBuy);
            button.iapButton.onPurchaseFailed.AddListener(OnPremiumFailed);

            if (button.HasValue)
            {
                button.Text = GetValueFromID(button.iapButton.productId);
            }
        }

        #region Earn Offline Time
        earnOffline_Title.text = $"Instant money equal to {TimeSpan.FromSeconds(rewardedOfflineTime_Seconds).TotalHours}h of offline gain";

        earnOffline_Button.Init();
        earnOffline_Button.SetText($"{costOfEarnOfflineTime}");
        earnOffline_Button.onClick += () => {
            if (resourcesManager.TryDecreaseCurrency(costOfEarnOfflineTime, Currency.Premium))
            {
                resourcesManager.IncreaseMoneyForOfflineByTime(rewardedOfflineTime_Seconds);
            } else
            {
                buttons[0].Click();
            }
            
        };

        resourcesManager.onAfkGainChange += (gainPerSec) => { earnOffline_Display.text = NumberFormatter.Format(gainPerSec*rewardedOfflineTime_Seconds); };
        #endregion

        #region Earn Prestige Reward
        earnPrestige_Title.text = $"Instant {percentageOfPrestigeReward*100}% prestige reward without reset";

        earnPrestige_Button.Init();
        earnPrestige_Button.SetText($"{costOfEarnPrestigeReward}");
        earnPrestige_Button.onClick += () =>
        {
            if (resourcesManager.TryDecreaseCurrency(costOfEarnPrestigeReward, Currency.Premium))
            {
                resourcesManager.IncreasePrestigeCurrency(resourcesManager.PrestigeCurrencyForNextPrestige*percentageOfPrestigeReward);
            }
            else
            {
                buttons[0].Click();
            }
        };

        StartCoroutine(DisplayPrestigeReward());
        #endregion
    }

    IEnumerator DisplayPrestigeReward()
    {
        while (true)
        {
            earnPrestige_Display.text = NumberFormatter.Format(resourcesManager.PrestigeCurrencyForNextPrestige * percentageOfPrestigeReward);
            yield return new WaitForSeconds(1);
        }
        
    }

    private string GetValueFromID(string id)
    {
        if(!crystalPacksValues.ContainsKey(id))
        {
            Debug.LogError($"No value for product of id: {id}");
            return "couldn't retrieve value!";
        }

        return $"{crystalPacksValues[id]}";
    }

    public void CheckNoAdSub()
    {

        try
        {
            var products = CodelessIAPStoreListener.Instance?.StoreController?.products;
            var product = products.WithID("liim.noads");
            var subscriptionManager = new SubscriptionManager(product, null);
            var isSubscribed = subscriptionManager.getSubscriptionInfo()?.isSubscribed();
            if (isSubscribed == Result.True)
            {
                adManager.subscribedNoAds = true;
            } else
            {
                adManager.subscribedNoAds = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

    }

    public UnityAction onPremiumBuy;
    public void OnPremiumBuy(Product product)
    {
        switch (product.definition.id)
        {
            case var x when x.Contains("liim.crystals"):
                resourcesManager.IncreasePremiumCurrency(crystalPacksValues[x]);
                break;
            case "liim.noads":
                adManager.subscribedNoAds = true;
                break;
            default:
                Debug.Log(product.definition.id);
                break;
        }
        onPremiumBuy?.Invoke();
    }

    public void OnPremiumFailed(Product product, PurchaseFailureReason reason)
    {

    }
}
