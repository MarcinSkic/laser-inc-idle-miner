using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class GameView : BaseView
{
    [SerializeField] private List<GameObject> tabButtonsContainers;
    [SerializeField] private List<UIButtonWithStringController> windowButtons;
    [SerializeField] private List<GameObject> windows;

    public Color bottomButton_Default;
    public Color bottomButton_Activated;
    public Color upgradeButton_MaxedUpgrades;

    public UIBallBar ballBarPrefab;
    public Transform ballBarsParent;

    [Header("Debug")]
    public List<UIBallBar> ballBars;

    private void Start()
    {
        AssignBottomButtonsEvent(); //TODO-FT-CURRENT: Should be in GameController
    }

    UnityAction<Color> onTabClosing;
    public void AssignBottomButtonsEvent()
    {
        foreach(var tabButtonsContainer in tabButtonsContainers)
        {
            foreach(var tabButton in tabButtonsContainer.GetComponentsInChildren<UIButtonWithStringController>())
            {
                tabButton.onClick += SwitchTab;
                onTabClosing += tabButton.SetColor;
            }
        }

        foreach (var windowButton in windowButtons)
        {
            windowButton.onClick += SwitchWindowButtons;
        }
    }

    private void SwitchTab(UIButtonController button,string name)
    {
        foreach (var window in windows)
        {
            var foundTab = window.transform.Find(name).gameObject;

            if (foundTab != null)
            {
                bool previousTabState = foundTab.activeSelf;
                DisableAllTabs();

                if (!previousTabState)
                {
                    foundTab.SetActive(true);
                    button.SetColor(bottomButton_Activated);
                } 
                else
                {
                    foundTab.SetActive(false);
                    button.SetColor(bottomButton_Default);
                }
                return;
            }
        }

        Debug.LogWarningFormat("Couldn't find tab of name {} to be activated by SwitchWindow method", name);
    }

    private void DisableAllTabs()
    {
        foreach (var window in windows)
        {
            foreach (Transform tab in window.transform)
            {
                tab.gameObject.SetActive(false);
            }
        }

        onTabClosing.Invoke(bottomButton_Default);
    }

    private void SwitchWindowButtons(UIButtonController button,string name)
    {
        foreach(var tabButtonsContainer in tabButtonsContainers)
        {
            tabButtonsContainer.SetActive(false);
        }
        DisableAllTabs();

        tabButtonsContainers.Find(tabButtonsContainer => tabButtonsContainer.name == name).SetActive(true);
    }
}
