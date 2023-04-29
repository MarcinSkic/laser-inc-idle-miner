using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using MyBox;
using UnityEngine.Events;

public class PremiumStoreManager : MonoBehaviour
{
    [Header("Managers and Models")]
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private ResourcesManager resourcesManager;
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private AdManager adManager;

    [Header("UI")]
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

    public void OnInGameCurrencyBuy()
    {
        //TODO: Add logic of buying stuff
        onPremiumBuy?.Invoke();
    }
}
