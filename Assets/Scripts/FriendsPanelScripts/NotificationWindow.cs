using Newtonsoft.Json;
using PokerHand.Common.Helpers.Friends;
using System;
using UnityEngine;
using UnityEngine.UI;
using static NotificationController;

public class NotificationWindow : MonoBehaviour
{
    [SerializeField] private Text notificationText;
    [SerializeField] private Text messageText;
    [SerializeField] private Text applyButtonText;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button quitButton;

    public void SetNotification(NotificationType notificationType, string message)
    {
        switch (notificationType)
        {
            case NotificationType.RequestToFriendSuccessful:
                notificationText.text = SettingsManager.Instance.GetString("FriendsPanel.Notification");
                messageText.text = SettingsManager.Instance.GetString("FriendsPanel.RequestToAddingSend");
                messageText.fontSize = 70;
                applyButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.Ready");
                break;
            case NotificationType.RemoveFriend:
                notificationText.text = SettingsManager.Instance.GetString("FriendsPanel.Notification");
                messageText.text = SettingsManager.Instance.GetString("FriendsPanel.SureToDeleteFriend");
                messageText.fontSize = 70;
                applyButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.Yes");
                Guid guid = Guid.Parse(message);
                applyButton.onClick.AddListener(() =>
                {
                    var playerIdJson = JsonConvert.SerializeObject(PlayerProfileData.Instance.Id);
                    var friendToRemoveIdJson = JsonConvert.SerializeObject(guid);
                    Hub.SendAsync(ServerMethods.RemoveFriend, playerIdJson, friendToRemoveIdJson);
                    print($"{DateTime.UtcNow} Remove from {PlayerProfileData.Instance.Id} to {guid}");
                    ReceiveFriendsDataFromServer.DeleteFriendFromFriendsList(guid);
                });
                break;
            case NotificationType.ErrorOnJoinFriend:
                notificationText.text = SettingsManager.Instance.GetString("FriendsPanel.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.OK");
                var errorOnJoin = JsonConvert.DeserializeObject<JoinFriendTableErrorTypes>(message);
                switch (errorOnJoin)
                {
                    case JoinFriendTableErrorTypes.FriendIsNotPlaying:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.FriendNotPlaying");
                        messageText.fontSize = 70;
                        break;
                    case JoinFriendTableErrorTypes.TableIsFull:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.NoSits");
                        messageText.fontSize = 70;
                        break;
                    case JoinFriendTableErrorTypes.GameOnSitAndGoAlreadyStarted:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.SitnGoStarted");
                        messageText.fontSize = 70;
                        break;
                }
                break;
            case NotificationType.ErrorOnInviteFriend:
                notificationText.text = SettingsManager.Instance.GetString("FriendsPanel.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.OK");
                var errorOnInvite = JsonConvert.DeserializeObject<InviteFriendToTableErrorTypes>(message);
                switch (errorOnInvite)
                {
                    case InviteFriendToTableErrorTypes.FriendIsOffline:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.FriendNotPlaying");
                        messageText.fontSize = 70;
                        break;
                    case InviteFriendToTableErrorTypes.FriendIsOnTable:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.FriendPlaying");
                        messageText.fontSize = 70;
                        break;
                    case InviteFriendToTableErrorTypes.GameOnSitAndGoAlreadyStarted:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.SitnGoStarted");
                        messageText.fontSize = 70;
                        break;
                }
                break;
            case NotificationType.ErrorOnRequestFriend:
                notificationText.text = SettingsManager.Instance.GetString("FriendsPanel.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.OK");
                var errorOnRequest = JsonConvert.DeserializeObject<FriendRequestErrorTypes>(message);
                switch(errorOnRequest)
                {
                    case FriendRequestErrorTypes.FriendRequestHasBeenSent:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.YouHaveAlreadySent");
                        messageText.fontSize = 70;
                        break;
                    case FriendRequestErrorTypes.PlayerAlreadySentRequest:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.RequestHasAlreadySent");
                        messageText.fontSize = 70;
                        break;
                    case FriendRequestErrorTypes.PlayerIsAlreadyFriend:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.IsYourFriendAlready");
                        messageText.fontSize = 70;
                        break;
                    case FriendRequestErrorTypes.PlayerNotFound:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.WrongCode");
                        messageText.fontSize = 70;
                        break;
                    case FriendRequestErrorTypes.WrongPersonalCode:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.WrongCode");
                        messageText.fontSize = 70;
                        break;
                }
                break;
            case NotificationType.ErrorOnConnectToTable:
                notificationText.text = SettingsManager.Instance.GetString("FriendsPanel.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.OK");
                var errorOnConnect = JsonConvert.DeserializeObject<ConnectToTableByIdErrorTypes>(message);
                switch(errorOnConnect)
                {
                    case ConnectToTableByIdErrorTypes.TableIsFull:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.NoSits");
                        messageText.fontSize = 70;
                        break;
                    case ConnectToTableByIdErrorTypes.TableIsNull:
                        messageText.text = SettingsManager.Instance.GetString("FriendsPanel.FreindHasLeavedFromTable");
                        messageText.fontSize = 70;
                        break;
                }
                break;
            case NotificationType.AutoTopInfo:
                notificationText.text = "AutoTop";
                applyButtonText.text = SettingsManager.Instance.GetString("TableInfo.OK");
                messageText.text = SettingsManager.Instance.GetString("TableInfo.AutoTop");
                messageText.fontSize = 70;
                break;
            case NotificationType.MoneyBoxNothingToOpen:
                notificationText.text = SettingsManager.Instance.GetString("MoneyBox.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("MoneyBox.OK");
                messageText.text = SettingsManager.Instance.GetString("MoneyBox.NothingToOpen");
                messageText.fontSize = 70;
                break;
            case NotificationType.NoAdsAvaible:
                notificationText.text = SettingsManager.Instance.GetString("Ads.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("Ads.OK");
                messageText.text = SettingsManager.Instance.GetString("Ads.NoAdsAvaible");
                messageText.fontSize = 70;
                break;
            case NotificationType.PlayerHasAlreadyAuthorized:
                notificationText.text = SettingsManager.Instance.GetString("Signing.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("Signing.OK");
                messageText.text = SettingsManager.Instance.GetString("Signing.PlayerHasAlreadyAuthorized");
                applyButton.onClick.AddListener(() => Application.Quit());
                quitButton.onClick.AddListener(() => Application.Quit());
                messageText.fontSize = 70;
                break;
            case NotificationType.ServerNotWorking:
                notificationText.text = SettingsManager.Instance.GetString("Signing.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("Signing.OK");
                messageText.text = SettingsManager.Instance.GetString("Signing.ServerNotWorking");
                messageText.fontSize = 70;
                break;
            case NotificationType.ServerVersionDismatch:
                notificationText.text = SettingsManager.Instance.GetString("Signing.Notification");
                applyButtonText.text = SettingsManager.Instance.GetString("Signing.OK");
                messageText.text = SettingsManager.Instance.GetString("Signing.ServerVersionDismatch");
                applyButton.onClick.AddListener(() => Application.OpenURL(GetStoreUrl()));
                applyButton.onClick.AddListener(() => Application.Quit());
                quitButton.onClick.AddListener(() => Application.Quit());
                quitButton.onClick.AddListener(() => Application.OpenURL(GetStoreUrl()));
                messageText.fontSize = 70;
                break;
        }
    }

    public void SetNotification(GameModes gameMode)
    {
        applyButtonText.text = SettingsManager.Instance.GetString("TableInfo.OK");
        switch (gameMode)
        {
            case GameModes.Texas:
                notificationText.text = "Texas poker";
                messageText.text = SettingsManager.Instance.GetString("TableInfo.TexasPoker");
                messageText.fontSize = 70;
                break;
            case GameModes.Royal:
                notificationText.text = "Royal poker";
                messageText.text = SettingsManager.Instance.GetString("TableInfo.RoyalPoker");
                messageText.fontSize = 49;
                break;
            case GameModes.Joker:
                notificationText.text = "Joker poker";
                messageText.text = SettingsManager.Instance.GetString("TableInfo.JokerPoker");
                messageText.fontSize = 50;
                break;
            case GameModes.Dash:
                notificationText.text = "Dash poker";
                messageText.text = SettingsManager.Instance.GetString("TableInfo.DashPoker");
                messageText.fontSize = 70;
                break;
            case GameModes.SitNGo:
                notificationText.text = "SitNGo poker";
                messageText.text = SettingsManager.Instance.GetString("TableInfo.SitnGo");
                messageText.fontSize = 50;
                break;
        }
    }

    public void Close() => Destroy(gameObject);

    public string GetStoreUrl()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "https://play.google.com/store/apps/details?id=com.MyGame.PokerHand";
            case RuntimePlatform.IPhonePlayer:
                return "https://apps.apple.com/us/app/poker-hand-cloud-card-games/id1585304705";
            default:
                return "https://play.google.com/store/apps/details?id=com.MyGame.PokerHand";
        }
    }
}