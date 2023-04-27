using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using MyBox;
using UnityEngine.Events;

public class PremiumStoreManager : MonoBehaviour
{
    [AutoProperty(AutoPropertyMode.Scene)] [SerializeField] private SavingManager savingManager;
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
            button.Text = GetValueFromID(button.iapButton.productId);
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
        onPremiumBuy?.Invoke();
    }

    public void OnInGameCurrencyBuy()
    {
        onPremiumBuy?.Invoke();
    }
}
