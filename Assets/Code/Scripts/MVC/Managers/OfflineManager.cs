using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;
using MyBox;

[Serializable]
public class OfflineData
{
    public DateTime lastActivitySaveTime;
    public double accumulatedOfflineTime;
}

public class OfflineManager : MonoBehaviour
{
    [InitializationField]
    [SerializeField] private string fileName;
    [SerializeField] private double minimumOfflineTimeInSeconds;
    [SerializeField] private double lastOfflineSeconds;
    

    private DateTime lastOfflineCountCheck = DateTime.MinValue;
    private DateTime lastActivitySaveTime = DateTime.MinValue;
    private DateTime oldTime;

    public bool offlineRewardWasReceived;

    //TODO-FEATURE: Protection against forwarding date

    private void Update()
    {
        if (SettingsModel.Instance.doOfflineEarning)
        {
            if (offlineRewardWasReceived)
            {
                offlineRewardWasReceived = false;
                SaveActivityTime(new Vector2(0, 0));
            }
            else if (DateTime.Now > lastActivitySaveTime.AddSeconds(1))
            {
                Vector2 temp = CountOfflineTime();
                SaveActivityTime(temp);
            }
            else if (DateTime.Now > lastOfflineCountCheck.AddSeconds(0.5))
            {
                CountOfflineTime();
            }
        }
    }

    public UnityAction<double> onReturnFromOffline;
    public Vector2 CountOfflineTime()
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            //Debug.LogError(destination + " not found!");
            return new Vector2(0, 0);
        }

        BinaryFormatter bf = new();
        OfflineData offlineData = (OfflineData) bf.Deserialize(file);
        oldTime = offlineData.lastActivitySaveTime;
        file.Close();
        double offlineSeconds = (DateTime.Now - oldTime).TotalSeconds + offlineData.accumulatedOfflineTime;
        /*Debug.Log("65");
        Debug.Log((DateTime.Now - oldTime).TotalSeconds);
        Debug.Log("67");
        Debug.Log(offlineData.accumulatedOfflineTime);*/
        lastOfflineCountCheck = DateTime.Now;
        double offlineSecondsRounded = 0;
        if (offlineSeconds-lastOfflineSeconds > minimumOfflineTimeInSeconds)
        {
            /*Debug.LogWarning("now - old");
            Debug.LogWarning((DateTime.Now - oldTime).TotalSeconds);
            Debug.LogWarning("acc");
            Debug.LogWarning(offlineData.accumulatedOfflineTime);*/
            offlineSecondsRounded = Math.Round(offlineSeconds, 1);
            onReturnFromOffline.Invoke(offlineSecondsRounded);
        }
        lastOfflineSeconds = offlineSeconds;
        /*Debug.LogError("77");
        Debug.LogError(offlineSecondsRounded);*/
        return new Vector2((float) offlineSecondsRounded, (float)offlineData.accumulatedOfflineTime);
    }

    public void SaveActivityTime(Vector2 data)
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new();

        OfflineData offlineData = new OfflineData();
        offlineData.lastActivitySaveTime = DateTime.Now;
        if (data.x != 0)
        {
            offlineData.accumulatedOfflineTime = data.x;
        } else
        {
            offlineData.accumulatedOfflineTime = data.y;
        }

        bf.Serialize(file, offlineData);
        file.Close();
        //Debug.LogWarning("activity DateTime saved!");
        lastActivitySaveTime = DateTime.Now;
    }
}
