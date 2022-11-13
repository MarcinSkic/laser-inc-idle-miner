using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    public void Awake()
    {
        button.onClick.AddListener(OnClicked);
        Activate();
    }

    public void Activate()
    {
        button.interactable = true;
    }

    public void Deactivate()
    {
        button.interactable = false;
    }

    public UnityAction onClick;
    public void OnClicked()
    {
        onClick?.Invoke();
    }

    public void ChangeText(string value)
    {
        text.text = value;
    }

    public void RemoveAllEvents()
    {
        onClick = null;
    }
}
