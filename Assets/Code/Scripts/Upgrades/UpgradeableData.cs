using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeableData<T>
{
    public UpgradeableData (T value) {
        this.value = value;
    }
    public T value;

    public static implicit operator T(UpgradeableData<T> data) => data.value; //Used to simplify bombBallData.speed.value => bombBallData.speed
}