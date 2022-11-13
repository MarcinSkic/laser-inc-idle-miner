using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuView : BaseView
{
    [SerializeField] private UIButtonController gameStartButton;
    public UIButtonController GameStartButton => gameStartButton;
}
