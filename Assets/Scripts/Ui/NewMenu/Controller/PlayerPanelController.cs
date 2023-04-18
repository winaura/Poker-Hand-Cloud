using System;
using UnityEngine;

public class PlayerPanelController : Singleton<PlayerPanelController>
{
    [SerializeField] private PlayerPanelView _playerPanelView;
    public event Action OnFriendsButtonClick;
    public event Action OnInfoButtonClick;
    public event Action OnRatingButtonClick;
    public event Action OnSettingButtonClick;
    public event OnShopButtonClickEventHandler OnShopButtonClick;
    public delegate void OnShopButtonClickEventHandler(int page);
    public event Action OnDailyTasksButtonClick;
    
    private void OnEnable()
    {
        _playerPanelView.OnFriendsButtonClick += FriendsButtonClick;
        _playerPanelView.OnInfoButtonClick += InfoButtonClick;
        _playerPanelView.OnRatingButtonClick += RatingButtonClick;
        _playerPanelView.OnSettingButtonClick += SettingButtonClick;
        _playerPanelView.OnShopButtonClick += ShopButtonClick;
        _playerPanelView.OnDailyTasksButtonClick += DailyTasksButtonClick;
        SettingsManager.Instance.UpdateTextsEvent += _playerPanelView.MP_UpdateLevelData;
    }

    private void Start()
    {
        PlayerProfileData.Instance.OnExperienceUpdated += MP_UpdateExperience;
        PlayerProfileData.Instance.OnProfileUpdated += MP_UpdateData;
        PlayerProfileData.Instance.OnMyTotalMoneyUpdated += MP_UpdateTotalMoney;
        Client.OnReceiveMyProfileImage += UpdateAvatarImage;
        MP_UpdateData();
    }

    private void OnDestroy()
    {
        PlayerProfileData.Instance.OnExperienceUpdated -= MP_UpdateExperience;
        PlayerProfileData.Instance.OnProfileUpdated -= MP_UpdateData;
        PlayerProfileData.Instance.OnMyTotalMoneyUpdated -= MP_UpdateTotalMoney;
        Client.OnReceiveMyProfileImage -= UpdateAvatarImage;
    }

    private void OnDisable()
    {
        _playerPanelView.OnFriendsButtonClick -= FriendsButtonClick;
        _playerPanelView.OnInfoButtonClick -= InfoButtonClick;
        _playerPanelView.OnRatingButtonClick -= RatingButtonClick;
        _playerPanelView.OnSettingButtonClick -= SettingButtonClick;
        _playerPanelView.OnShopButtonClick -= ShopButtonClick;
        _playerPanelView.OnDailyTasksButtonClick -= DailyTasksButtonClick;
        SettingsManager.Instance.UpdateTextsEvent -= _playerPanelView.MP_UpdateLevelData;
    }

    private void ShopButtonClick(int page) => OnShopButtonClick?.Invoke(page);

    private void SettingButtonClick() => OnSettingButtonClick?.Invoke();

    private void RatingButtonClick() => OnRatingButtonClick?.Invoke();

    private void InfoButtonClick() => OnInfoButtonClick?.Invoke();

    private void FriendsButtonClick() => OnFriendsButtonClick?.Invoke();

    private void DailyTasksButtonClick() => OnDailyTasksButtonClick?.Invoke();
    

    public void UpdateData() => MP_UpdateData();

    public void MP_UpdateData()
    {
        _playerPanelView.SetMoneyText($"${PlayerProfileData.Instance.TotalMoney.IntoDivided()}");
        _playerPanelView.SetLevelText($"Level {new LevelCounter(PlayerProfileData.Instance.Experience).Level}");
        _playerPanelView.SetNameText($"{PlayerProfileData.Instance.Nickname}");
        _playerPanelView.MP_UpdateLevelData();
    }
    
    public void MP_UpdateTotalMoney() => _playerPanelView.SetMoneyText($"${PlayerProfileData.Instance.TotalMoney.IntoDivided()}");

    public void MP_UpdateExperience()
    {
        _playerPanelView.MP_UpdateLevelData();
    }

    void UpdateAvatarImage() => _playerPanelView.MP_SetPicture();
}