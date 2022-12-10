using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameView : BaseView
{
    [SerializeField] private List<GameObject> topButtonsTabs;
    [SerializeField] private List<UIButtonWithStringController> bottomButtons;
    [SerializeField] private List<GameObject> tabs;

    public List<UIBallBar> ballBars;
    public UIBallBar ballBarPrefab;
    public Transform ballBarsParent;

    private void Start()
    {
        AssignBottomButtonsEvent(); //TODO-FT-CURRENT: Should be in GameController
    }

    public void AssignBottomButtonsEvent()
    {
        /*foreach(var topButtonsTab in topButtonsTabs)
        {
            foreach(var button in topButtonsTab.GetComponentsInChildren<UIButtonWithStringController>())
            {
                button.onClick += SwitchWindow;
            }
        }*/

        foreach (var button in bottomButtons)
        {
            button.onClick += SwitchButtonTab;
        }
    }

    private void SwitchWindow(string name)
    {
        foreach(var tab in tabs)
        {
            foreach(var insideTab in tab.GetComponentsInChildren<GameObject>())
            {
                insideTab.SetActive(false);
            }
        }

        foreach (var tab in tabs)
        {
            var insideTab = tab.GetComponentsInChildren<GameObject>().ToList().Find(insideTab => insideTab.name == name);
            insideTab.SetActive(!insideTab.activeSelf);
        }
    }

    private void SwitchButtonTab(string name)
    {
        foreach(var tab in topButtonsTabs)
        {
            tab.SetActive(false);
        }

        topButtonsTabs.Find(tab => tab.name == name).SetActive(true);
    }

    public void GenerateBallBar()
    {
        var bar = Instantiate(ballBarPrefab, ballBarsParent);
        ballBars.Add(bar);


    }
}
