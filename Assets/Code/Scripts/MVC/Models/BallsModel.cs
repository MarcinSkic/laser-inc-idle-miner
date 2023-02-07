using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using MyBox;

public class BallsModel : MonoBehaviour
{
    public BallData[] ballsDataList;
    public Dictionary<UpgradeableObjects, BallData> ballsData;

    void Awake()
    {
        CreateListOfBalls();
    }

    private void CreateListOfBalls()
    {
        ballsData = new Dictionary<UpgradeableObjects, BallData>() { };

        foreach (var data in ballsDataList)
        {
            data.Init();
            ballsData.Add(data.type, data);
        }
    }
}
