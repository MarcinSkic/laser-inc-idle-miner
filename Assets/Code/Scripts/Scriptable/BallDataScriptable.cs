using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallData", menuName = "ScriptableObjects/BallData", order = 4)]
public class BallDataScriptable : ScriptableObject
{
    [SerializeField]
    private BallData ballData;
    public BallData BallData
    {
        get => Functions.GetObjectCopy(ballData);
        set => ballData = value;
    }
}
