using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using TMPro;

public class GameView : BaseView
{
    [Header("UNIVERSAL")]
    public Color upgradeButton_MaxedUpgrades;

    [Space(10)]
    [Header("IN GAME")]
    [Header("Top Bar")]
    [SerializeField] private TMP_Text moneyDisplay;

    [Header("Tabs Switching")]
    [SerializeField] private List<GameObject> tabButtonsContainers;
    [SerializeField] private List<UIButtonWithStringController> windowButtons;
    [SerializeField] private List<GameObject> windows;

    public Color bottomButton_Default;
    public Color bottomButton_Activated;

    [Header("Depth Meter")]
    public UIDepthMeter depthMeter;

    [Header("Offline popup")]
    public GameObject offlinePopup;
    public TMP_Text offlineText;
    public TMP_Text offlineMoney;
    public UIButtonController offlineConfirmButton;
    public UIButtonController offlineGetBonusButton;

    [Space(10)]
    [Header("TABS/WINDOWS")]
    [Header("Lasers Tab")]
    public UIBallBar ballBarPrefab;
    public Transform ballBarsParent;

    [Header("Debug Window")]
    public GameObject debugWindow;
    public TMP_Text fpsDisplay;
    public TMP_Text avg_FpsDisplay;
    public TMP_Text blocksHpDisplay;

    [Header("DEBUG")]
    public List<UIBallBar> ballBars;

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

    public void InitButtons()
    {
        offlineConfirmButton.Init();
        offlineGetBonusButton.Init();
    }

    private void SwitchTab(UIButtonController button,string name)
    {
        foreach (var window in windows)
        {
            var foundTab = window.transform.Find(name)?.gameObject;

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
        foreach(var tabButtonsContainerInForeach in tabButtonsContainers)
        {
            tabButtonsContainerInForeach.SetActive(false);
        }
        DisableAllTabs();

        var tabButtonsContainer = tabButtonsContainers.Find(tabButtonsContainer => tabButtonsContainer.name == name);
        tabButtonsContainer.SetActive(true);

        //If in window there is only one tab, it should be automatically opened
        if (tabButtonsContainer.transform.childCount == 1)   
        {
            var tabButton = tabButtonsContainer.GetComponentInChildren<UIButtonWithStringController>();
            SwitchTab(tabButton, tabButton.name);
        }
    }

    public void CreateBallBar(BaseBallData ballType)
    {
        var ballBar = Instantiate(ballBarPrefab, ballBarsParent);
        ballBar.SetUpgradesName(ballType.name);
        ballBar.ballIcon.sprite = ballType.sprite;

        ballBars.Add(ballBar);
    }

    public void ShowOfflineTimePopup(double seconds,double earnedMoney)
    {
        offlinePopup.SetActive(true);
        offlineText.text = $"You were offline for <color=#0bf>{seconds}</color>!";
        SetOfflineMoney(earnedMoney);
    }

    public void SetOfflineMoney(double earnedMoney)
    {
        offlineMoney.text = string.Format("You made <color=#da0>{0:F0}</color>$ when away!", earnedMoney);
    }

    public void SetMoneyDisplay(double value)
    {
        moneyDisplay.text = string.Format("Money: {0:F0}",value);
    }

    public void SetBlocksHpDisplay(double value)
    {
        blocksHpDisplay.text = string.Format("block hp: {0:F2}", value);
    }
}
