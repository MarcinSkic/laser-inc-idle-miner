using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoPrestigeTab : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] ResourcesManager resourcesManager;
    [SerializeField] TMP_Text rewardText;
    [SerializeField] UIButtonController resetButton; 

    void UpdateDisplay()
    {
        double currentReward = resourcesManager.PrestigeCurrencyForNextPrestige;
        if (currentReward > 0)
        {
            resetButton.Activate();
            rewardText.text = $"If you prestige now, you will get <color=\"green\">{NumberFormatter.Format(currentReward)}</color> prestige currency.";
        }
        else
        {
            resetButton.Deactivate();
            rewardText.text = "<color=\"orange\">You need to reach a depth of 500m to be able to prestige.</color>";
        }
    }

    private void Start()
    {
        resetButton.Init();
        resetButton.onClick += gameController.TryExecutePrestige;
        UpdateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }
}
