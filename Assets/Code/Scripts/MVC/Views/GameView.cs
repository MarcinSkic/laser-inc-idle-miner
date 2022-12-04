using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : BaseView
{
    [SerializeField] private List<GameObject> topButtonsTabs;
    [SerializeField] private List<UIButtonWithStringController> bottomButtons;

    public List<UIBallBar> ballBars;
    public UIBallBar ballBarPrefab;
    public Transform ballBarsContent;

    private void Start()
    {
        AssignBottomButtonsEvent(); //TODO-FT-CURRENT: Should be in GameController
    }

    public void AssignBottomButtonsEvent()
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

    public void GenerateBallBar()
    {
        var bar = Instantiate(ballBarPrefab, ballBarsContent);
        ballBars.Add(bar);


    }
}
