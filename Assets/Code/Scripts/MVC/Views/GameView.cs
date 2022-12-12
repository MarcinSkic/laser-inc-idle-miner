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
        foreach(var tabButtonContainer in tabButtonsContainers)
        {
            Debug.Log(tabButtonContainer, tabButtonContainer);

            foreach(var button in tabButtonContainer.GetComponentsInChildren<UIButtonWithStringController>())
            {
                Debug.Log(button, button);
                button.onClick += SwitchWindow;
            }
        }

        foreach (var button in windowButtons)
        {
            button.onClick += SwitchButtonTab;
        }
    }

    

    private void SwitchWindow(string name)
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

    private void SwitchButtonTab(string name)
    {
        foreach(var windowTabButtons in tabButtonsContainers)
        {
            windowTabButtons.SetActive(false);
        }

        tabButtonsContainers.Find(windowTabButtons => windowTabButtons.name == name).SetActive(true);
    }

    public void GenerateBallBar()
    {
        var bar = Instantiate(ballBarPrefab, ballBarsParent);
        ballBars.Add(bar);


    }
}
