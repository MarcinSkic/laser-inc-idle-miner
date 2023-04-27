using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;

public class UIPremiumElement : MonoBehaviour
{
    public IAPButton iapButton;
    [SerializeField] private TMP_Text valueText;

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
