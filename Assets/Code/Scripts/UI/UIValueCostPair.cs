using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIValueCostPair : MonoBehaviour
{
    [SerializeField]
    private TMP_Text value;
    [SerializeField]
    private TMP_Text cost;  //TODO-CURRENT: ButtonUpgradeController?

    public double currentCost;  //TODO-FT-RESOURCES

    public void SetValues(string value, double cost)
    {
        currentCost = cost;
        this.value.text = value;
        this.cost.text = cost.ToString();
    }
}
