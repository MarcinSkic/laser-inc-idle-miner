using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using UnityEngine.UI;

public class UIPremiumElement : MonoBehaviour
{
    
    public IAPButton iapButton;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text valueText;
    public void Click()
    {
        button.onClick?.Invoke();
    }
    public bool HasValue
    {
        get
        {
            return valueText != null;
        }
    }
    public string Text
    {
        set
        {
            if(valueText != null)
            {
                valueText.text = value;
            }
        }
    }
}
