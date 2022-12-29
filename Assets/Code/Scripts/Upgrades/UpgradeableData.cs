using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class UpgradeableData<T>
{
    public UpgradeableValues type;
    [ConditionalField(nameof(type),false,UpgradeableValues.Special)]
    public string name;
    public T value;
    
    public static implicit operator T(UpgradeableData<T> data) => data.value; //Used to simplify bombBallData.speed.value => bombBallData.speed
}