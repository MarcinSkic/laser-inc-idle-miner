using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using TMPro;

public class GameView : BaseView
{
    [Header("Universal")]
    public Color upgradeButton_MaxedUpgrades;

    [Header("Tabs Switching")]
    [SerializeField] private List<GameObject> tabButtonsContainers;
    [SerializeField] private List<UIButtonWithStringController> windowButtons;
    [SerializeField] private List<GameObject> windows;

    public Color bottomButton_Default;
    public Color bottomButton_Activated;

    [Header("Lasers Tab")]
    public UIBallBar ballBarPrefab;
    public Transform ballBarsParent;

    [Header("Debug Window")]
    public GameObject debugWindow;
    public TMP_Text fpsDisplay;
    public TMP_Text avg_FpsDisplay;

    [Header("DEBUG")]
    public List<UIBallBar> ballBars;

    private void Start()
    {
        InitBottomButtonsEvent(); //TODO-FT-CURRENT: Should be in GameController
    }

    UnityAction<Color> onTabClosing;
    public void InitBottomButtonsEvent()
    {
        foreach(var tabButtonsContainer in tabButtonsContainers)
        {
            foreach(var tabButton in tabButtonsContainer.GetComponentsInChildren<UIButtonWithStringController>())
            {
                tabButton.Init();
                tabButton.onClick += SwitchTab;
                onTabClosing += tabButton.SetColor;
            }
        }

        foreach (var windowButton in windowButtons)
        {
            windowButton.Init();
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

    public void CreateBallBar(BaseBallData ballType)
    {
        var ballBar = Instantiate(ballBarPrefab, ballBarsParent);
        ballBar.SetUpgradesName(ballType.name);
        ballBar.ballIcon.sprite = ballType.sprite;

        ballBars.Add(ballBar);
    }
}
