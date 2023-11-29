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

    public bool subscribedNoAds = false;

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
        rewardObject.adManager = this;
        
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

    [ContextMenu("Test")]
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

    // TODO: From here all the way down code is ABSOLUTELY HORRIBLE, it should connect functions to onRewardCollect action which would be called in reward callbacks or instantly when skip allowed 
    public bool TryToShowRewardedAd(bool argDoubleAfkReward = false, BatRewardType argBatType = BatRewardType.money, int argBatRewardValue = 0)
    {

        if (subscribedNoAds)
        {
            return SkipAd("Reward given, skipping ad - subscribed", argDoubleAfkReward, argBatType, argBatRewardValue);
        }

#if UNITY_EDITOR
        return SkipAd("Reward given, skipping ad - in-editor!", argDoubleAfkReward, argBatType, argBatRewardValue);
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

    private bool SkipAd(string reason, bool argDoubleAfkReward, BatRewardType argBatType, int argBatRewardValue)
    {
        doubleAfkReward = argDoubleAfkReward;
        batType = argBatType;
        batRewardValue = argBatRewardValue;
        OnRewardComplete();
        Debug.Log(reason);
        return true;
    }

    public void TryToShowInterstitial()
    {
#if !UNITY_EDITOR

        if (Interstitial.IsAvailable(InterID) && TotalInterAds < 10 && subscribedNoAds == false) {
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

    public AdManager adManager;

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
        adManager.OnRewardComplete();
    }

    public void OnRequestStart(string placementId, string requestId)
    {
        
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

    public void OnRequestStart(string placementId, string requestId)
    {

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

    public void OnRequestStart(string placementId, string requestId)
    {

    }
}