using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UI;

public class UIDayReward : MonoBehaviour
{
    [SerializeField] [AutoProperty(AutoPropertyMode.Children)] UIButtonController button;
    [SerializeField] Image TopBar;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;

    public void Enable()
    {
        button.Init();
        button.Select();
        TopBar.color = enabledColor;
    }

    public void Disable()
    {
        button.Deactivate();
        TopBar.color = disabledColor;
    }
}
