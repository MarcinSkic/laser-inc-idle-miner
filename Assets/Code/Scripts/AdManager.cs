using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fyber;

#if UNITY_IOS && !UNITY_EDITOR
using Unity.Advertisement.IosSupport;
#endif

public class AdManager : MonoBehaviour
{

    public int TotalInterAds;
    public int TotalRewardAds;

    public ResourcesManager resourcesManager;
    public GameModel gameModel;
    public GameController gameController;

    public bool BannerDisplayed;

    public bool NoAdsActive;

    string AppId = "148809";
    string BannerID = "1230284";
    string RewardID = "1230282";
    string InterID = "1230283";

    System.DateTime lastAdTime;

    MyRerwardCallbacks rewardObject;
    MyInterstitialCallbacks interObject;
    MyBannerCallbacks bannerObject;

    bool doubleAfkReward;
    BatRewardType batType;
    int batRewardValue;

    // Start is called before the first frame update
    void Start()
    {

#if UNITY_IOS
        AppId = "148810";
        BannerID = "1230287";
        RewardID = "1230285";
        InterID = "1230286";
#endif


        InitAdSystem();
#if UNITY_IOS
        CheckiOSToken();
#endif

        DoDailyReset();

        lastAdTime = System.DateTime.Now;

        BannerDisplayed = false;

        doubleAfkReward = false;
        batType = BatRewardType.money;
        batRewardValue = 0;
    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {

//#if !UNITY_EDITOR
/*        if(gameModel.Depth > 40 && MainMenuContainer.activeSelf == true && NoAdsActive == false) {
            Banner.Show(BannerID);
            BannerDisplayed = true;            
        }else{
            Banner.Hide(BannerID);
        }*/
        if(rewardObject != null) {
            if(Rewarded.IsAvailable(RewardID) == false) {
                Rewarded.Request(RewardID);
            }
        }
        if (interObject != null) {
            if (Interstitial.IsAvailable(InterID) == false) {
                Interstitial.Request(InterID);
            }
        }

        if (gameModel.Depth > 40 && Rewarded.IsAvailable(RewardID) && TotalRewardAds < 20) {
            System.TimeSpan tAd = System.DateTime.Now - lastAdTime;
            if (tAd.Minutes > 5) {
                //BonusAdButton.SetActive(true);
            } else {
                //BonusAdButton.SetActive(false);
            }
        } else {
            //BonusAdButton.SetActive(false);
        }
//#endif




    }

    public void CheckiOSToken()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if(Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
            Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
            Unity.Advertisement.IosSupport.ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }

    public bool RewardAvailable()
    {
#if UNITY_EDITOR
        return true;
#endif

        if (TotalRewardAds < 20)
        {
            return Rewarded.IsAvailable(RewardID);
        }
        return false;
    }

    public void HideBanner()
    {
#if !UNITY_EDITOR
        Banner.Hide(BannerID);
#endif
    }

    public void InitAdSystem()
    {
#if !UNITY_EDITOR
        FairBid.Start(AppId);

        rewardObject = new MyRerwardCallbacks();
        rewardObject.currentAd = this;
        
        interObject = new MyInterstitialCallbacks();

        bannerObject = new MyBannerCallbacks();

        Rewarded.SetRewardedListener(rewardObject);
        Banner.SetBannerListener(bannerObject);
        Interstitial.SetInterstitialListener(interObject);
#endif

    }


    public void DoDailyReset()
    {
        TotalInterAds = 0;
        TotalRewardAds = 0;

    }

    public void VerifyIntegration()
    {
        FairBid.ShowTestSuite();
    }

    public void TryShowBatAd(BatRewardType argBatType, int argBatRewardValue)
    {
        TryToShowRewardedAd(false, argBatType, argBatRewardValue);
    }

    public void TryShowDoubleOfflineGainAd()
    {
        TryToShowRewardedAd(true);
    }

    public bool TryToShowRewardedAd(bool argDoubleAfkReward = false, BatRewardType argBatType = BatRewardType.money, int argBatRewardValue = 0)
    {
#if UNITY_EDITOR
        doubleAfkReward = argDoubleAfkReward;
        batType = argBatType;
        batRewardValue = argBatRewardValue;
        OnRewardComplete();
        Debug.Log("Reward given, skipping ad - in-editor!");
        return true;
#endif

        if (Rewarded.IsAvailable(RewardID))
        {
            Debug.Log("196");
            doubleAfkReward = argDoubleAfkReward;
            batType = argBatType;
            batRewardValue = argBatRewardValue;
            Rewarded.Show(RewardID);
            lastAdTime = System.DateTime.Now;
            return true;
        }
        Debug.Log("204");
        return false;

    }

    public void TryToShowInterstitial()
    {
#if !UNITY_EDITOR

        if (Interstitial.IsAvailable(InterID) && TotalInterAds < 10 && NoAdsActive == false) {
            System.TimeSpan tAd = System.DateTime.Now - lastAdTime;
            if(tAd.Minutes > 3) {
                Interstitial.Show(InterID);
                lastAdTime = System.DateTime.Now;
                TotalInterAds++;
            }
        }
#endif
    }

    public void OnRewardComplete()
    {
        gameController.HandleDoubleOfflineReward();
        if (batRewardValue > 0)
        {
            gameController.AcceptOfflineReward(120);
            switch (batType)
            {
                case BatRewardType.money:
                    //debugString += " seconds worth of offlineReward";
                    resourcesManager.IncreaseMoneyForOfflineByTime(batRewardValue);
                    AudioManager.Instance.Play("caught_coins");
                    break;
                case BatRewardType.powerup:
                    //debugString += " seconds of double laser power";
                    resourcesManager.IncreasePowerUpTimeLeft(batRewardValue);
                    AudioManager.Instance.Play("caught_p_up");
                    break;
                case BatRewardType.premium:
                    //debugString += " premium curency";
                    resourcesManager.IncreasePremiumCurrency(batRewardValue);
                    AudioManager.Instance.Play("caught_crystal");
                    break;
            }
        }

        TotalRewardAds++;
    }
}

public class MyRerwardCallbacks : RewardedListener
{

    public AdManager currentAd;

    public void OnShow(string placementId, ImpressionData impressionData)
    {
        // Called when a rewarded ad from placementId shows up. In case the ad is a video, audio play will start here.
        // On Android, this callback might be called only once the ad is closed.
    }

    public void OnClick(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' is clicked
    }

    public void OnHide(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' hides. 
    }

    public void OnShowFailure(string placementId, ImpressionData impressionData)
    {
        // Called when an error arises when showing a rewarded ad from placement 'placementId'
    }

    public void OnAvailable(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' becomes available
    }

    public void OnUnavailable(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' becomes unavailable
    }

    public void OnCompletion(string placementId, bool userRewarded)
    {
        // Called when a rewarded ad from placement 'placementId' finishes playing. In case the ad is a video, audio play will stop here.
        currentAd.OnRewardComplete();
    }

    public void OnRequestStart(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' is going to be requested
    }
}

public class MyInterstitialCallbacks : InterstitialListener
{
    public void OnShow(string placementId, ImpressionData impressionData)
    {
        // Called when an Interstitial from placement 'placementId' shows up. In case the ad is a video, audio play will start here.
        // On Android, this callback might be called only once the ad is closed.
    }

    public void OnClick(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' is clicked
    }

    public void OnHide(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' hides.
    }

    public void OnShowFailure(string placementId, ImpressionData impressionData)
    {
        // Called when an error arises when showing an Interstitial from placement 'placementId'
    }

    public void OnAvailable(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' becomes available
    }

    public void OnUnavailable(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' becomes unavailable
    }

    public void OnRequestStart(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' is going to be requested
    }
}

public class MyBannerCallbacks : BannerListener
{
    public void OnError(string placementId, string error)
    {
        // Called when an error from placement 'placementId' arises when loading an ad
    }

    public void OnLoad(string placementId)
    {
        // Called when an ad from placement 'placementId' is loaded
    }

    public void OnShow(string placementId, ImpressionData impressionData)
    {
        // Called when banner from placement 'placementId' shows up
    }
    public void OnClick(string placementId)
    {
        // Called when banner from placement 'placementId' is clicked
    }

    public void OnRequestStart(string placementId)
    {
        // Called when a banner from placement 'placementId' is going to be requested
    }
}