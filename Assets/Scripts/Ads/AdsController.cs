//using System;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Advertisements;

//public class AdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
//{
//    [SerializeField] private string androidGameId;
//    [SerializeField] private string iosGameId;
//    [SerializeField] private string androidPlacementID;
//    [SerializeField] private string iosPlacementID;
//    [SerializeField] private bool testMode = true;
//    [SerializeField] private bool enablePerPlacementMode = true;
//    private string gameID;
//    private string placementID;
//    private Coroutine loadAdRoutine;
//    public static AdsController Instance { get; private set; }
//    public event Action OnWatchedRewardedVideo;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            InitializeAds();
//        }
//        else
//            Destroy(gameObject);
//    }

//    public bool ShowRewardedVideo()
//    {
//        if (Advertisement.IsReady(placementID))
//        {
//            Advertisement.Show(placementID, this);
//            return true;
//        }
//        else
//        {
//            if (loadAdRoutine != null)
//                StopCoroutine(loadAdRoutine);
//            Advertisement.Load(placementID, this);
//            return false;
//        }
//    }

//    public void InitializeAds()
//    {
//        switch(Application.platform)
//        {
//            case RuntimePlatform.Android:
//                gameID = androidGameId;
//                placementID = androidPlacementID;
//                break;
//            case RuntimePlatform.IPhonePlayer:
//                gameID = iosGameId;
//                placementID = iosPlacementID;
//                break;
//            case RuntimePlatform.WindowsEditor:
//                gameID = androidGameId;
//                placementID = androidPlacementID;
//                break;
//        }
//        Advertisement.Initialize(gameID, testMode, enablePerPlacementMode, this);
//    }

//    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
//    {
//        switch(showCompletionState)
//        {
//            case UnityAdsShowCompletionState.COMPLETED:
//            case UnityAdsShowCompletionState.SKIPPED:
//                ExecuteOnMainThread.RunOnMainThread.Enqueue(() => {
//                    OnWatchedRewardedVideo?.Invoke();
//                });
//                Advertisement.Load(placementID, this);
//                break;
//        }
//    }

//    public void OnInitializationComplete()
//    {
//        Debug.Log("Unity Ads initialization complete.");
//        Advertisement.Load(placementID, this);
//    }

//    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
//    {
//        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
//        StartCoroutine(ExecuteActionWithDelay(() =>
//        {
//            Advertisement.Initialize(gameID, testMode, enablePerPlacementMode, this);
//        }, 5, "Unity Ads try to initalize again"));
//    }

//    public void OnUnityAdsAdLoaded(string placementId) => Debug.Log("Unity Ads loaded");

//    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
//    {
//        Debug.LogError($"Unity Ads loading Failed: {error} - {message}");
//        loadAdRoutine = StartCoroutine(ExecuteActionWithDelay(() =>
//        {
//            Advertisement.Load(placementID, this);
//        }, 180, "Unity Ads try to load ad again"));
//    }

//    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
//    {
//        Debug.LogError($"Unity Ads Show Failed: {error} - {message}");
//        StartCoroutine(ExecuteActionWithDelay(() =>
//        {
//            Advertisement.Load(placementID, this);
//        }, 0, "Unity Ads try to show ad again"));
//    }

//    public void OnUnityAdsShowStart(string placementId) => Debug.Log("Unity Ads Show");


//    public void OnUnityAdsShowClick(string placementId) { }

//    private IEnumerator ExecuteActionWithDelay(Action action, float delay, string message="")
//    {
//        yield return new WaitForSeconds(delay);
//        if (!string.IsNullOrEmpty(message))
//            Debug.Log(message);
//        action?.Invoke();
//    }
//}