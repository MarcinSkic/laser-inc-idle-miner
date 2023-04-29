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

    [Header("UI")]
    [SerializeField] List<UIPremiumElement> buttons;
    Dictionary<string, double> crystalPacksValues;

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

    public UnityAction onPremiumBuy;
    
    public void OnPremiumBuy(Product product)
    {
        switch (product.definition.id)
        {
            case var x when x.Contains("liim.crystals"):
                resourcesManager.IncreasePremiumCurrency(crystalPacksValues[x]);
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
