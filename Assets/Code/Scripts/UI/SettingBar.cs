using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingBar : MonoBehaviour
{
    public GameController gameController;
    public string settingName;
    public TMP_Text label;
    public int ignore;

    private void Start()
    {
        label.text = settingName;
    }

    public void ChangeSetting(bool value)
    {
        if(ignore == 1)
        {
            //print("Change skipped (changing to match save)");
            ignore -= 1;
        }
        else
        {
            gameController.ChangeSetting(value, settingName);
        }
    }
}
