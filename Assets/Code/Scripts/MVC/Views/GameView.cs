using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using MyBox;
using System;

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
    [SerializeField] private TMP_Text moneyCurrencyDisplay;
    [SerializeField] private TMP_Text premiumCurrencyDisplay;

    [Header("Tabs Switching")]
    [SerializeField] private List<GameObject> tabButtonsContainers;
    [SerializeField] private UIWindow[] windows;
    [SerializeField] private UIButtonWithStringController[] windowButtons;
    public GameObject cheatWindowButton;

    public Color bottomButton_Default;
    public Color bottomButton_Activated;

    [Header("Worlds Switching")]
    public UIButtonWithStringController dysonSwarmButton;

    [Header("Dyson Swarm")]
    public UIStoryBit dysonSwarmStory;
    public GameObject dysonSwarmDescription;
    public UIButtonController dysonSwarmStoryOpenButton;
    public TMP_Text dysonSwarmTitle;

    [Header("Depth Meter")]
    public UIDepthMeter depthMeter;

    [Header("PowerUps")]
    public UIPowerUp damagePowerUp;

    [Header("Floating texts")]
    public GameObject floatingTextsDisplay;

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
    public TMP_InputField username;
    public UIToggleController is60fps;
    public UIToggleController displayFloatingDamage;
    public UIToggleController useAlternativeNotation;
    public UIToggleController playMusic;
    public UIToggleController playSounds;
    public UIButtonController eraseSaveFile;
    public UIButtonController rateUs;
    public UIButtonController facebook;
    public UIButtonController instagram;
    public UIButtonController termsOfUse;
    public UIButtonController privacyPolicy;
    public UIButtonController restorePurchases;

    [Header("Cheats Tab")]
    public UIToggleController showDebugWindow;
    public UIButtonController cheatMoney;
    public UIButtonController cheatDepth;
    public UIButtonController forcePrestige;
    public UIButtonController cheatSpeedUp;
    public UIButtonController cheatSlowDown;
    public UIButtonController cheatToggleBatsSpawn;

    [Header("AchievementsTab")]
    public AchievementSquare achievementPrefab;

    private List<AchievementSquare> achievementSquares;
    [SerializeField] GridLayoutGroup achievementsGlp;

    [Header("Debug Window")]
    public GameObject debugWindow;
    public TMP_Text fpsDisplay;
    public TMP_Text avg_FpsDisplay;
    public TMP_Text blocksHpDisplay;

    [Header("DEBUG")]
    [ReadOnly]
    public List<UIBallBar> ballBars;

    public Coroutine flashingDysonSwarmButtonCoroutine; 

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
        cheatToggleBatsSpawn.Init();
        dysonSwarmButton.Init();
        dysonSwarmButton.Deselect();

        facebook.Init();
        facebook.onClick += () => { Application.OpenURL("https://www.facebook.com/profile.php?id=100091986607863"); };

        instagram.Init();
        instagram.onClick += () => { Application.OpenURL("https://www.instagram.com/laserinc.idleminer/?fbclid=IwAR2kcIsm0G-7cLZTtM9f9PJw39UYG6oPGZrcV8E2YBqbUdkyABgHWqdET9w"); };

        termsOfUse.Init();
        termsOfUse.onClick += () => { Application.OpenURL("https://mistybytes.com/terms-of-service/"); };

        privacyPolicy.Init();
        privacyPolicy.onClick += () => { Application.OpenURL("https://mistybytes.com/privacy-policy/"); };

        dysonSwarmStoryOpenButton.Init();
        dysonSwarmStoryOpenButton.onClick += () =>
        {
            if (dysonSwarmStory.currentTime <= 0f)
            {
                dysonSwarmStory.finished = false;
                dysonSwarmStory.Show();
            } else
            {
                dysonSwarmStory.StartHiding();
            }
        };

#if UNITY_ANDROID
        rateUs.gameObject.SetActive(true);
        rateUs.Init();
        rateUs.onClick += () => { ReviewPopup.Instance.ShowReviewPopup(true); };
#else
        rateUs.gameObject.SetActive(false);
#endif

#if UNITY_ANDROID
        restorePurchases.gameObject.SetActive(false);
#else
        restorePurchases.gameObject.SetActive(true);
        restorePurchases.Init();
#endif
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

        DeselectAllWindowButtons();
        SelectButtonsWhenNoWindowOpen();
    }

    private void SwitchWindow(UIButtonController button, string name)
    {
#if UNITY_ANDROID
        ReviewPopup.Instance.AdvanceReviewPopupCounter();
#endif
        if (activeWindow != null && name == activeWindow.name)
        {
            DeselectAllWindowButtons();
            activeWindow.Dectivate();
            activeWindow = null;
            SelectButtonsWhenNoWindowOpen();
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

    private void SelectButtonsWhenNoWindowOpen()
    {
        foreach (var b in windowButtons)
        {
            if (b.selectWhenNoWindowOpen)
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
        offlineText.text = $"You were offline for <color=#0bf>{NumberFormatter.FormatSecondsToReadable(seconds)}</color>!";
        SetOfflineMoney(earnedMoney);
    }
                
    public void InitAchievementsWindow(int achievementsInRow)
    {
        achievementSquares = new List<AchievementSquare>();
        SetAchievementSquaresWidth(achievementsInRow);
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

    private void SetAchievementSquaresWidth(int achievementsInRow)
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
        offlineMoney.text = $"You made <color=#da0>{NumberFormatter.Format(earnedMoney)}</color>$ when away!";
    }

    public void SetMoneyDisplay(double value)
    {
        moneyCurrencyDisplay.text = NumberFormatter.Format(value);
    }

    public void SetPremiumCurrencyDisplay(double value)
    {
        premiumCurrencyDisplay.text = NumberFormatter.Format(value);
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
