using Newtonsoft.Json;
using PokerHand.Common.Dto;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AnotherPlayerProfileView : MonoBehaviour
{
    [SerializeField] private Button _addToFriendsButton;
    [SerializeField] private Text _addToFriendsButtonText;
    [SerializeField] private RawImage _avatarImage;
    [SerializeField] private Image _countryImage;
    [SerializeField] private TextMeshProUGUI _nickname;
    [SerializeField] private Slider _levelSlider;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _chipsText;
    [SerializeField] private TextMeshProUGUI _firstLoginText;
    [Header("Achievements"), SerializeField] private TextMeshProUGUI _handsPlayedCountText;
    [SerializeField] private TextMeshProUGUI _bestHandCombinationText;
    [SerializeField] private TextMeshProUGUI _handsWonCountText;
    [SerializeField] private TextMeshProUGUI _biggestPotCountText;
    [SerializeField] private TextMeshProUGUI _sitNGoTournamentsWonCountText;
    [Header("Profile texts"), SerializeField] private TextMeshProUGUI _handsPlayedText;
    [SerializeField] private TextMeshProUGUI _bestHandText;
    [SerializeField] private TextMeshProUGUI _handsWonText;
    [SerializeField] private TextMeshProUGUI _biggestPotText;
    [SerializeField] private TextMeshProUGUI _sitNGoTournamentsWonText;
    private string personalCode;
    private Guid playerID;

    private void Awake() => _addToFriendsButton.onClick.AddListener(AddToFriend);

    public void Close() => Destroy(transform.parent.gameObject);

    public void UpdateData(PlayerProfileDto profile, Texture avatarTexture)
    {
        personalCode = profile.PersonalCode;
        playerID = profile.Id;
        SetWindowTexts();
        _nickname.text = profile.UserName;
        _chipsText.text = profile.TotalMoney.IntoCluttered();
        _firstLoginText.text = $"{SettingsManager.Instance.GetString("ProfileWindow.PlayerSince")}: {profile.RegistrationDate}";
        _handsPlayedCountText.text = profile.GamesPlayed.ToString();
        _handsWonCountText.text = profile.GamesWon.ToString();
        _sitNGoTournamentsWonCountText.text = profile.SitAndGoWins.ToString();
        _biggestPotCountText.text = profile.BiggestWin.IntoCluttered();
        if (profile.BestHandType != PokerHand.Common.Helpers.HandType.None)
            _bestHandCombinationText.text = SettingsManager.Instance.GetString(profile.BestHandType.ToCombinationString());
        LevelCounter levelCounter = new LevelCounter(profile.Experience);
        _levelSlider.minValue = levelCounter.MinExperienceBorder;
        _levelSlider.maxValue = levelCounter.MaxExperienceBorder;
        _levelSlider.value = levelCounter.Experience;
        _levelText.text = $"{SettingsManager.Instance.GetString("ProfileWindow.Level")} {levelCounter.Level}";
        _avatarImage.texture = avatarTexture;
        _countryImage.sprite = profile.Country.LoadCountryFlagSprite();
    }

    private void AddToFriend()
    {
        SendFriendRequest(PlayerProfileData.Instance.Id, personalCode);
        _addToFriendsButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.RequestToAddingSend");
        _addToFriendsButton.interactable = false;
    }

    public static void SendFriendRequest(Guid senderJson, string personalCode)
    {
        if (string.IsNullOrEmpty(personalCode))
        {
            NotificationController.ShowNotification(NotificationController.NotificationType.RequestToFriendSuccessful);
            Debug.Log($"{DateTime.UtcNow} Try to send friend request to bot. Fake approve");
        }
        else
        {
            var requestSenderIdJson = JsonConvert.SerializeObject(senderJson);
            Hub.SendAsync(ServerMethods.AddFriendByPersonalCode, requestSenderIdJson, personalCode);
            Debug.Log($"{DateTime.UtcNow} SendRequest from {senderJson} to {personalCode}");
        }
    }

    void SetWindowTexts()
    {
        _handsPlayedText.text = SettingsManager.Instance.GetString("ProfileWindow.HandsPlayed");
        _bestHandText.text = SettingsManager.Instance.GetString("ProfileWindow.BestHand");
        _handsWonText.text = SettingsManager.Instance.GetString("ProfileWindow.HandsWon");
        _biggestPotText.text = SettingsManager.Instance.GetString("ProfileWindow.BiggestPot");
        _sitNGoTournamentsWonText.text = SettingsManager.Instance.GetString("ProfileWindow.SitNGoWon");
        if (ReceiveFriendsDataFromServer.friendsList.Any(t => t.Id == playerID))
        {
            _addToFriendsButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.YourFriend");
            _addToFriendsButton.interactable = false;
        }
        else
            _addToFriendsButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.AddToFriendText");
    }
}