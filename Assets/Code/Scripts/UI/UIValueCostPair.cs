using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIValueCostPair : MonoBehaviour
{
    [SerializeField]
    private TMP_Text value;  //TODO-FUTURE-BUG: This class should be responsible for updating values changed by more than one upgrade, like speed, damage etc.
    [SerializeField]
    private TMP_Text cost; 

    public double currentCost;  //TODO-FT-RESOURCES

    public void SetValues(string value, double cost)
    {
        currentCost = cost;
        this.value.text = value;
        this.cost.text = cost.ToString();
    }
}
