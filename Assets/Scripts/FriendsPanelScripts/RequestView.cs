using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PokerHand.Common.Helpers.Friends;
using System.Linq;

public class RequestView : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Text experience;
    [SerializeField] private Text totalSum;
    [SerializeField] private RawImage userImage;
    [SerializeField] private GameObject _onlineIndictaor;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _declineButton;
    [SerializeField] private Text _accept;
    [SerializeField] private Text _refuse;
    private Guid guid;
    private bool isOnline;

    private void Awake()
    {
        SetTexts();
        _acceptButton.onClick.AddListener(AcceptFriendInvite);
        _declineButton.onClick.AddListener(DeclineFriendInvite);
        ReceiveFriendsDataFromServer.OnReceiveOnlineStatus += ChangeOnlineStatus;
    }

    private void SetTexts()
    {
        _accept.text = SettingsManager.Instance.GetString("FriendsPanel.Accept");
        _refuse.text = SettingsManager.Instance.GetString("FriendsPanel.Refuse");
    }

    private void OnDestroy() => ReceiveFriendsDataFromServer.OnReceiveOnlineStatus -= ChangeOnlineStatus;

    public void SetRequestData(string userName, int experience, long totalSum, Texture userImage, bool isOnline, Guid guid)
    {
        LevelCounter level = new LevelCounter(experience);
        this.userName.text = userName;
        this.experience.text = $"{SettingsManager.Instance.GetString("PlayerPanel.Level")} { new LevelCounter(experience).Level}";
        this.totalSum.text = totalSum.IntoCluttered();
        this.userImage.texture = userImage;
        this.guid = guid;
        this.isOnline = isOnline;
        _onlineIndictaor.SetActive(isOnline);
    }

    private void ChangeOnlineStatus(List<PlayerOnlineStatusDto> list)
    {
        if (!list.Any(t => t.Id == guid))
            return;
        bool isOnlineNow = list.First(t => t.Id == guid).IsOnline;
        if (isOnlineNow != isOnline)
        {
            _onlineIndictaor.SetActive(isOnlineNow);
            var friendRequest = ReceiveFriendsDataFromServer.friendRequestsList.First(t => t.Id == guid);
            friendRequest.IsOnline = isOnlineNow;
            isOnline = isOnlineNow;
        }
    }

    private void AcceptFriendInvite()
    {
        AcceptFriendRequest(PlayerProfileData.Instance.Id, guid);
        Destroy(gameObject);
    }

    private void DeclineFriendInvite()
    {
        DeclineFriendRequest(PlayerProfileData.Instance.Id, guid);
        Destroy(gameObject);
    }

    private void AcceptFriendRequest(Guid receiver, Guid sender)
    {
        FriendRequestModel friendRequest = new FriendRequestModel()
        {
            requestReceiverId = receiver,
            requestSenderId = sender
        };
        ReceiveFriendsDataFromServer.AcceptRequest(guid);
        ReceiveFriendsDataFromServer.AddChatToDictionary(guid);
        Hub.SendAsync(ServerMethods.AcceptFriendRequest, JsonConvert.SerializeObject(friendRequest));
        print($"{DateTime.UtcNow} Accepted friend reques {sender}");
    }

    private void DeclineFriendRequest(Guid receiver, Guid sender)
    {
        FriendRequestModel friendRequest = new FriendRequestModel()
        {
            requestReceiverId = receiver,
            requestSenderId = sender
        };
        ReceiveFriendsDataFromServer.DeleteFriendFromRequestList(guid);
        Hub.SendAsync(ServerMethods.DeclineFriendRequest, JsonConvert.SerializeObject(friendRequest));
        print($"{DateTime.UtcNow} Declined friend reques {sender}");
    }
}
