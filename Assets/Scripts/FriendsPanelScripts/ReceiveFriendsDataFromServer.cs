using System.Collections.Generic;
using BestHTTP;
using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Text;
using PokerHand.Common.Dto;

public static class ReceiveFriendsDataFromServer
{
    public static string PersonalCode;
    public static List<FriendDto> friendsList = new List<FriendDto>();
    public static List<Texture2D> friendsImagesList = new List<Texture2D>();
    public static List<Guid> allGuids = new List<Guid>();
    public static List<FriendDto> friendRequestsList = new List<FriendDto>();
    public static List<Texture2D> friendRequestsImagesList = new List<Texture2D>();
    public static Dictionary<Guid, List<PrivateMessageDto>> allChatsDictionary = new Dictionary<Guid, List<PrivateMessageDto>>();
    public static Dictionary<Guid, int> allUnreadMessagesDictionary = new Dictionary<Guid, int>();
    public static Action<List<PlayerOnlineStatusDto>> OnReceiveOnlineStatus;
    public static Action OnUpdateFriendsPanelView;
    public static bool _isConnectToTableById = false;
    public static Guid tableId;

    public static void SetAllFriendsData(FriendsWindowDto friendsWindowDto)
    {
        CleanAllLists();
        if (friendsWindowDto.PlayerPersonalCode != null)
            PersonalCode = friendsWindowDto.PlayerPersonalCode;
        ParseServerData(friendsList, friendsWindowDto.Friends, friendsImagesList);
        ParseServerData(friendRequestsList, friendsWindowDto.FriendRequests, friendRequestsImagesList);
        SetGuidList();
        Debug.Log($"{DateTime.UtcNow} Receive Friend Data Complete");
    }

    public static void ShowNewMessagesNotification(List<MessageNotificationDto> list)
    {
        for (int i = 0; i < list.Count; i++)
            allUnreadMessagesDictionary.Add(list[i].SenderId, list[i].NumberOfNewMessages);
        OnUpdateFriendsPanelView?.Invoke();
    }

    public static void AddChatToDictionary(Guid guid)
    {
        List<PrivateMessageDto> privateMessageDtos = new List<PrivateMessageDto>();
        allChatsDictionary.Add(guid, privateMessageDtos);
    }

    public static void AddFriendToRequestList(List<FriendDto> friendDtos)
    {
        friendRequestsList.Clear();
        friendRequestsImagesList.Clear();
        friendRequestsList.AddRange(friendDtos);
        for (int i = 0; i < friendDtos.Count; i++)
            ParseStringToTexture(friendDtos[i].ProfileImage, friendRequestsImagesList);
        OnUpdateFriendsPanelView?.Invoke();
    }

    public static void SetAllChatsList(List<ConversationDto> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Guid anotherGuidKey;
            if (list[i].FirstPlayerId.Equals(PlayerProfileData.Instance.Id))
                anotherGuidKey = list[i].SecondPlayerId;
            else
                anotherGuidKey = list[i].FirstPlayerId;
            if (!allChatsDictionary.ContainsKey(anotherGuidKey))
                allChatsDictionary.Add(anotherGuidKey, list[i].Messages);
        }
    }

    public static void AddMeToAnotherPlayer(FriendDto friendDto)
    {
        friendsList.Add(friendDto);
        ParseStringToTexture(friendDto.ProfileImage, friendsImagesList);
        OnUpdateFriendsPanelView?.Invoke();
    }

    public static void DeleteFriendFromFriendsList(Guid guid)
    {
        for (int i = 0; i < friendsList.Count; i++)
            if (friendsList[i].Id.Equals(guid))
            {
                friendsList.Remove(friendsList[i]);
                friendsImagesList.RemoveAt(i);
            }
        allChatsDictionary.Remove(guid);
        allUnreadMessagesDictionary.Remove(guid);
        OnUpdateFriendsPanelView?.Invoke();
    }

    public static void AcceptRequest(Guid guid)
    {
        for (int i = 0; i < friendRequestsList.Count; i++)
            if (friendRequestsList[i].Id.Equals(guid))
            {
                friendsList.Add(friendRequestsList[i]);
                friendsImagesList.Add(friendRequestsImagesList[i]);
                friendRequestsList.Remove(friendRequestsList[i]);
                friendRequestsImagesList.RemoveAt(i);
            }
        OnUpdateFriendsPanelView?.Invoke();
    }

    public static void DeleteFriendFromRequestList(Guid guid)
    {
        for (int i = 0; i < friendRequestsList.Count; i++)
            if (friendRequestsList[i].Id.Equals(guid))
            {
                friendRequestsList.Remove(friendRequestsList[i]);
                friendRequestsImagesList.RemoveAt(i);
            }
        OnUpdateFriendsPanelView?.Invoke();
    }

    public static void CheckOnline()
    {
        var json = JsonConvert.SerializeObject(allGuids);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Friends/checkStatus"), methodType: HTTPMethods.Post, callback: OnRequestFinished);
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(json);
        request.Send();
        void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
            {
                var respJson = JsonConvert.DeserializeObject<FriendsOnlineStatus>(resp.DataAsText);
                OnReceiveOnlineStatus?.Invoke(respJson.Value);
            }
            else 
                Debug.LogWarning($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public static void ParseServerData(List<FriendDto> list, List<FriendDto> dataList, List<Texture2D> textureList)
    {
        list.AddRange(dataList);
        for (int i = 0; i < dataList.Count; i++)
            ParseStringToTexture(dataList[i].ProfileImage, textureList);
    }

    private static void SetGuidList()
    {
        for (int i = 0; i < friendsList.Count; i++)
            allGuids.Add(friendsList[i].Id);
        for (int i = 0; i < friendRequestsList.Count; i++)
            allGuids.Add(friendRequestsList[i].Id);
    }

    private static void ParseStringToTexture(string stringToParse, List<Texture2D> textureList)
    {
        if (stringToParse == null)
        {
            textureList.Add(Resources.Load<Texture2D>("Images/Avatar_standart"));
            return;
        }
        var texture = new Texture2D(0, 0);
        var bytes = Convert.FromBase64String(stringToParse);
        texture.LoadImage(bytes);
        textureList.Add(texture);
    }

    public static Texture2D ParseStringToTexture(string stringToParse)
    {
        if (string.IsNullOrEmpty(stringToParse))
            return Resources.Load<Texture2D>("Images/Avatar_standart");
        var texture = new Texture2D(0, 0);
        var bytes = Convert.FromBase64String(stringToParse);
        texture.LoadImage(bytes);
        return texture;
    }

    private static void CleanAllLists()
    {
        friendsList.Clear();
        friendsImagesList.Clear();
        friendRequestsList.Clear();
        friendRequestsImagesList.Clear();
    }
}

[Serializable]
public class FriendDto
{
    public Guid Id { get; set; }
    public bool IsOnline { get; set; }
    public string UserName { get; set; }
    public string ProfileImage { get; set; }
    public int Experience { get; set; }
    public long TotalMoney { get; set; }
}

[Serializable]
public class Friend
{
    public string Message { get; set; }
    public FriendDto Value { get; set; }
}

[Serializable]
public class FriendsWindowDto
{
    public string PlayerPersonalCode { get; set; }
    public List<FriendDto> Friends { get; set; }
    public List<FriendDto> FriendRequests { get; set; }
}

[Serializable]
public class FriendsMessageDto
{
    public string Message { get; set; }
    public FriendsWindowDto Value { get; set; }
}

[Serializable]
public class FriendsOnlineStatus
{
    public string Message { get; set; }
    public List<PlayerOnlineStatusDto> Value { get; set; }
}

[Serializable]
public class PlayerOnlineStatusDto
{
    public Guid Id { get; set; }
    public bool IsOnline { get; set; }
}

// Для получения объекта одного сообщения 
[Serializable]
public class PrivateMessageDto
{
    public Guid SenderId { get; set; }
    public string Text { get; set; }
    public DateTime SendTime { get; set; }
    public Guid Id { get; set; }
}

[Serializable]
public class ConversationDto
{
    public Guid FirstPlayerId { get; set; }
    public Guid SecondPlayerId { get; set; }
    public List<PrivateMessageDto> Messages { get; set; }
}

// Для расшифровки всех чатов
[Serializable]
public class AllChats
{
    public string Message { get; set; }
    public List<ConversationDto> Value { get; set; }
}

[Serializable]
public class AuthenticationResultDto
{
    public PlayerProfileDto PlayerProfileDto { get; set; }
    public FriendsWindowDto FriendsWindowDto { get; set; }
    public List<ConversationDto> ConversationDtos { get; set; }
    public List<MessageNotificationDto> NotificationDtos { get; set; }
    public int MoneyBoxAmount { get; set; }
}

// ### Сумма непрочитанных сообщений 
[Serializable]
public class MessageNotificationDto
{
    public Guid PlayerId { get; set; }
    public Guid SenderId { get; set; }
    public int NumberOfNewMessages { get; set; }
}

[Serializable]
public class GetAuthenticationResultDto
{
    public string Message { get; set; }
    public AuthenticationResultDto Value { get; set; }
}

[Serializable]
public class GetInvitationDto
{
    public string PlayerName { get; set; }
    public Guid TableId { get; set; }
    public TablesInWorlds TableTitle { get; set; }
}

[Serializable]
public class JoinTableDto
{
    public Guid TableId { get; set; }
    public TablesInWorlds TableTitle { get; set; }
}



public enum AnswerType
{
    acceptRequest,
    declineRequest
}


