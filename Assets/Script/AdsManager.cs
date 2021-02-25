using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
#pragma warning disable 0649
    [SerializeField]
    private string _projectIdAppleAppStore = "3910494";
    [SerializeField]
    private string _projectIdGooglePlayStore = "3910495";
    [SerializeField]
    private string _videoPlacementId = "rewardedVideo";
    [SerializeField]
    private string _bannerPlacementId = "TopBanner";
#pragma warning restore 0649


    public bool testMode = true;
    public static AdsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(_projectIdGooglePlayStore, testMode);
        StartCoroutine(TryShowBannerAdd());
    }

    IEnumerator TryShowBannerAdd()
    {
        while (!Advertisement.IsReady(_bannerPlacementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(_bannerPlacementId);
    }

    private void Update()
    {
    }

    public void DisplayVideoAd()
    {
        Debug.Log("Displaying Video Ads");
        Advertisement.Show(_videoPlacementId);
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogFormat("Ad failed: {0}", message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == _videoPlacementId)
        {
            if (showResult == ShowResult.Finished)
            {
                Debug.Log("Ad Finished");

                GlobalGameContext.ReloadCredits();
            }
            else if (showResult == ShowResult.Skipped)
            {
                Debug.Log("Ad Skipped");
            }
            else
            {
                Debug.Log("Ad Failed");
            }
        }
    }
}
