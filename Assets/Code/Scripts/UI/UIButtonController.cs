using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MyBox;
using System;

[Flags]
public enum TransitionType { SpriteSwap = 0b1, ObjectSwap = 0b10, FontColorSwap=0b100, ImageColorSwap=0b1000, IconSpriteSwap=0b10000}

public class UIButtonController : MonoBehaviour
{
    [Header("BUTTON BASE")]
    [SerializeField] protected Button button;
    [Tooltip("Optional")]
    [SerializeField] protected TMP_Text text;
    [SerializeField] protected bool hardDeactivate;
    [SerializeField] protected TransitionType transitionType;

    [ConditionalField(nameof(transitionType),false,TransitionType.SpriteSwap)]
    [SerializeField] protected Sprite defaultSprite;
    [ConditionalField(nameof(transitionType), false, TransitionType.SpriteSwap)]
    [SerializeField] protected Sprite selectedSprite;

    [ConditionalField(nameof(transitionType), false, TransitionType.IconSpriteSwap)]
    [SerializeField] protected Image icon;
    [ConditionalField(nameof(transitionType), false, TransitionType.IconSpriteSwap)]
    [SerializeField] protected Sprite defaultIconSprite;
    [ConditionalField(nameof(transitionType), false, TransitionType.IconSpriteSwap)]
    [SerializeField] protected Sprite selectedIconSprite;

    [ConditionalField(nameof(transitionType),false,TransitionType.ObjectSwap)]
    [SerializeField] protected GameObject defaultObject;
    [ConditionalField(nameof(transitionType), false, TransitionType.ObjectSwap)]
    [SerializeField] protected GameObject selectedObject;

    [ConditionalField(nameof(transitionType),false,TransitionType.FontColorSwap)]
    [SerializeField] protected Color defaultFont;
    [ConditionalField(nameof(transitionType), false, TransitionType.FontColorSwap)]
    [SerializeField] protected Color selectedFont;

    [ConditionalField(nameof(transitionType), false, TransitionType.ImageColorSwap)]
    [SerializeField] protected Color defaultColor;
    [ConditionalField(nameof(transitionType), false, TransitionType.ImageColorSwap)]
    [SerializeField] protected Color selectedColor;

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
            Select();
        }
    }

    public void Deactivate()
    {
        button.interactable = false;
        Deselect();
    }

    public void SetHardDeactivate(bool state)
    {
        hardDeactivate = state;
    }

    public void Select()
    {
        if (transitionType.HasFlag(TransitionType.SpriteSwap))
        {
            button.image.sprite = selectedSprite;
        }

        if (transitionType.HasFlag(TransitionType.ObjectSwap))
        {
            defaultObject.SetActive(false);
            selectedObject.SetActive(true);
        }

        if (transitionType.HasFlag(TransitionType.FontColorSwap))
        {
            text.color = selectedFont;
        }

        if (transitionType.HasFlag(TransitionType.ImageColorSwap))
        {
            button.image.color = selectedColor;
        }

        if (transitionType.HasFlag(TransitionType.IconSpriteSwap))
        {
            icon.sprite = selectedIconSprite;
        }
    }

    public void Deselect()
    {
        if (transitionType.HasFlag(TransitionType.SpriteSwap))
        {
            button.image.sprite = defaultSprite;
        }

        if (transitionType.HasFlag(TransitionType.ObjectSwap))
        {
            selectedObject.SetActive(false);
            defaultObject.SetActive(true);
        }

        if (transitionType.HasFlag(TransitionType.FontColorSwap))
        {
            text.color = defaultFont;
        }

        if (transitionType.HasFlag(TransitionType.ImageColorSwap))
        {
            button.image.color = defaultColor;
        }

        if (transitionType.HasFlag(TransitionType.IconSpriteSwap))
        {
            icon.sprite = defaultIconSprite;
        }
    }

    public UnityAction onClick;
    protected virtual void OnClicked()
    {
        onClick?.Invoke();
        AudioManager.Instance.Play("button_click");
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
