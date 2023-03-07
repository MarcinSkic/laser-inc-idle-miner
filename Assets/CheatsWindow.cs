using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheatsWindow : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [Header("PROGRESSION DEBUG")]
    [SerializeField] TMP_Text moneyDebugText;
    [SerializeField] TMP_Text depthDebugText;

    private void Start()
    {
        InvokeRepeating("ShowProgressionDebugData", 0f, 1f);
    }

    private void ShowProgressionDebugData()
    {
        moneyDebugText.text = string.Join("\n",gameController.earnedMoneyMessages);
        depthDebugText.text = string.Join("\n",gameController.depthMessages);
    }
}
