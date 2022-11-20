using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ValueUpgradeFormula { Add, Multiply};
[System.Serializable]
public class UpgradeDouble : Upgrade<double>
{
    [SerializeField]
    private ValueUpgradeFormula formula;
    public double changeValue;
    
    protected override void UpgradeValues()
    {
        for (int i = 0; i<upgradeableDatas.Length; i++)
        {
            UpgradeableData<double> data = upgradeableDatas[i];
            switch (formula)
            {
                case ValueUpgradeFormula.Add:
                    Add(data);
                    break;
                case ValueUpgradeFormula.Multiply:
                    Multiply(data);
                    break;
            }
            
        }
    }

    private void Add(UpgradeableData<double> data)
    {
        data.value += changeValue;
    }

    private void Multiply(UpgradeableData<double> data)
    {
        data.value *= changeValue;
    }
}