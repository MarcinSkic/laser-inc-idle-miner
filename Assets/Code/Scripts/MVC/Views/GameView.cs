using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using MyBox;

public class GameView : BaseView
{
    [Header("UNIVERSAL")]
    public Color upgradeButton_MaxedUpgrades;
    public Sprite moneyIcon;
    public Sprite prestigeCurrencyIcon;
    public Sprite premiumCurrencyIcon;
    public Sprite lockedIcon;

    [Space(10)]
    [Header("IN GAME")]
    [Header("Top Bar")]
    [SerializeField] private TMP_Text moneyDisplay;

    [Header("Tabs Switching")]
    [SerializeField] private List<GameObject> tabButtonsContainers;
    [SerializeField] private List<UIButtonWithStringController> legacyWindowButtons;
    [SerializeField] private List<GameObject> legacyWindows;

    [SerializeField] private UIWindow[] windows;
    [SerializeField] private UIButtonWithStringController[] windowButtons;
    public GameObject cheatWindowButton;

    public Color bottomButton_Default;
    public Color bottomButton_Activated;

    [Header("Depth Meter")]
    public UIDepthMeter depthMeter;

    [Header("PowerUps")]
    public UIPowerUp damagePowerUp;

    [Header("Offline popup")]
    public GameObject offlinePopup;
    public TMP_Text offlineText;
    public TMP_Text offlineMoney;
    public UIButtonController offlineConfirmButton;
    public UIButtonController offlineGetBonusButton;

    [Header("Achievement popup")]
    public AchievementPopup achievementPopup;
    public VerticalLayoutGroup achievementsParent;

    [Space(10)]
    [Header("TABS/WINDOWS")]
    [Header("Lasers Tab")]
    public UIBallBar ballBarPrefab;
    public Transform ballBarsParent;

    [Header("Upgrades Tab")]
    public UIUpgradeBar upgradeBarPrefab;
    public Transform upgradeBarsParent;

    [Header("Choice Upgrades Tab")]
    public Transform choiceUpgradeBarsParent;

    [Header("Prestige Upgrades Tab")]
    public TMP_Text[] prestigeCurrencyDisplays;
    public Transform prestigeUpgradeBarsParent;

    [Header("Settings Tab")]
    public UIToggleController is60fps;
    public UIToggleController displayFloatingDamage;
    public UIToggleController useAlternativeNotation;
    public UIButtonController eraseSaveFile;


    [Header("Cheats Tab")]
    public UIToggleController showDebugWindow;
    public UIButtonController cheatMoney;
    public UIButtonController cheatDepth;
    public UIButtonController forcePrestige;
    public UIButtonController cheatSpeedUp;
    public UIButtonController cheatSlowDown;

    [Header("AchievementsTab")]
    public AchievementSquare achievementPrefab;
    public AchievementTooltip achievementTooltip;

    private List<AchievementSquare> achievementSquares;
    [SerializeField] int achievementsInRow;
    [SerializeField] GridLayoutGroup achievementsGlp;

    [Header("Debug Window")]
    public GameObject debugWindow;
    public TMP_Text fpsDisplay;
    public TMP_Text avg_FpsDisplay;
    public TMP_Text blocksHpDisplay;

    [Header("DEBUG")]
    [ReadOnly]
    public List<UIBallBar> ballBars;

    public void InitButtons()
    {
        offlineConfirmButton.Init();
        offlineGetBonusButton.Init();
        cheatMoney.Init();
        cheatDepth.Init();
        forcePrestige.Init();
        eraseSaveFile.Init();
        cheatSpeedUp.Init();
        cheatSlowDown.Init();
    }

    #region WINDOWS-TABS SYSTEM
    private UIWindow activeWindow = null;

    public void InitWindows()
    {
        foreach(var window in windows)
        {
            window.Setup();

        }

        foreach(var button in windowButtons)
        {
            button.Init();
            button.onClick += SwitchWindow;
        }

        SelectBottomWindowButtons();
    }

    private void SwitchWindow(UIButtonController button, string name)
    {
        if (activeWindow != null && name == activeWindow.name)
        {
            DeselectAllWindowButtons();
            activeWindow.Dectivate();
            activeWindow = null;
            SelectBottomWindowButtons();
            return;
        }

        var newWindow = windows.First(w => w.name == name);
        if(newWindow == null)
        {
            Debug.LogWarning($"There is no window called {name} to be activated, typo in button string parameter?", button);
            return;
        }

        if(activeWindow == null)
        {
            DeselectAllWindowButtons();
            activeWindow = newWindow;
            activeWindow.Activate();
            button.Select();
            return;
        }

        if(activeWindow != null && name != activeWindow.name)
        {
            DeselectAllWindowButtons();
            activeWindow.Dectivate();
            activeWindow = newWindow;
            activeWindow.Activate();
            button.Select();
            return;
        }
    }

    private void SelectBottomWindowButtons()
    {
        foreach (var b in windowButtons)
        {
            if (!b.name.Contains("Setting") && !b.name.Contains("Cheat"))
            {
                b.Select();
            }        
        }
    }

    private void DeselectAllWindowButtons()
    {
        foreach (var b in windowButtons)
        {
            b.Deselect();
        }
    }

    public void DisableAllWindows()
    {
        foreach(var window in windows)
        {
            window.Dectivate();
        }
    }

    #endregion
    public void CreateBallBar(BallData ballData)
    {
        var ballBar = Instantiate(ballBarPrefab, ballBarsParent);
        ballBar.Init(ballData);

        ballBars.Add(ballBar);
    }

    public UIUpgradeBar CreateUpgradeBar(Upgrade upgrade)
    {
        UIUpgradeBar upgradeBar = null;
        switch (upgrade.whereToGenerate)
        {
            case UISection.Normal:
                upgradeBar = Instantiate(upgradeBarPrefab, upgradeBarsParent);
                break;
            case UISection.Prestige:
                upgradeBar = Instantiate(upgradeBarPrefab, prestigeUpgradeBarsParent);
                break;
            case UISection.Choice:
                upgradeBar = Instantiate(upgradeBarPrefab, choiceUpgradeBarsParent);
                break;
        }

        switch (upgrade.currency)
        {
            case Currency.Money:
                upgradeBar.SetCurrencySprite(moneyIcon);
                break;
            case Currency.Prestige:
                upgradeBar.SetCurrencySprite(prestigeCurrencyIcon);
                break;
            case Currency.Premium:
                upgradeBar.SetCurrencySprite(premiumCurrencyIcon);
                break;
        }

        upgradeBar.SetToUnlockDescription(upgrade.toUnlockDescription);
        upgradeBar.SetDescription(upgrade.description);
        upgradeBar.SetLevel(upgrade);
        upgradeBar.SetTitle(upgrade.title);
        upgradeBar.UpgradeButton.upgradeName = upgrade.name;

        return upgradeBar;
    }

    public void ShowOfflineTimePopup(double seconds,double earnedMoney)
    {
        offlinePopup.SetActive(true);
        offlineText.text = $"You were offline for <color=#0bf>{seconds}</color>!";
        SetOfflineMoney(earnedMoney);
    }

    public void InitAchievementsWindow()
    {
        achievementSquares = new List<AchievementSquare>();
        SetAchievementSquaresWidth();
    }

    public void CreateAchievement(Achievement achievement)
    {
        AchievementSquare achievementSquareInstance = Instantiate(achievementPrefab, achievementsGlp.transform);

        achievementSquareInstance.Init(achievement);
        achievementSquareInstance.onAchievementClicked += DisplayAchievement;

        achievementSquares.Add(achievementSquareInstance);

        achievement.onAchievementUnlocked += achievementSquareInstance.SetColor;

        
    }

    private void DisplayAchievement(Achievement achievement)
    {
        MessagesManager.Instance.DisplayMessage(achievement.name, achievement.description,achievement.sprite, achievement.isCompleted ? Color.white : new Color(0.1875f, 0.1875f, 0.1875f, 1));
    }

    private void SetAchievementSquaresWidth()
    {
        int squareWidth = Mathf.RoundToInt(1120 / (achievementsInRow + 0.5f));
        int squareSpacing = (1120 - achievementsInRow * squareWidth) / (achievementsInRow - 1);
        achievementsGlp.spacing = new Vector2(squareSpacing, squareSpacing);
        achievementsGlp.cellSize = new Vector2(squareWidth, squareWidth);
    }

    public void CreateAchievementPopup(Achievement achievement)
    {
        AchievementPopup ap = Instantiate(achievementPopup, achievementsParent.transform);
        ap.Init(achievement,achievementsParent);
    }

    public void SetOfflineMoney(double earnedMoney)
    {
        offlineMoney.text = string.Format("You made <color=#da0>{0:F0}</color>$ when away!", earnedMoney);
    }

    public void SetMoneyDisplay(double value)
    {
        moneyDisplay.text = NumberFormatter.Format(value);
    }

    public void SetPrestigeCurrencyDisplay(double value)
    {
        foreach(var display in prestigeCurrencyDisplays)
        {
            display.text = NumberFormatter.Format(value);
        }
    }

    public void SetBlocksHpDisplay(double value)
    {
        blocksHpDisplay.text = string.Format("block hp: {0:F2}", value);
    }
}
