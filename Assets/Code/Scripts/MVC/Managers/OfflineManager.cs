using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;
using MyBox;

public class OfflineManager : MonoBehaviour
{
    [InitializationField]
    [SerializeField] private string fileName;
    [SerializeField] private double minimumOfflineTimeInSeconds;
    

    private DateTime lastOfflineCountCheck = DateTime.MinValue;
    private DateTime lastActivitySaveTime = DateTime.MinValue;
    private DateTime oldTime;

    //TODO-FEATURE: Protection against forwarding date

    private void Update()
    {
        if (SettingsModel.Instance.doOfflineEarning)
        {
            if (DateTime.Now > lastOfflineCountCheck.AddSeconds(0.5))
            {
                CountOfflineTime();
            }
            if (DateTime.Now > lastActivitySaveTime.AddSeconds(1))
            {
                CountOfflineTime();
                SaveActivityTime();
            }
        }
    }

    public UnityAction<double> onReturnFromOffline;
    public void CountOfflineTime()
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            //Debug.LogError(destination + " not found!");
            return;
        }

        BinaryFormatter bf = new();
        oldTime = (DateTime) bf.Deserialize(file);
        file.Close();
        double offlineSeconds = (DateTime.Now - oldTime).TotalSeconds;
        //Debug.Log(offlineSeconds);
        lastOfflineCountCheck = DateTime.Now;

        if (offlineSeconds > minimumOfflineTimeInSeconds)
        {
            double offlineSecondsRounded = Math.Round(offlineSeconds, 1);

            onReturnFromOffline.Invoke(offlineSecondsRounded);
        }
    }

    public void SaveActivityTime()
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new();
        bf.Serialize(file, DateTime.Now);
        file.Close();
        //Debug.LogWarning("activity DateTime saved!");
        lastActivitySaveTime = DateTime.Now;
    }
}
