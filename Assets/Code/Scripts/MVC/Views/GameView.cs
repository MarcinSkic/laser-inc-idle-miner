using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : BaseView
{
    [SerializeField] private List<GameObject> topButtonsTabs;
    [SerializeField] private List<UIButtonWithStringController> bottomButtons;

    private void Start()
    {
        AssignBottomButtonsEvent();
    }

    private void AssignBottomButtonsEvent()
    {
        foreach (var button in bottomButtons)
        {
            button.onClick += SwitchTab;
        }
    }

    private void SwitchTab(string name)
    {
        foreach(var tab in topButtonsTabs)
        {
            tab.SetActive(false);
        }

        topButtonsTabs.Find(tab => tab.name == name).SetActive(true);
    }
}
