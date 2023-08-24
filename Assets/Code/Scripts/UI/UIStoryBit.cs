using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIStoryBit : MonoBehaviour
{
    public TextMeshProUGUI text;
    public UIButtonController[] closePopup;
    public string story;

    private bool soundToPlay = true;
    private string currentText = "";
    public void Show()
    {
        gameObject.SetActive(true);

        foreach (var p in closePopup)
        {
            p.Init();
            p.onClick += Hide;
        }

        if (soundToPlay)
        {
            AudioManager.Instance.Play("robot");
            soundToPlay = false;
        }

        
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
