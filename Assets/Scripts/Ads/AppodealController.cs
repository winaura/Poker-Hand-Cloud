using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using System;
using System.Globalization;

public class AppodealController : MonoBehaviour, IRewardedVideoAdListener, IInterstitialAdListener
{
    [SerializeField] private string AndroidKey;
    [SerializeField] private string IOSKey;
    [SerializeField] private int interstitialDelaySeconds;
    [SerializeField] private bool testMode = false;

    private TimeSpan interstitialDelay;
    private const string interstitialKey = "LastInterstitialShowTime";
    private const string timePattern = @"dd.MM.yyyy HH:mm:ss";
    public event Action OnWatchedRewardedVideo;
    private bool wasSound;

    public static AppodealController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            print($"{DateTime.UtcNow} Initializing Appodeal");
            interstitialDelay = new TimeSpan(0, 0, interstitialDelaySeconds);
            Instance = this;
            Appodeal.SetTesting(testMode);
            Appodeal.SetLogLevel(AppodealLogLevel.Verbose);
            Appodeal.Initialize(GetKey(), AppodealAdType.Interstitial | AppodealAdType.RewardedVideo);
            Appodeal.Cache(AppodealAdType.Interstitial | AppodealAdType.RewardedVideo);
            Appodeal.SetRewardedVideoCallbacks(this);
            Appodeal.SetInterstitialCallbacks(this);
        }
        else
            Destroy(gameObject);
    }

    public bool ShowRewardedVideo()
    {
        if (Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
        {
            Appodeal.Show(AppodealAdType.RewardedVideo);
            return true;
        }
        else
            return false;
    }

    public bool ShowInterstitial()
    {
        DateTime lastInterstitialShowTime = DateTime.ParseExact(
            PlayerPrefs.GetString(interstitialKey, DateTime.MinValue.ToString(timePattern)),
            timePattern,
            CultureInfo.InvariantCulture
            );
        if ((DateTime.UtcNow - lastInterstitialShowTime) < interstitialDelay)
        {
            Debug.Log($"{DateTime.UtcNow} Try to show interstitial but the delay has not yet passed");
            return false;
        }
        if (Appodeal.IsLoaded(AppodealAdType.Interstitial))
        {
            Appodeal.Show(AppodealAdType.Interstitial);
            PlayerPrefs.SetString(interstitialKey, DateTime.UtcNow.ToString(timePattern));
            return true;
        }
        else
            return false;
    }

    private string GetKey()
    {
        switch(Application.platform)
        {
            case RuntimePlatform.Android:
                if (string.IsNullOrEmpty(AndroidKey))
                    Debug.LogError($"{DateTime.UtcNow} Android key is empty");
                return AndroidKey;
            case RuntimePlatform.IPhonePlayer:
                if (string.IsNullOrEmpty(IOSKey))
                    Debug.LogError($"{DateTime.UtcNow} IOS key is empty");
                return IOSKey;
            default: 
                return AndroidKey;
        }
    }

    public void OnRewardedVideoLoaded(bool precache)
    {
        print($"{DateTime.UtcNow} Rewarded Video Loaded");
    }

    public void OnRewardedVideoFailedToLoad()
    {
        Debug.LogError($"{DateTime.UtcNow} Rewarded Video Failed To Load");
    }

    public void OnRewardedVideoShowFailed()
    {
        Debug.LogError($"{DateTime.UtcNow} Rewarded Video Show Failed");
    }

    public void OnRewardedVideoShown()
    {
        wasSound = PlayerData.Instance.isSoundOn;
        PlayerData.Instance.isSoundOn = false;
        print($"{DateTime.UtcNow} Rewarded Video Is Shown");
    }

    public void OnRewardedVideoFinished(double amount, string name)
    {
        print($"{DateTime.UtcNow} Rewarded Video Is Finished");
    }

    public void OnRewardedVideoClosed(bool finished)
    {
        PlayerData.Instance.isSoundOn = wasSound;
        print($"{DateTime.UtcNow} Rewarded Video Closed");
        if (finished)
            ExecuteOnMainThread.AddAction(() => OnWatchedRewardedVideo?.Invoke());
    }

    public void OnRewardedVideoExpired()
    {
        Debug.LogError($"{DateTime.UtcNow} Rewarded Video Is Expired");
    }

    public void OnRewardedVideoClicked()
    {
        print($"{DateTime.UtcNow} Rewarded Video Is Clicked");
    }

    public void OnInterstitialLoaded(bool isPrecache)
    {
        print($"{DateTime.UtcNow} Interstitial Is Loaded");
    }

    public void OnInterstitialFailedToLoad()
    {
        Debug.LogError($"{DateTime.UtcNow} Interstitial Failed To Load");
    }

    public void OnInterstitialShowFailed()
    {
        Debug.LogError($"{DateTime.UtcNow} Interstitial Show Is Failed");
    }

    public void OnInterstitialShown()
    {
        wasSound = PlayerData.Instance.isSoundOn;
        PlayerData.Instance.isSoundOn = false;
        Debug.LogError($"{DateTime.UtcNow} Interstitial Is Shown");
    }

    public void OnInterstitialClosed()
    {
        PlayerData.Instance.isSoundOn = wasSound;
        print($"{DateTime.UtcNow} Interstitial Is Closed");
    }

    public void OnInterstitialClicked()
    {
        print($"{DateTime.UtcNow} Interstitial Is Closed");
    }

    public void OnInterstitialExpired()
    {
        Debug.LogError($"{DateTime.UtcNow} Interstitial Is Expired");
    }
}
