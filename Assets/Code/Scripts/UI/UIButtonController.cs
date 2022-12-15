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
    [SerializeField] protected TMP_Text text;

    [SerializeField] Color @default;
    [SerializeField] Color activated;

    public void Awake()
    {
        button.onClick.AddListener(OnClicked);
        Activate();
    }

    public void SetActivatedColor()
    {
        button.targetGraphic.color = activated;
    }

    public void SetDefaultColor()
    {
        button.targetGraphic.color = @default;
    }

    public void SetColor(Color color)
    {
        button.targetGraphic.color = color;
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
    protected virtual void OnClicked()
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
