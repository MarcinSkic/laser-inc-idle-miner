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

    #region BlocksModel

    #endregion

    #region UpgradesModel
    public PersistentUpgrade[] upgrades;
    #endregion

    #region GameModel
    public double depth;
    #endregion

    #region SettingsModel
    public bool is60fps;
    public bool displayFloatingText;
    #endregion

    public PersistentData()
    {

    }
}