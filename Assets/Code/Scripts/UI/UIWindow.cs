using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public UIButtonWithStringController[] tabButtons;
    public GameObject[] tabs;

    public void Setup()
    {
        foreach (var button in tabButtons)
        {
            button.Init();
            button.onClick += SwitchTab;
        }
    }

    public void Activate()
    {     
        gameObject.SetActive(true);

        if(tabs.Length != 0)
        {
            DisableAllTabs();
            SwitchTab(tabButtons[0], tabs[0].name);
        }
    }

    public void Dectivate()
    {
        gameObject.SetActive(false);
    }

    private void SwitchTab(UIButtonController button, string name)
    {
        DisableAllTabs();

        var newTab = tabs.First(t => t.name == name);
        if(newTab == null)
        {
            Debug.LogWarning($"There is no tab called {name} to be activated, typo in button string parameter?",button);
            return;
        }

        newTab.SetActive(true);
        button.Select();
    }

    private void DisableAllTabs()
    {
        foreach (var tab in tabs)
        {
            tab.SetActive(false);
        }

        foreach(var button in tabButtons)
        {
            button.Deselect();
        }
    }
}
