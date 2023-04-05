using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIButtonWithStringController : UIButtonController
{
    [Header("BUTTON WITH STRING")]
    [SerializeField] protected string parameter;
    public new UnityAction<UIButtonController,string> onClick;

    protected override void OnClicked()
    {
        base.OnClicked();
        onClick?.Invoke(this,parameter);
    }
}
