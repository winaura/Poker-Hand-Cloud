using AppsFlyerSDK;
using EnhancedScrollerDemos.SnappingDemo;
using PokerHand.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class LuckySpinWindowView : MonoBehaviour
{
    [SerializeField] private GameObject _confetti;
    [SerializeField] private SnappingDemo _snappingDemo;
    [SerializeField] private GameObject _slotScroller;
    [SerializeField] private GameObject _spinWindow;
    [SerializeField] private GameObject _rewardWindow;
    [SerializeField] private GameObject _darkBackground;
    [Header("Main menu"), SerializeField] private Button _openSpinWindowButton;
    [SerializeField] private Image _timerButton;
    [SerializeField] private Text _timerText;
    [Header("SpinWindow"), SerializeField] private Button _spinButton;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Animator _rainbowAnimator;
    [SerializeField] private GameObject _rainbowAnimationImages;
    [Header("RewardWindow"), SerializeField] private Text _rewardChips;
    [SerializeField] private Button _closeLuckySpinWindowButton;
    [SerializeField] private Button _collectChipsButtonWindow;
    [SerializeField] private Button _reRollButton;
    [SerializeField] private Text _spinText;
    [SerializeField] private Text _youWonText;
    [SerializeField] private Text _chipsText;
    [SerializeField] private Text _spinFreeText;
    [SerializeField] private Text _noThanksText;
    [SerializeField] private Text _collectText;
    public event Action OnCloseButtonLuckySpinWindowClick;
    public event Action OnOpenSpinWindowButtonClick;
    public event Action OnSpinButtonClick;
    private Vector3 _rainbowAnimationStartPosition;
    private RarityMenuButtons _rarity;
    private State _state = State.Finally;
    private int _rewardChipsValue = 0;
    private bool isCounterActive = false;
    private bool isAllAdsWatched = false;
    private TimeSpan counterTime;
    private FreeSpinCounter spinTime = new FreeSpinCounter();
    private TimeSpan spinDuration = new TimeSpan(6, 0, 0);

    private void Awake()
    {
        _openSpinWindowButton.onClick.AddListener(() => OnOpenSpinWindowButtonClick.Invoke());
        _reRollButton.onClick.AddListener(WatchAd);
        _closeLuckySpinWindowButton.onClick.AddListener(() => OnCloseButtonLuckySpinWindowClick.Invoke());
        _rainbowAnimationStartPosition = new Vector3(_rainbowAnimationImages.transform.position.x, _rainbowAnimationImages.transform.position.y, _rainbowAnimationImages.transform.position.z);
        Client.OnFreeSpinTimeReceived += CheckStartOptions;
    }

    private void Start()
    {
        AppodealController.Instance.OnWatchedRewardedVideo += RollSpinAfterAds;
        SetWindowTexts();
        CheckStartOptions();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
            CheckStartOptions();
    }

    private void OnDestroy()
    {
        AppodealController.Instance.OnWatchedRewardedVideo -= RollSpinAfterAds;
        Client.OnFreeSpinTimeReceived -= CheckStartOptions;
    }

    private void CheckStartOptions()
    {
        isCounterActive = false;
        StopAllCoroutines();
        if (PlayerPrefs.HasKey("lastEntry"))
        {
            spinTime.LastScrollTime = DateTime.ParseExact(PlayerPrefs.GetString("lastEntry"), GameConstants.dateTimeFormat, CultureInfo.InvariantCulture);
            TimeSpan timeGone = TimeManager.GetTime() - spinTime.LastScrollTime;
            if (timeGone.TotalHours < 6)
            {
                counterTime = spinDuration - timeGone;
                spinTime.NumberOfScrollsLeft = PlayerPrefs.GetInt("watchedAds", 0);
                if (spinTime.NumberOfScrollsLeft >= 3) 
                    isAllAdsWatched = true;
                StartCoroutine(Counter());
            }
            else
            {
                spinTime.NumberOfScrollsLeft = 0;
                PlayerPrefs.SetInt("watchedAds", spinTime.NumberOfScrollsLeft);
                isAllAdsWatched = false;
                isCounterActive = false;
            }
        }
    }

    public void OpenSpinWindow()
    {
        if (!isCounterActive)
            FreeRewardWindow();
        else
        {
            AdsRewardWindow();
            if (isAllAdsWatched)
                ShowReRollButton(false);
            else
                ShowReRollButton(true);
        }
    }

    private void FreeRewardWindow()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(AppsFlyerConstants.FreespinRollTime, TimeManager.GetTime().ToString(AppsFlyerConstants.TimePattern));
        AppsFlyer.sendEvent(AppsFlyerConstants.Freespin, eventValues);
        if (!isCounterActive)
        {
            spinTime.LastScrollTime = TimeManager.GetTime();
            PlayerPrefs.SetString("lastEntry", spinTime.LastScrollTime.ToString(GameConstants.dateTimeFormat));
            spinTime.NumberOfScrollsLeft = 0;
            PlayerPrefs.SetInt("watchedAds", spinTime.NumberOfScrollsLeft);
            Client.HTTP_UpdateFreeSpinTimeRequest(spinTime);
            counterTime = spinDuration;
            ShowReRollButton(true);
            StartCoroutine(Counter());
        }
        _darkBackground.SetActive(true);
        _youWonText.gameObject.SetActive(true);
        _chipsText.gameObject.SetActive(true);
        _rewardChips.gameObject.SetActive(true);
        _confetti.SetActive(true);
        _rewardWindow.SetActive(false);
        _spinWindow.SetActive(true);
        PrepareToSpin();
        StartCoroutine(RollSpinWithDelay(0.5f));
    }

    private void AdsRewardWindow()
    {
        _darkBackground.SetActive(true);
        _youWonText.gameObject.SetActive(false);
        _chipsText.gameObject.SetActive(false);
        _rewardChips.gameObject.SetActive(false);
        _confetti.SetActive(false);
        _rewardWindow.SetActive(true);
        _spinWindow.SetActive(false);
    }

    private void ShowReRollButton(bool rerollActive)
    {
        _closeLuckySpinWindowButton.gameObject.SetActive(true);
        if (rerollActive)
        {
            _spinFreeText.text = SettingsManager.Instance.GetString("LuckySpin.SpinFree") + " X" + (3 - spinTime.NumberOfScrollsLeft);
            _noThanksText.text = SettingsManager.Instance.GetString("LuckySpin.NoThanks");
            _reRollButton.gameObject.SetActive(true);
        }
        else
        {

            _noThanksText.text = SettingsManager.Instance.GetString("LuckySpin.Quit");
            _reRollButton.gameObject.SetActive(false);
        }
    }

    private void OpenCollectRewardWindow()
    {
        _rainbowAnimationImages.transform.position = new Vector3(_rainbowAnimationStartPosition.x, _rainbowAnimationStartPosition.y, _rainbowAnimationStartPosition.z);
        _darkBackground.SetActive(true);
        _spinWindow.SetActive(false);
        _rewardWindow.SetActive(true);
    }

    public void CloseWindow()
    {
        _darkBackground.SetActive(false);
        _spinWindow.SetActive(false);
        _rewardWindow.SetActive(false);
    }

    private void RollSpinAfterAds()
    {
        PlayerPrefs.SetInt("watchedAds", ++spinTime.NumberOfScrollsLeft);
        _spinFreeText.text = SettingsManager.Instance.GetString("LuckySpin.SpinFree") + " X" + (3 - spinTime.NumberOfScrollsLeft);
        if (spinTime.NumberOfScrollsLeft >= 3)
        {
            isAllAdsWatched = true;
            ShowReRollButton(false);
        }
        Client.HTTP_UpdateFreeSpinTimeRequest(spinTime);
        FreeRewardWindow();
    }

    public void RollSpin()
    {
        Client.AddChips(1500);
        AudioManager.Instance.PlaySound(Clips.Spin, _audioSource, 0.2f);
        StartCoroutine(SpinActive());
        _state = State.Roll;
        _snappingDemo.PullLeverButton_OnClick();
        StartCoroutine(AnimationDelay(0));
    }

    private IEnumerator AnimationDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _rainbowAnimator.SetTrigger("Start");
    }

    private void PrepareToSpin()
    {
        _state = State.Prepare;
        _rarity = RarityMenuButtons.Lucky;
        _snappingDemo.SetRarity(_rarity);
    }

    private IEnumerator SpinActive()
    {
        yield return new WaitForSeconds(6.5f);
        AudioManager.Instance.PlaySound(Clips.Winning, _audioSource);
        _state = State.Finally;
        _rewardChipsValue = _snappingDemo.GetScore();
        _rewardChips.text = _rewardChipsValue.IntoCluttered();
        Client.AddChips(_rewardChipsValue - 1500);
        OpenCollectRewardWindow();
    }

    private enum State
    {
        Prepare,
        Roll,
        Finally
    }

    public void SetWindowTexts()
    {
        _spinText.text = SettingsManager.Instance.GetString("LuckySpin.Spin");
        _youWonText.text = SettingsManager.Instance.GetString("LuckySpin.YouWon");
        _chipsText.text = SettingsManager.Instance.GetString("LuckySpin.Chips");
        _spinFreeText.text = SettingsManager.Instance.GetString("LuckySpin.SpinFree") + " X" + (3 - spinTime.NumberOfScrollsLeft);
        if (isAllAdsWatched)
            _noThanksText.text = SettingsManager.Instance.GetString("LuckySpin.Quit");
        else
            _noThanksText.text = SettingsManager.Instance.GetString("LuckySpin.NoThanks");
        _collectText.text = SettingsManager.Instance.GetString("LuckySpin.GetChips");
    }

    private void WatchAd()
    {
        if (!isCounterActive)
        {
            FreeRewardWindow();
            return;
        }
        if (!AppodealController.Instance.ShowRewardedVideo())
        {
            NotificationController.ShowNotification(NotificationController.NotificationType.NoAdsAvaible);
            return;
        }
    }

    private IEnumerator RollSpinWithDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RollSpin();
    }

    IEnumerator Counter()
    {
        isCounterActive = true;
        _spinText.gameObject.SetActive(false);
        _timerButton.gameObject.SetActive(true);
        TimeSpan oneSecond = new TimeSpan(0, 0, 1);
        WaitForSeconds oneSecondWait = new WaitForSeconds(1);
        while (counterTime.TotalSeconds > 0)
        {
            yield return oneSecondWait;
            counterTime -= oneSecond;
            _timerText.text = counterTime.ToString(@"hh\:mm\:ss");
        }
        ShowReRollButton(false);
        _spinText.gameObject.SetActive(true);
        _timerButton.gameObject.SetActive(false);
        isCounterActive = false;
        isAllAdsWatched = false;
    }
}