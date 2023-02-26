using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UIToggleController : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private RectTransform handle;
    [SerializeField] private TMP_Text label;

    public bool IsOn
    {
        get
        {
            return toggle.isOn;
        }
        set
        {
            toggle.isOn = value;
        }
    }
    public UnityAction<bool> onValueChanged;
    

    private void Awake()
    {
        Init();
        toggle.onValueChanged.AddListener(ValueChange);
    }

    private void Init()
    {
        ValueChange(IsOn);
    }

    private void ValueChange(bool value)
    {
        IsOn = value;

        handle.anchorMin = IsOn ? new Vector2(0.53f, handle.anchorMin.y) : new Vector2(0, handle.anchorMin.y);
        handle.anchorMax = IsOn ? new Vector2(1, handle.anchorMax.y) : new Vector2(0.47f, handle.anchorMax.y);

        onValueChanged?.Invoke(IsOn);
    }

    public void SetLabel(string value)
    {
        label.text = value;
    }

    private void OnValidate()
    {
        ValueChange(IsOn);
    }
}
