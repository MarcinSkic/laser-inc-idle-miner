using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SavingManager : MonoBehaviour
{
    [AutoProperty(AutoPropertyMode.Scene)]
    [SerializeField] private SavingModel model;

    public PersistentData LoadPersistentData()
    {
        string destination = Application.persistentDataPath + "/" + model.fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            return null;
        }

        BinaryFormatter bf = new();
        var persistentData = JsonUtility.FromJson<PersistentData>((string)bf.Deserialize(file));
        file.Close();

        return persistentData;
    }

    public void SavePersistentData(PersistentData data)
    {
        data.lastSaveTime = DateTime.Now;

        string destination = Application.persistentDataPath + "/" + model.fileName;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new();
        var jsonedData = JsonUtility.ToJson(data);
        Debug.Log(jsonedData);

        bf.Serialize(file, jsonedData);
        file.Close();
    }
}