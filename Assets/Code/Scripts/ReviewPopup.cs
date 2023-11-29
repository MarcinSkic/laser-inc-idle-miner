using System.Collections;
using UnityEngine;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
public class ReviewPopup : MonoBehaviour
{
#if UNITY_ANDROID
    public static ReviewPopup Instance;
    [SerializeField] private int maxCountToDisplayReviewPopup = 5;
    [SerializeField] private int currentCountToDisplayReviewPopup = 0;

    void Awake()
    {
        Load();
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private IEnumerator ShowPopUp(bool isClicked)
    {
        ReviewManager ReviewManager = new ReviewManager();
        var requestFlowOperation = ReviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log($"Review Popup request error: {requestFlowOperation.Error.ToString()}");
            if (isClicked)
            {
                 Application.OpenURL("market://details?id=laser.inc.idle.miner");
            }
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }

        var PlayReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = ReviewManager.LaunchReviewFlow(PlayReviewInfo);
        yield return launchFlowOperation;
        PlayReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log($"Review Popup show error: {launchFlowOperation.Error.ToString()}");
            if (isClicked)
            {
                Application.OpenURL("market://details?id=laser.inc.idle.miner");
            }
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
    }

    public void ShowReviewPopup(bool isClicked)
    {
        StartCoroutine(ShowPopUp(isClicked));
    }

    public void AdvanceReviewPopupCounter()
    {
        if (currentCountToDisplayReviewPopup > maxCountToDisplayReviewPopup)
        {
            ShowReviewPopup(false);
            currentCountToDisplayReviewPopup = 0;
        }
        currentCountToDisplayReviewPopup++;
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetInt("count-display-review-popup", currentCountToDisplayReviewPopup);
    }

    private void Load()
    {
        currentCountToDisplayReviewPopup = PlayerPrefs.GetInt("count-display-review-popup");
    }
#endif
}