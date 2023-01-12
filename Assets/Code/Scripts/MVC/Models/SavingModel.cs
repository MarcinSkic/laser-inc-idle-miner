using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingModel : MonoBehaviour
{
    public string fileName = "save.dat";
}

[System.Serializable]
public class PersistentData
{
    #region General
    public DateTime lastSaveTime;
    #endregion

    #region ResourceModel
    public double money;
    #endregion



    public PersistentData()
    {

    }
}