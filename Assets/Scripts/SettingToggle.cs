using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{

    public GameController gameController;
    public SettingBar settingBar;

    private void Start()
    {
        gameController = settingBar.gameController;
    }
    

    public void ChangeSetting(bool value)
    {
        settingBar.ChangeSetting(value);
    }
}
