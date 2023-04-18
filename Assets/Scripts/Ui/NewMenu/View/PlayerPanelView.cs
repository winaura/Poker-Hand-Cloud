using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerPanelView : MonoBehaviour
{
    [SerializeField] private GameObject _unreadMessagesIcon;
    [SerializeField] private Text _playerLevelText;
    [SerializeField] private Text _shopText;
    [SerializeField] private Text _playerMoneyText;
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Text _bigCoinText;
    [SerializeField] private Button _ratingsButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button[] _chipsShopButtons;
    [SerializeField] private Button _bigcoinsShopButton;
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _dailyTasksButton;
    [SerializeField] private Button _ratingButton;
    [SerializeField] private Animator _dailyTasksDoneAnimation;
    [SerializeField] private RawImage _avatarImage;
    [SerializeField] private Slider _levelSlider;
    private Image _dailyTasksButtonImage;
    private PlayerData _playerData;
    private PlayerProfileData _playerProfileData;
    public event Action OnRatingButtonClick;
    public event Action OnFriendsButtonClick;
    public event Action OnSettingButtonClick;
    public event OnShopButtonClickEventHandler OnShopButtonClick;
    public delegate void OnShopButtonClickEventHandler(int page);
    public event Action OnInfoButtonClick;
    public event Action OnDailyTasksButtonClick;

    private void OnDestroy()
    {
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView -= ChangeNotification;
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView -= UpdateNotificationCount;
        FriendsChatView.OnMessageRead -= UpdateNotificationCount;
        DailyEventsWindowController.OnDailyEventUpdate -= UpdateDailyEventIcon;
    }

    private void Awake()
    {
        _dailyTasksButtonImage = _dailyTasksButton.GetComponent<Image>();
        _ratingsButton.onClick.AddListener(() => OnRatingButtonClick?.Invoke());
        _settingsButton.onClick.AddListener(() => OnSettingButtonClick?.Invoke());
        foreach(var shopButton in _chipsShopButtons)
            shopButton.onClick.AddListener(() => OnShopButtonClick?.Invoke(0));
        _bigcoinsShopButton.onClick.AddListener(() => OnShopButtonClick?.Invoke(1));
        _infoButton.onClick.AddListener(() => OnInfoButtonClick?.Invoke());
        _dailyTasksButton.onClick.AddListener(() => OnDailyTasksButtonClick?.Invoke());
        _ratingButton.onClick.AddListener(() => Instantiate(Resources.Load<RatingWindow>("RatingWindow")));
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView += ChangeNotification;
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView += UpdateNotificationCount;
        FriendsChatView.OnMessageRead += UpdateNotificationCount;
        DailyEventsWindowController.OnDailyEventUpdate += UpdateDailyEventIcon;
    }

    private void Start()
    {
        MP_SetPicture();
        UpdateNotificationCount();
        UpdateDailyEventIcon();
    }

    private void UpdateDailyEventIcon()
    {
        if (TasksTracker.Instance.tasks == null || TasksTracker.Instance.tasks.Any(t => t == null))
            return;
        _dailyTasksButton.interactable = true;
        if (TasksTracker.Instance.tasks.Any(t => t.IsDone == true && t.IsCollectedReward == false))
            _dailyTasksButtonImage.color = Color.red;
        else
            _dailyTasksButtonImage.color = Color.white;
    }

    public void SetLevelText(string text) => _playerLevelText.text = text;

    public void SetMoneyText(string text) => _playerMoneyText.text = text;

    public void SetNameText(string text) => _playerNameText.text = text;

    public void SetBigCoinText(string text) => _bigCoinText.text = text;

    public void MP_SetPicture()
    {
        if (Client.ProfileImages != null)
            _avatarImage.texture = Client.ProfileImages[0];
    }


    private void ChangeNotification()
    {
        if (StaticRuntimeSets.Items.ContainsKey("MainFriendsWindow(Clone)") && StaticRuntimeSets.Items["MainFriendsWindow(Clone)"].activeSelf) 
            return;
        UpdateNotificationCount();
    }

    private void UpdateNotificationCount()
    {
        if (ReceiveFriendsDataFromServer.allUnreadMessagesDictionary.Count != 0 || ReceiveFriendsDataFromServer.friendRequestsList.Count != 0)
        {
            _unreadMessagesIcon.SetActive(true);
            _unreadMessagesIcon.transform.GetChild(0).GetComponent<Text>().text = "<b>" +
                (ReceiveFriendsDataFromServer.allUnreadMessagesDictionary.Count + ReceiveFriendsDataFromServer.friendRequestsList.Count).ToString() + 
                "</b>";
        }
        else
            _unreadMessagesIcon.SetActive(false);
    }

    private void UpdateDailyEventButtonState()
    {
        if (TasksTracker.Instance.IsAnyDone())
        {
            _dailyTasksDoneAnimation.gameObject.SetActive(true);
            _dailyTasksDoneAnimation.SetTrigger("EventIsDone");
        }
        else
        {
            _dailyTasksDoneAnimation.gameObject.SetActive(false);
            _dailyTasksDoneAnimation.StopPlayback();
        }
    }

    public void MP_UpdateLevelData()
    {
        _playerProfileData = PlayerProfileData.Instance;
        LevelCounter levelCounter = new LevelCounter(PlayerProfileData.Instance.Experience);
        _levelSlider.minValue = levelCounter.MinExperienceBorder;
        _levelSlider.maxValue = levelCounter.MaxExperienceBorder;
        _levelSlider.value = levelCounter.Experience;
        _playerLevelText.text = $"{SettingsManager.Instance.GetString("PlayerPanel.Level")} {levelCounter.Level}";
        _shopText.text = SettingsManager.Instance.GetString("PlayerPanel.Shop");
    }
}