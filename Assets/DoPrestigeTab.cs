using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoPrestigeTab : MonoBehaviour
{
    [SerializeField] ResourcesManager resourcesManager;
    [SerializeField] TMP_Text rewardText;
    [SerializeField] Button resetButton; 

    void UpdateDisplay()
    {
        double currentReward = resourcesManager.PrestigeCurrencyForNextPrestige;
        if (currentReward > 0)
        {
            resetButton.interactable = true;
            rewardText.text = $"If you prestige now, you will get <color=\"green\">{currentReward}</color> prestige currency.";
        }
        else
        {
            resetButton.interactable = false;
            rewardText.text = "<color=\"orange\">You need to reach a depth of 500m to be able to prestige.</color>";
        }
    }

    private void Start()
    {
        UpdateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }
}
