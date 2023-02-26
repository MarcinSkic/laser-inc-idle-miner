using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;

[System.Serializable]
public class UpgradeableData<T>
{
    public UpgradeableValues type;
    [ConditionalField(nameof(type),false,UpgradeableValues.Special)]
    public string name;
    [SerializeField] private T value;
    public UnityAction<T> onValueChange;
    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            onValueChange?.Invoke(value);
        }
    }
    
    public static implicit operator T(UpgradeableData<T> data) => data.Value; //Used to simplify bombBallData.speed.value => bombBallData.speed
}