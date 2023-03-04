using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class BallData
{
    public UpgradeableObjects type;
    public string nameForUI;
    public string description;
    public Sprite sprite;

    [SerializeField]
    private UpgradeableData<double>[] valuesList;

    public Dictionary<UpgradeableValues, UpgradeableData<double>> values;

    public void Init()
    {
        values = new Dictionary<UpgradeableValues, UpgradeableData<double>>();

        foreach (var value in valuesList)
        {
            values.Add(value.type, value);
        }
    }
}
