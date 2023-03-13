using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIConfirmPopup : MonoBehaviour
{
    [SerializeField] private UIButtonController[] confirmButtons;
    [SerializeField] private UIButtonController[] cancelButtons;
    [SerializeField] private TMP_Text message;

    public void Init()
    {
        foreach(var button in confirmButtons)
        {
            button.Init();
            button.onClick += Confirm;
        }
        foreach(var button in cancelButtons)
        {
            button.Init();
            button.onClick += Cancel;
        }

        gameObject.SetActive(false);
    }

    public UnityAction<bool> onResponse;

    public void Display(string message = "")
    {
        gameObject.SetActive(true);
        this.message.text = message;
    }

    private void Confirm()
    {
        onResponse?.Invoke(true);
        Close();
    }

    private void Cancel()
    {
        onResponse?.Invoke(false);
        Close();
    }

    private void Close()
    {
        onResponse = null;
        gameObject.SetActive(false);
    }
}
