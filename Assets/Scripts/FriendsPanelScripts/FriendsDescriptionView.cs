using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class FriendsDescriptionView : MonoBehaviour
{
    [SerializeField] private GameObject _chat;
    [SerializeField] private Button _chatButton;
    [SerializeField] private Button _tableInviteButton;
    [SerializeField] private Button _connectToTableButton;
    [SerializeField] private Text userName;
    [SerializeField] private Text experience;
    [SerializeField] private Text totalSum;
    [SerializeField] private GameObject _unreadMessagesIcon;
    [SerializeField] private Text _inviteButtonText;
    [SerializeField] private Text _connectToTableText;
    [SerializeField] private RawImage userImage;
    [SerializeField] private GameObject _onlineIndictaor;
    [SerializeField] private NotificationWindow notificationWindowPref;
    [SerializeField] private Button avatarButton;
    private Guid guid;
    private bool isOnline;
    private bool isButtonClicked;

    private void Awake()
    {
        SetTexts();
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView += ChangeMessageCount;
        ReceiveFriendsDataFromServer.OnReceiveOnlineStatus += ChangeOnlineStatus;
        _chatButton.onClick.AddListener(OpenChatWindow);
        _tableInviteButton.onClick.AddListener(() => InviteFriendToTable(PlayerProfileData.Instance.Id, guid, Client.TableData.Id));
        _connectToTableButton.onClick.AddListener(() => JoinToTable(guid));
        avatarButton.onClick.AddListener(() =>
        {
            AnotherPlayerProfileController.Instance.SetPickedTexture(userImage.texture);
            Client.HTTP_SendGetPlayerProfile(guid);
        });
        Client.OnSuccessOnInviteFriendToTable += SuccessButtonText;
    }

    private void SetTexts()
    {
        _inviteButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.InviteToTable");
        _connectToTableText.text = SettingsManager.Instance.GetString("FriendsPanel.ConnectToTable");
    }

    private void OnDestroy()
    {
        ReceiveFriendsDataFromServer.OnReceiveOnlineStatus -= ChangeOnlineStatus;
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView -= ChangeMessageCount;
        Client.OnSuccessOnInviteFriendToTable -= SuccessButtonText;
    }

    private void Start() => ChangeMessageCount();

    public void SetFriendData(string userName, int experience, long totalSum, Texture userImage, bool isOnline, Guid guid)
    {
        LevelCounter level = new LevelCounter(experience);
        this.userName.text = userName;
        this.experience.text = $"{SettingsManager.Instance.GetString("PlayerPanel.Level")} {new LevelCounter(experience).Level}";
        this.totalSum.text = totalSum.IntoCluttered();
        this.userImage.texture = userImage;
        this.guid = guid;
        this.isOnline = isOnline;
        ChangeButtonState(isOnline);
        _chatButton.interactable = Client.TableData == null || Client.TableData != null && Client.TableData.Players.All(t => t.Id != guid);
    }

    private void JoinToTable(Guid friendGuid)
    {
        var friendJson = JsonConvert.SerializeObject(friendGuid);
        Hub.SendAsync(ServerMethods.JoinFriendsTable, friendJson);
        print($"{DateTime.UtcNow} JoinToTable");
    }

    private void ChangeOnlineStatus(List<PlayerOnlineStatusDto> list)
    {
        if (!list.Any(t => t.Id == guid))
            return;
        bool isOnlineNow = list.First(t => t.Id == guid).IsOnline;
        if (isOnlineNow != isOnline)
        {
            ChangeButtonState(isOnlineNow);
            var friend = ReceiveFriendsDataFromServer.friendsList.First(t => t.Id == guid);
            friend.IsOnline = isOnlineNow;
            isOnline = isOnlineNow;
        }
    }

    private void ChangeMessageCount()
    {
        var dictionary = ReceiveFriendsDataFromServer.allUnreadMessagesDictionary;
        if (!dictionary.ContainsKey(guid))
            return;
        print($"{DateTime.UtcNow} {guid}");
        _unreadMessagesIcon.transform.GetChild(0).GetComponent<Text>().text = "<b>" + dictionary[guid].ToString() + "</b>";
        _unreadMessagesIcon.SetActive(true);
    }

    private void ChangeButtonState(bool isOnline)
    {
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
            _tableInviteButton.interactable = true;
        _onlineIndictaor.SetActive(isOnline);
        _connectToTableButton.interactable = isOnline;
    }

    private void OpenChatWindow()
    {
        SendMessageReadedRequest(PlayerProfileData.Instance.Id, guid);
        _unreadMessagesIcon.SetActive(false);
        ReceiveFriendsDataFromServer.allUnreadMessagesDictionary.Remove(guid);
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView?.Invoke();
        var prefab = Instantiate(_chat, StaticRuntimeSets.Items["MainFriendsWindow(Clone)"].transform);
        var component = prefab.GetComponent<FriendsChatView>();
        component.recipientId = guid;
    }

    public static void SendMessageReadedRequest(Guid recipientJson, Guid senderJson)
    {
        var recipientIdJson = JsonConvert.SerializeObject(recipientJson);
        var senderIdJson = JsonConvert.SerializeObject(senderJson);
        Hub.SendAsync(ServerMethods.RemoveMessageNotification, recipientIdJson, senderIdJson);
    }

    public void DeleteFriend() => NotificationController.ShowNotification(NotificationController.NotificationType.RemoveFriend, guid.ToString());

    private void InviteFriendToTable(Guid playerJson, Guid friendJson, Guid tableIdJson)
    {
        isButtonClicked = true;
        var playerIdJson = JsonConvert.SerializeObject(playerJson);
        var friendIdJson = JsonConvert.SerializeObject(friendJson);
        var tableId = JsonConvert.SerializeObject(tableIdJson);
        Hub.SendAsync(ServerMethods.InviteFriendToTable, playerIdJson, friendIdJson, tableId);
        print($"{DateTime.UtcNow} Send Invite to Table");
    }

    private void SuccessButtonText()
    {
        if (!isButtonClicked) 
            return;
        _inviteButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.Sent");
        StartCoroutine(ChangeButtonText());
        isButtonClicked = false;
    }

    IEnumerator ChangeButtonText()
    {
        yield return new WaitForSeconds(1);
        _inviteButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.InviteToFriendText");
    }
}
