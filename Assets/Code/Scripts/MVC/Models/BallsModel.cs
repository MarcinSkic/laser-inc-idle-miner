using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using MyBox;

public class BallsModel : MonoBehaviour
{
    public BallDataScriptable[] ballDataScriptables;
    public Dictionary<UpgradeableObjects, BallData> ballsData;
    public Dictionary<UpgradeableObjects, UpgradeableData<int>> ballsCount;

    void Awake()
    {
        CreateListOfBalls();
    }

    private void CreateListOfBalls()
    {
        ballsData = new Dictionary<UpgradeableObjects, BallData>() { };
        ballsCount = new Dictionary<UpgradeableObjects, UpgradeableData<int>>();

        foreach (var data in ballDataScriptables)
        {
            var ballData = data.BallData;
            ballData.Init();
            ballsData.Add(ballData.type, ballData);

            UpgradeableData<int> ballCount = new (0);
            ballCount.onValueChange += v => { Debug.Log($"Count changed for {ballData.type} = {v}"); };
            ballsCount.Add(ballData.type, ballCount);
        }
    }
}
