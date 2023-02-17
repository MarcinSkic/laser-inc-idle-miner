using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
    [Header("BUTTON BASE")]
    [SerializeField] protected Button button;
    [Tooltip("Optional")]
    [SerializeField] protected TMP_Text text;
    [SerializeField] protected bool hardDeactivate;

    public virtual void Init()
    {
        button.onClick.AddListener(OnClicked);
        Activate();
        hardDeactivate = false;
    }

    public void SetColor(Color color)
    {
        button.targetGraphic.color = color;
    }

    public void Activate()
    {
        if (!hardDeactivate)
        {
            button.interactable = true;
        }
    }

    public void Deactivate()
    {
        button.interactable = false;
    }

    public void SetHardDeactivate(bool state)
    {
        hardDeactivate = state;
    }

    public UnityAction onClick;
    protected virtual void OnClicked()
    {
        onClick?.Invoke();
    }

    public void SetText(string value)
    {
        text.text = value;
    }

    public void RemoveAllEvents()
    {
        onClick = null;
    }
}
