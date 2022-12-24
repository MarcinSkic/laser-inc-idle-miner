using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class OfflineManager : MonoBehaviour
{
    public GameObject OfflinePopup;
    public TextMeshProUGUI offlineText;

    private DateTime lastOfflineCountCheck = DateTime.MinValue;
    private DateTime lastActivitySaveTime = DateTime.MinValue;
    public DateTime oldTime;

    [SerializeField] string fileName;
    [SerializeField] double minimumOfflineTime;

    // TODO ZABEZPIECZENIA

    private void Update()
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

        BinaryFormatter bf = new BinaryFormatter();
        oldTime = (DateTime) bf.Deserialize(file);
        file.Close();
        double offlineSeconds = (DateTime.Now - oldTime).TotalSeconds;
        //Debug.Log(offlineSeconds);
        lastOfflineCountCheck = DateTime.Now;

        if (offlineSeconds > minimumOfflineTime)
        {
            double offlineSecondsRounded = Math.Round(offlineSeconds, 1);
            // TODO ACTUALLY COUNT AND ADD MONEY
            double moneyMade = Math.Round(3 * offlineSeconds, 1);
            offlineText.text = $"You were offline for <color=#0bf>{offlineSecondsRounded}</color>! You made <color=#da0>{moneyMade}</color> when away!";
            OfflinePopup.SetActive(true);
        }
    }

    public void SaveActivityTime()
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, DateTime.Now);
        file.Close();
        //Debug.LogWarning("activity DateTime saved!");
        lastActivitySaveTime = DateTime.Now;
    }
}
