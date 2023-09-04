using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIDayReward : MonoBehaviour
{
    [SerializeField] [AutoProperty(AutoPropertyMode.Children)] UIButtonController button;
    [SerializeField] UIDailyRewardsWindow owner;
    [SerializeField] Image TopBar;
    public TMPro.TextMeshProUGUI title;
    public TMPro.TextMeshProUGUI value;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;
    [SerializeField] double timeToCollect;
    [SerializeField] bool collectable = false;

    public void Enable(UnityAction onClick)
    {
        button.Init();
        button.Select();
        TopBar.color = enabledColor;
        button.onClick += onClick;
        collectable = true;
        button.SetText("Collect");
    }

    public void TickTime()
    {
        if (!collectable)
        {
            timeToCollect--;
            button.SetText(NumberFormatter.FormatSecondsToReadable(timeToCollect));

            if (timeToCollect < 0)
            {
                owner.ConfigureRewards();               
            }
        }
    }

    public void Disable()
    {
        button.Deactivate();
        TopBar.color = disabledColor;
        button.onClick = null;
        collectable = false;      
    }

    public void SetTime(double timeToCollect)
    {
        this.timeToCollect = timeToCollect;

        if (!collectable)
        {
            button.SetText(NumberFormatter.FormatSecondsToReadable(this.timeToCollect));
        }
    }
}
