using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIConfirmPopup : MonoBehaviour
{
    [SerializeField] private UIButtonController[] confirmButtons;
    [SerializeField] private UIButtonController[] cancelButtons;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private float delayPreventingAccidentalClosing = 0.4f;

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

    public IEnumerator PreventAccidentalClosing(UIButtonController buttonController, float seconds)
    {
        buttonController.Deactivate();
        yield return new WaitForSeconds(seconds);
        buttonController.Activate();
    }

    public UnityAction<bool> onResponse;

    public void Display(string title = "Are you certain?", string description = "")
    {
        gameObject.SetActive(true);

        foreach(var button in cancelButtons)
        {
            StartCoroutine(PreventAccidentalClosing(button, delayPreventingAccidentalClosing));
        }
        
        this.title.text = title;
        this.description.text = description;
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
