using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using MyBox;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.UI;

public class PremiumStoreManager : MonoBehaviour
{
    [Header("Upgrades")]
    [SerializeField] UpgradeScriptable doubleMoneyUpgrade;

    [Header("Values")]
    [SerializeField] float costOfEarnOfflineTime = 40;
    [SerializeField] int rewardedOfflineTime_Seconds = 36000;

    [SerializeField] float costOfEarnOfflineTime2 = 40;
    [SerializeField] int rewardedOfflineTime_Seconds2 = 36000;

    [Space(10)]
    [SerializeField] float costOfEarnPrestigeReward = 50;
    [SerializeField] [Range(0f, 1f)] float percentageOfPrestigeReward = 0.5f;

    [Header("Managers and Models")]
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private ResourcesManager resourcesManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private AdManager adManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private UpgradesModel upgradesModel;

    [Header("UI")]
    [SerializeField] private TMP_Text earnOffline_Title;
    [SerializeField] private UIButtonController earnOffline_Button;
    [SerializeField] private TMP_Text earnOffline_Display;

    [SerializeField] private TMP_Text earnOffline_Title2;
    [SerializeField] private UIButtonController earnOffline_Button2;
    [SerializeField] private TMP_Text earnOffline_Display2;

    [SerializeField] private Button doubleMoneyButton;
    [SerializeField] private Button noAdsButton;

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
            noAdsButton.gameObject.SetActive(!adManager.subscribedNoAds);
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


        #region Earn Offline Time 2
        earnOffline_Title2.text = $"Instant money equal to {TimeSpan.FromSeconds(rewardedOfflineTime_Seconds2).TotalHours}h of offline gain";

        earnOffline_Button2.Init();
        earnOffline_Button2.SetText($"{costOfEarnOfflineTime2}");
        earnOffline_Button2.onClick += () => {
            if (resourcesManager.TryDecreaseCurrency(costOfEarnOfflineTime2, Currency.Premium))
            {
                resourcesManager.IncreaseMoneyForOfflineByTime(rewardedOfflineTime_Seconds2);
            }
            else
            {
                buttons[0].Click();
            }

        };

        resourcesManager.onAfkGainChange += (gainPerSec) => { earnOffline_Display2.text = NumberFormatter.Format(gainPerSec * rewardedOfflineTime_Seconds2); };
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


        Upgrade temp = upgradesModel.upgrades[doubleMoneyUpgrade.Upgrade.GenerateName()];
        if (temp.currentLevel == temp.maxLevel)
        {
            doubleMoneyButton.gameObject.gameObject.gameObject.SetActive(false);
        }
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
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"LI_purchase_{crystalPacksValues[x]}_diamonds");
                break;
            case "liim.noads":
                adManager.subscribedNoAds = true;
                noAdsButton.gameObject.SetActive(false);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("LI_purchase_NoAds");
                break; 
            case "liim.doublemoney":
                upgradesModel.upgrades[doubleMoneyUpgrade.Upgrade.GenerateName()].DoUpgrade();
                Firebase.Analytics.FirebaseAnalytics.LogEvent("LI_purchase_DoubleMoney");
                // vewwy impowtant cowd UwU
                doubleMoneyButton.gameObject.SetActive(false);
                break;
            default:
                Debug.Log(product.definition.id);
                break;
        }
        onPremiumBuy?.Invoke();
    }

    public void OnPurchaseRestoration(bool result,string message)
    {
        if (result)
        {
#if !UNITY_EDITOR
            CheckNoAdSub();
#endif
            var products = CodelessIAPStoreListener.Instance?.StoreController?.products;
            var doubleMoney = products.WithID("liim.doublemoney");
            if (doubleMoney.appleProductIsRestored)
            {
                OnPremiumBuy(doubleMoney);
            }
        } 
        else
        {
            Debug.LogError($"Purchase restoration failed: {message}");
        }
    }

    public void OnPremiumFailed(Product product, PurchaseFailureReason reason)
    {

    }
}
