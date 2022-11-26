using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIButtonWithStringController : UIButtonController
{
    [SerializeField] protected string parameter;
    public new UnityAction<string> onClick;

    protected override void OnClicked()
    {
        onClick?.Invoke(parameter);
    }
}
