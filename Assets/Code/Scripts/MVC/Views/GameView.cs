using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameView : BaseView
{
    [SerializeField] private List<GameObject> tabButtonsContainers;
    [SerializeField] private List<UIButtonWithStringController> windowButtons;
    [SerializeField] private List<GameObject> windows;

    public UIBallBar ballBarPrefab;
    public Transform ballBarsParent;

    [Header("Debug")]
    public List<UIBallBar> ballBars;

    private void Start()
    {
        AssignBottomButtonsEvent(); //TODO-FT-CURRENT: Should be in GameController
    }

    public void AssignBottomButtonsEvent()
    {
        foreach(var tabButtonsContainer in tabButtonsContainers)
        {
            foreach(var tabButton in tabButtonsContainer.GetComponentsInChildren<UIButtonWithStringController>())
            {
                tabButton.onClick += SwitchTab;
            }
        }

        foreach (var windowButton in windowButtons)
        {
            windowButton.onClick += SwitchWindowButtons;
        }
    }

    private void SwitchTab(string name)
    {
        bool tabState;

        foreach (var window in windows)
        {
            var foundTab = window.transform.Find(name).gameObject;

            if (foundTab != null)
            {
                tabState = foundTab.activeSelf;
                DisableAllTabs();
                foundTab.SetActive(!tabState);
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
    }

    private void SwitchWindowButtons(string name)
    {
        foreach(var tabButtonsContainer in tabButtonsContainers)
        {
            tabButtonsContainer.SetActive(false);
        }

        tabButtonsContainers.Find(tabButtonsContainer => tabButtonsContainer.name == name).SetActive(true);
    }
}
