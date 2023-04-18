using BestHTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokerHand.Common;
using PokerHand.Common.Dto;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;
using PokerHand.Common.Helpers.Present;
using PokerHand.Common.Helpers.QuickChat;
using PokerHand.Common.Helpers.Table;
using PokerHand.Common.ViewModels.Media;
using PokerHand.Common.ViewModels.Profile;
using PokerHand.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.Debug;

public static class Client
{
    public static event Action<TableDto> OnReceiveTableState;
    public static event Action<List<PlayerDto>, List<SidePotDto>> OnReceiveWinningList;
    public static event Action<PlayerProfileDto> OnReceiveMyPlayerProfile;
    public static event Action OnAuthenticated;
    public static event Action<PlayerProfileDto> OnReceiveAnotherPlayerProfile;
    public static event Action<PlayerDto> OnAnotherPlayerConnected;
    public static event Action<Pot> OnReceiveUpdatedPot;
    public static event Action<PresentDto> OnReceivePresent;
    public static event Action<QuickMessageDto> OnReceiveQuickMessage;
    public static event Action<int> OnReceiveCurrentMoneyBoxAmount;
    public static event Action<int> OnPlayerDisconnected;
    public static event Action<int> OnReceiveEndSitAndGoGame;
    public static event Action<int> OnReceiveProfileImage;
    public static event Action<long> OnReceiveMyPlayerTotalMoney;
    public static event Action<PlayerActionType> OnReceivePlayerActionType;
    public static event Action<PrivateMessageDto> OnReceivePrivateMessage;
    public static event Action OnActFoldByInactivePlayer;
    public static event Action OnPurchaseCanselledPayPal;
    public static event Action OnKickPlayerFromTable;
    public static event Action OnPressedFoldDash;
    public static event Action OnReceiveMyProfileImage;
    public static event Action OnReceiveConnectionId;
    public static event Action OnReceivePrepareForGame;
    public static event Action OnReceiveAllTablesInfo;
    public static event Action OnReceivePresentsInfo;
    public static event Action OnReceiveTableInfo;
    public static event Action OnFirstTableDataReceived;
    public static event Action OnReceiveTable;
    public static event Action OnReceiveDealCommunityCards;
    public static event Action OnReceivePlayerAction;
    public static event Action OnReceiveCurrentPlayerId;
    public static event Action OnReceiveWinners;
    public static event Action OnReceiveOnLackOfStackMoney;
    public static event Action OnReceiveOnGameEnd;
    public static event Action OnReceiveShowWinnerCards;
    public static event Action OnReceivePlayerDto;
    public static event Action OnReconected;
    public static event Action OnRewardedTimeReceived;
    public static event Action OnFreeSpinTimeReceived;
    public static event Action OnReceiveContinueRegistration;
    public static event Action OnReceiveMessageOnLackOfMoneyInDash;
    public static event Action<GetInvitationDto> OnReceiveInvitationToTableFromFriend;
    public static event Action<JoinTableDto> OnReceiveFriendsTableInfo;
    public static event Action<RankDto> OnReceiveRank;
    public static event Action<IEnumerable<PlayerWithRankDto>> OnReceivePlayersRank;
    //ErrorBlock
    public static event Action OnSuccessOnAddFriendById;
    public static event Action OnSuccessOnInviteFriendToTable;
    public static event Action OnSuccessOnAddFriendByPersonalCode;
    public static event Action<string> OnErrorOnInviteFriendToTable;
    public static event Action<string> OnErrorOnJoinFriendsTable;
    public static event Action<string> OnErrorOnConnectionToTableById;
    public static event Action<string> OnErrorOnAddFriendById;
    public static event Action<string> OnErrorOnAddFriendByPersonalCode;
    public static event Action<int> OnSuccessOnPayPalPurchase;
    public static Action<FriendDto> OnReceiveFriendRequest;
    public static TableInfoDto TableInfo { get; private set; }
    public static List<TableInfoDto> TablesInfo { get; private set; }
    public static List<PresentInfoDto> PresentsInfo { get; private set; }
    public static PlayerDto MyPlayer { get; private set; }
    public static PlayerDto CurrentPlayer { get; private set; }
    public static PlayerAction LastPlayerAction { get; private set; } = null;
    public static TableDto DealersAndBlinds { get; private set; }
    public static List<PlayerDto> PocketCardsPlayers { get; private set; }
    public static List<PlayerDto> Winners { get; private set; }
    public static List<SidePotDto> WinnersPots { get; private set; }
    public static List<CardDto> CommunityCards { get; private set; }
    public static TableDto TableData { get; private set; }
    public static Texture2D[] ProfileImages { get; private set; } = new Texture2D[8];
    public static bool IsMyPlayerActive { get; private set; }
    public static bool IsMyFirstTurnInGame { get; private set; }
    public static bool IsBlindsReleased { get; private set; } = false;
    static bool isConnectedToTable;
    public static string ConnectionId => Hub.ConnectionID;

    #region Sending
    public static void AddChips(int chips)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} AddChips. Try to send request with empty ID.");
            return;
        }
        AddMoneyVM addMoneyObject = new AddMoneyVM()
        {
            PlayerId = PlayerProfileData.Instance.Id,
            Amount = chips
        };
        var json = JsonConvert.SerializeObject(addMoneyObject);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Profile/addTotalMoney"), methodType: HTTPMethods.Post, callback: OnRequestFinished);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(json);
        request.Send();
        void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
                HTTP_UpdateTotalMoneyAndExperienceRequest();
            else
                Log($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public static void Reconnected() => OnReconected?.Invoke();

    public static void UpdateProfileImage() => OnReceiveMyProfileImage?.Invoke();

    public static void UpdateFriendsListOnAdd(string json)
    {
        Log($"{DateTime.UtcNow} UpdateFriendsListOnAdd");
        var respJson = JsonConvert.DeserializeObject<FriendDto>(json);
        ReceiveFriendsDataFromServer.AddMeToAnotherPlayer(respJson);
        ReceiveFriendsDataFromServer.AddChatToDictionary(respJson.Id);
        Log($"{DateTime.UtcNow} AddChatToDictionary");
    }

    public static void SuccessOnPayPalPurchase(int operationCode) => OnSuccessOnPayPalPurchase?.Invoke(operationCode);

    public static void ReceiveInvitationToTableFromFriend(string json)
    {
        Log($"{DateTime.UtcNow} ReceiveInvitationToTableFromFriend");
        var respJson = JsonConvert.DeserializeObject<GetInvitationDto>(json);
        OnReceiveInvitationToTableFromFriend?.Invoke(respJson);
    }

    public static void ErrorOnInviteFriendToTable(string json) => OnErrorOnInviteFriendToTable?.Invoke(json);

    public static void ErrorOnJoinFriendsTable(string json) => OnErrorOnJoinFriendsTable?.Invoke(json);

    public static void ErrorOnConnectionToTableById(string json) => OnErrorOnConnectionToTableById?.Invoke(json);

    public static void SuccessOnAddFriendById() => OnSuccessOnAddFriendById?.Invoke();

    public static void SuccessOnInviteFriendToTable() => OnSuccessOnInviteFriendToTable?.Invoke();

    public static void ActFoldByInactivePlayer() => OnActFoldByInactivePlayer?.Invoke();

    public static void PurchaseCancelledPayPal() => OnPurchaseCanselledPayPal?.Invoke();

    public static void KickPlayerFromTable()
    {
        OnKickPlayerFromTable?.Invoke();
        Log($"{DateTime.UtcNow} KickPlayerFromTable");
    }

    public static void ReceiveConnectionID() => OnReceiveConnectionId?.Invoke();

    public static void SuccessOnAddFriendByPersonalCode() => OnSuccessOnAddFriendByPersonalCode?.Invoke();
    public static void ErrorOnAddFriendById(string json) => OnErrorOnAddFriendById?.Invoke(json);

    public static void ErrorOnAddFriendByPersonalCode(string json) => OnErrorOnAddFriendByPersonalCode?.Invoke(json);

    public static void ReceiveFriendsTableInfo(string json)
    {
        Log($"{DateTime.UtcNow} ReceiveFriendsTableInfo");
        var respJson = JsonConvert.DeserializeObject<JoinTableDto>(json);
        OnReceiveFriendsTableInfo?.Invoke(respJson);
    }

    public static void UpdateFriendsListOnRemove(string json)
    {
        Log($"{DateTime.UtcNow} UpdateFriendsListOnDelete");
        var respJson = JsonConvert.DeserializeObject<Guid>(json);
        ReceiveFriendsDataFromServer.DeleteFriendFromFriendsList(respJson);
        Log($"{DateTime.UtcNow} RemoveChatToDictionary");
    }

    public static void ReceiveMoneyBox(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} ReceiveMoneyBox. Try to send request with empty ID.");
            return;
        }
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/MoneyBox/open/?playerId={guid}"), methodType: HTTPMethods.Get, callback: OnRequestFinished);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
                HTTP_UpdateTotalMoneyAndExperienceRequest();
            else Log($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public static void ReceiveNewFriendRequest(string json)
    {
        Log($"{DateTime.UtcNow} ReceiveNewRequest");
        var respJson = JsonConvert.DeserializeObject<List<FriendDto>>(json);
        OnReceiveFriendRequest?.Invoke(respJson.Except(ReceiveFriendsDataFromServer.friendRequestsList).First());
        ReceiveFriendsDataFromServer.AddFriendToRequestList(respJson);
    }

    public static void ReceiveMoneyBoxAmount(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} ReceiveMoneyBoxAmount. Try to send request with empty ID.");
            return;
        }
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/MoneyBox/get/?playerId={guid}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
            {
                var json = JsonConvert.DeserializeObject<MoneyBoxDto>(resp.DataAsText);
                OnReceiveCurrentMoneyBoxAmount?.Invoke(json.Value);
            }
            else
                Log($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public static void HTTP_SendGetImageRequest(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_SendGetImageRequest. Try to send request with empty ID.");
            return;
        }
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profileImage/get/?playerId={guid}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendGetImageRequest: {guid}");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.StatusCode != 200)
            {
                Log($"{DateTime.UtcNow} HTTP_OnGetImageRequestFinished {resp.StatusCode} {resp.Message} {resp.DataAsText}");
                return;
            }
            SetAvatarImage(GetHTTPResponseValueAs<PokerHand.Common.Helpers.Media.Avatar>(resp));
            OnReceiveMyProfileImage?.Invoke();
            Log($"{DateTime.UtcNow} {resp.StatusCode} {resp.Message} {resp.DataAsText}");
        }
    }

    public static void HTTP_SendTryAuthenticateWithExternalProvider(string providerKey)
    {
        TryAuthenticateWithProviderVM tryAuthenticateWithProviderVM = new TryAuthenticateWithProviderVM
        {
            ConnectionId = ConnectionId,
            ProviderKey = providerKey
        };
        var json = JsonConvert.SerializeObject(tryAuthenticateWithProviderVM);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/auth/tryAuthenticateWithExternalProvider"), HTTPMethods.Post, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(json);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendTryAuthenticateWithExternalProvider: {providerKey}");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_TryAuthenticateWithExternalProviderResponce: {resp.DataAsText}");
            switch (resp.StatusCode)
            {
                case 200:
                    if (PlayerProfileData.Instance.ExternalLogin != ExternalProviderName.None)
                        NetworkManager.Instance.SignOutWithProvider(PlayerProfileData.Instance.ExternalLogin);
                    var playerProfileDto = JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText);
                    ReceiveFriendsDataFromServer.SetAllChatsList(playerProfileDto.Value.ConversationDtos);
                    ReceiveFriendsDataFromServer.SetAllFriendsData(playerProfileDto.Value.FriendsWindowDto);
                    if (playerProfileDto.Value.NotificationDtos != null)
                        ReceiveFriendsDataFromServer.ShowNewMessagesNotification(playerProfileDto.Value.NotificationDtos);
                    Log($"{DateTime.UtcNow} HTTP_ReceivePlayerProfile");
                    OnReceiveMyPlayerProfile?.Invoke(playerProfileDto.Value.PlayerProfileDto);
                    break;
                case 400:
                    var authenticationResult = JsonConvert.DeserializeObject<AuthenticationResultType>
                        (JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText).Message);
                    switch (authenticationResult)
                    {
                        case AuthenticationResultType.PlayerIsAuthorized:
                            Log($"{DateTime.UtcNow} Player has already authorized");
                            NetworkManager.Instance.SignOutWithProvider(NetworkManager.ChosenProvider);
                            NetworkManager.ChosenProvider = ExternalProviderName.None;
                            NotificationController.ShowNotification(NotificationController.NotificationType.PlayerHasAlreadyAuthorized);
                            break;
                        case AuthenticationResultType.PlayerNotFound:
                            Log($"{DateTime.UtcNow} HTTP_ReceiveContinueRegistration");
                            OnReceiveContinueRegistration?.Invoke();
                            break;
                    }
                    break;
                default:
                    Log($"{DateTime.UtcNow} Server is not working");
                    NetworkManager.Instance.SignOutWithProvider(NetworkManager.ChosenProvider);
                    NetworkManager.ChosenProvider = ExternalProviderName.None;
                    NotificationController.ShowNotification(NotificationController.NotificationType.ServerNotWorking);
                    break;
            }
        }
    }

    public static void HTTP_SendRegisterWithExternalProvider
    (string userName, Gender gender, HandsSpriteType handsSprite, ExternalProviderName provider, string userId)
    {
        var vm = new PokerHand.Common.ViewModels.Profile.AddExternalProviderVM
        {
            ConnectionId = ConnectionId,
            Gender = gender,
            HandsSprite = handsSprite,
            ProviderKey = userId,
            ProviderName = provider,
            UserName = userName
        };
        var vmJson = JsonConvert.SerializeObject(vm);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/auth/registerWithExternalProvider"), HTTPMethods.Post, HTTP_PlayerProfileResponce);
        request.AddHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(vmJson);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendRegisterWithExternalProvider: {vmJson}");
    }

    public static void HTTP_SendRegisterAsGuest(string userName, Gender gender, HandsSpriteType handsType)
    {
        var vm = new RegisterAsGuestVM
        {
            ConnectionId = ConnectionId,
            Gender = gender,
            UserName = userName,
            HandsSprite = handsType
        };
        var vmJson = JsonConvert.SerializeObject(vm);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/auth/registerAsGuest"), HTTPMethods.Post, HTTP_PlayerProfileResponce);
        request.AddHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(vmJson);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendRegisterAsGuest: {vmJson}");
    }

    public static void HTTP_SendAuthenticateRequest()
    {
        var vm = new AuthenticateVM
        {
            UserId = PlayerProfileData.Instance.Id,
            ConnectionId = ConnectionId
        };
        var vmJson = JsonConvert.SerializeObject(vm);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Auth/authenticate"), HTTPMethods.Post, Responce);
        request.AddHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(vmJson);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendAuthenticateRequest: {vmJson}");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_GetAuthenticateResponce: {resp.DataAsText}");
            switch(resp.StatusCode)
            {
                case 200:
                    var playerProfileDto = JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText);
                    ReceiveFriendsDataFromServer.SetAllChatsList(playerProfileDto.Value.ConversationDtos);
                    ReceiveFriendsDataFromServer.SetAllFriendsData(playerProfileDto.Value.FriendsWindowDto);
                    if (playerProfileDto.Value.NotificationDtos != null)
                        ReceiveFriendsDataFromServer.ShowNewMessagesNotification(playerProfileDto.Value.NotificationDtos);
                    ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView?.Invoke();
                    OnReceiveMyPlayerProfile?.Invoke(playerProfileDto.Value.PlayerProfileDto);
                    break;
                case 400:
                    var authenticationResult = JsonConvert.DeserializeObject<AuthenticationResultType>
                      (JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText).Message);
                    switch (authenticationResult)
                    {
                        case AuthenticationResultType.PlayerIsAuthorized:
                            Log($"{DateTime.UtcNow} Player has already authorized");
                            NetworkManager.Instance.SignOutWithProvider(NetworkManager.ChosenProvider);
                            NetworkManager.ChosenProvider = ExternalProviderName.None;
                            NotificationController.ShowNotification(NotificationController.NotificationType.PlayerHasAlreadyAuthorized);
                            break;
                        case AuthenticationResultType.PlayerNotFound:
                            Log($"{DateTime.UtcNow} HTTP_ReceiveContinueRegistration");
                            OnReceiveContinueRegistration?.Invoke();
                            break;
                    }
                    break;
                default:
                    Log($"{DateTime.UtcNow} Server is not working");
                    NetworkManager.Instance.SignOutWithProvider(NetworkManager.ChosenProvider);
                    NetworkManager.ChosenProvider = ExternalProviderName.None;
                    NotificationController.ShowNotification(NotificationController.NotificationType.ServerNotWorking);
                    break;
            }
            if (resp.StatusCode == 200)
            {
                var playerProfileDto = JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText);
                ReceiveFriendsDataFromServer.SetAllChatsList(playerProfileDto.Value.ConversationDtos);
                ReceiveFriendsDataFromServer.SetAllFriendsData(playerProfileDto.Value.FriendsWindowDto);
                if (playerProfileDto.Value.NotificationDtos != null)
                    ReceiveFriendsDataFromServer.ShowNewMessagesNotification(playerProfileDto.Value.NotificationDtos);
                ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView?.Invoke();
                OnReceiveMyPlayerProfile?.Invoke(playerProfileDto.Value.PlayerProfileDto);
            }
            else
                LogError($"{DateTime.UtcNow} HTTP_GetAuthenticate failed");
        }
    }

    public static void HTTP_UpdateTotalMoneyAndExperienceRequest()
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_UpdateTotalMoneyAndExperienceRequest. Try to send request with empty ID.");
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            LogError($"{DateTime.UtcNow} Failed to update total money and experience: no Internet");
            return;
        }
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/GetTotalMoneyAndExperience/?playerId={PlayerProfileData.Instance.Id}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_UpdateTotalMoneyAndExperienceRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_GetAuthenticateResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
            {
                var moneyAndExperience = GetHTTPResponseValueAs<LeaveTableDto>(resp);
                PlayerProfileData.Instance.UpdateTotalMoney(moneyAndExperience.TotalMoney);
                PlayerProfileData.Instance.UpdateExperience(moneyAndExperience.Experience);
            }
            else
                LogError($"{DateTime.UtcNow} HTTP_UpdateTotalMoneyAndExperience failed");
        }
    }

    private static bool dailyRewardTime = false;

    public static void HTTP_GetLastDailyRewardTimeRequest(PlayerProfileDto profileDto)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_GetLastDailyRewardTimeRequest. Try to send request with empty ID.");
            return;
        }
        if (dailyRewardTime)
            return;
        else
            dailyRewardTime = true;
        OnReceiveMyPlayerProfile -= HTTP_GetLastDailyRewardTimeRequest;
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/getDailyRewardReceiveTime/?playerId={profileDto.Id}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_GetLastDailyRewardTimeRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_GetLastDailyRewardTimeResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
            {
                PlayerPrefs.SetString("CollectedRewardTime", resp.DataAsText.Replace("\"", ""));
                OnRewardedTimeReceived?.Invoke();
            }
            else
                LogError($"{DateTime.UtcNow} HTTP_GetLastDailyRewardTime failed");
        }
    }

    public static void HTTP_GetLastFreeSpinTimeRequest(PlayerProfileDto profileDto)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_GetLastFreeSpinTimeRequest. Try to send request with empty ID.");
            return;
        }
        OnReceiveMyPlayerProfile -= HTTP_GetLastFreeSpinTimeRequest;
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/getFreeSpinReceiveTime/?playerId={profileDto.Id}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_GetLastDailyRewardTimeRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_GetLastFreeSpinTimeResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
            {
                var freeSpinTime = JsonConvert.DeserializeObject<FreeSpinCounter>(resp.DataAsText);
                PlayerPrefs.SetInt("watchedAds", freeSpinTime.NumberOfScrollsLeft);
                PlayerPrefs.SetString("lastEntry", freeSpinTime.LastScrollTime.ToString(GameConstants.dateTimeFormat));
                OnFreeSpinTimeReceived?.Invoke();
            }
            else
                LogError($"{DateTime.UtcNow} HTTP_GetLastFreeSpinTime failed");
        }
    }

    public static void HTTP_GetDailyTasksRequest(PlayerProfileDto profileDto)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_GetDailyTasksRequest. Try to send request with empty ID.");
            return;
        }
        OnReceiveMyPlayerProfile -= HTTP_GetDailyTasksRequest;
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/getDailyTasks/?playerId={profileDto.Id}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_GetDailyTasksRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_GetDailyTasksRequestResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
            {
                DailyTaskServerData[] tasks;
                if (string.IsNullOrEmpty(resp.DataAsText))
                    tasks = new DailyTaskServerData[0];
                else
                {
                    string[] tasksString = resp.DataAsText.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                    tasks = new DailyTaskServerData[tasksString.Length];
                    for (int i = 0; i < tasksString.Length; ++i)
                    {
                        tasks[i] = JsonConvert.DeserializeObject<DailyTaskServerData>(tasksString[i]);
                        if (tasks[i].CreationTime == null)
                            tasks[i].CreationTime = TimeManager.GetTime();
                    }
                }
                TasksTracker.Instance.LoadTasks(tasks);
            }
            else
                LogError($"{DateTime.UtcNow} HTTP_GetLastFreeSpinTime failed");
        }
    }

    public static void HTTP_UpdateLastDailyRewardTimeRequest()
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_UpdateLastDailyRewardTimeRequest. Try to send request with empty ID.");
            return;
        }
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/updateDailyRewardReceiveTime/?playerId={PlayerProfileData.Instance.Id}"), HTTPMethods.Put, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendLastDailyRewardTimeRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_GetAuthenticateResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
                Log($"{DateTime.UtcNow} LastDailyRewardTime updated");
            else
                LogError($"{DateTime.UtcNow} HTTP_UpdateLastDailyRewardTime failed");
        }
    }

    public static void HTTP_UpdateFreeSpinTimeRequest(FreeSpinCounter freeSpinTime)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_UpdateFreeSpinTimeRequest. Try to send request with empty ID.");
            return;
        }
        var vmJson = JsonConvert.SerializeObject(freeSpinTime);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/updateFreeSpinReceiveTime/?playerId={PlayerProfileData.Instance.Id}&freeSpinCounter={vmJson}"), HTTPMethods.Put, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_UpdateFreeSpinTimeRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_UpdateFreeSpinTimeResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
                Log($"{DateTime.UtcNow} FreeSpinTime updated");
            else
                LogError($"{DateTime.UtcNow} HTTP_UpdateFreeSpinTime failed");
        }
    }

    public static void HTTP_UpdateDailyTasksRequest(DailyTask[] tasks)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_UpdateDailyTasksRequest. Try to send request with empty ID.");
            return;
        }
        DailyTaskServerData[] serverTasks = new DailyTaskServerData[tasks.Length];
        for (int i = 0; i < tasks.Length; ++i)
            serverTasks[i] = new DailyTaskServerData()
            {
                CollectedRewardTime = tasks[i].CollectedRewardTime,
                CurrentAmount = tasks[i].CurrentAmount,
                IsCollectedReward = tasks[i].IsCollectedReward,
                IsDone = tasks[i].IsDone,
                TaskName = tasks[i].TaskName
            };
        string vmJson = "";
        foreach (var task in serverTasks)
            vmJson += JsonConvert.SerializeObject(task) + "|||";
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/updateDailyTasks/?playerId={PlayerProfileData.Instance.Id}&dailyTasks={vmJson}"), HTTPMethods.Put, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_UpdateDailyTasksRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_UpdateDailyTasksResponce: {resp.DataAsText}");
            if (resp.StatusCode == 200)
                Log($"{DateTime.UtcNow} DailyTasks updated");
            else
                LogError($"{DateTime.UtcNow} HTTP_UpdateDailyTasks failed");
        }
    }

    public static void HTTP_SendGetPlayerProfile(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_UpdateFreeSpinTimeRequest. Try to send request with empty ID.");
            return;
        }
        var guidString = guid.ToString();
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/get/?playerId={guidString}"), HTTPMethods.Get, HTTP_PlayerProfileResponce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendGetPlayerProfile {guidString}");
    }

    public static void HTTP_SendUpdatePlayerProfile()
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_SendUpdatePlayerProfile. Try to send request with empty ID.");
            return;
        }
        var vm = new PlayerProfileUpdateVM
        {
            Id = PlayerProfileData.Instance.Id,
            Country = PlayerProfileData.Instance.Country,
            Gender = PlayerProfileData.Instance.Gender,
            HandsSprite = PlayerProfileData.Instance.HandsSpriteType,
            UserName = PlayerProfileData.Instance.Nickname
        };
        var vmJson = JsonConvert.SerializeObject(vm);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/update/"), HTTPMethods.Put, HTTP_PlayerProfileResponce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(vmJson);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendUpdatePlayerProfile: {vmJson}");
    }

    public static void HTTP_SendUpdateProfileImage(byte[] imageBytes)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_SendUpdateProfileImage. Try to send request with empty ID.");
            return;
        }
        var texture = new Texture2D(0, 0);
        texture.LoadImage(imageBytes);
        if (imageBytes.Length > GameConstants.MaxAvatarSize * GameConstants.MaxAvatarSize)
        {
            texture = texture.ScaleTexture(GameConstants.MaxAvatarSize, GameConstants.MaxAvatarSize);
            imageBytes = texture.EncodeToPNG();
        }
        ProfileImages[0] = texture;
        var convertedBytes = Convert.ToBase64String(imageBytes);
        var vm = new UpdateProfileImageVM
        {
            PlayerId = PlayerProfileData.Instance.Id,
            NewProfileImage = convertedBytes
        };
        var vmJson = JsonConvert.SerializeObject(vm);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/ProfileImage/update"), HTTPMethods.Put, Responce);
        request.AddHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(vmJson);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendUpdateProfileImage: {PlayerProfileData.Instance.Id} {convertedBytes}");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.StatusCode != 200)
                Log($"{DateTime.UtcNow} HTTP_OnGetImageRequestFinished {resp.StatusCode} {resp.Message} {resp.DataAsText}");
            else
            {
                OnReceiveMyProfileImage?.Invoke();
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "AvatarImage.png"), ProfileImages[0].EncodeToPNG());
            }
        }
    }

    public static void HTTP_SendAllTablesInfoRequest()
    {
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Table/getAllInfo"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendGetAllTablesInfoRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            TablesInfo = GetHTTPResponseValueAs<List<TableInfoDto>>(resp);
            Log($"{DateTime.UtcNow} HTTP_ReceiveAllTablesInfo: {resp.DataAsText}");
            OnReceiveAllTablesInfo?.Invoke();
        }
    }

    public static void HTTP_SendPresentsInfoRequest()
    {
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/present/getall"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendPresentsInfoRequest");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            PresentsInfo = GetHTTPResponseValueAs<List<PresentInfoDto>>(resp);
            Log($"{DateTime.UtcNow} HTTP_PresentsInfoResponse: {resp.DataAsText}");
            OnReceivePresentsInfo?.Invoke();
        }
    }

    public static void HTTP_SendTableInfoRequest(TablesInWorlds tableName)
    {
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Table/getInfo/?tableTitle={tableName}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        Log($"{DateTime.UtcNow} {DateTime.UtcNow} HTTP_SendGetTableInfoRequest with table name {tableName}");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            TableInfo = GetHTTPResponseValueAs<TableInfoDto>(resp);
            Log($"{DateTime.UtcNow} {DateTime.UtcNow} HTTP_ReceiveTableInfo: {resp.DataAsText}");
            OnReceiveTableInfo?.Invoke();
        }
    }

    public static void HTTP_AddExternalProviderRequest(ExternalProviderName newProvider, string providerKey)
    {
        TryAuthenticateWithProviderVM tryAuthenticateWithProviderVM = new TryAuthenticateWithProviderVM
        {
            ConnectionId = ConnectionId,
            ProviderKey = providerKey
        };
        var json = JsonConvert.SerializeObject(tryAuthenticateWithProviderVM);
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/auth/tryAuthenticateWithExternalProvider"), HTTPMethods.Post, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(json);
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendTryAuthenticateWithExternalProvider: {providerKey}");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            Log($"{DateTime.UtcNow} HTTP_TryAuthenticateWithExternalProviderResponce: {resp.DataAsText}");
            switch(resp.StatusCode)
            {
                case 200:
                    if (PlayerProfileData.Instance.ExternalLogin != ExternalProviderName.None)
                        NetworkManager.Instance.SignOutWithProvider(PlayerProfileData.Instance.ExternalLogin);
                    var playerProfileDto = JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText);
                    ReceiveFriendsDataFromServer.SetAllChatsList(playerProfileDto.Value.ConversationDtos);
                    ReceiveFriendsDataFromServer.SetAllFriendsData(playerProfileDto.Value.FriendsWindowDto);
                    if (playerProfileDto.Value.NotificationDtos != null)
                        ReceiveFriendsDataFromServer.ShowNewMessagesNotification(playerProfileDto.Value.NotificationDtos);
                    Log($"{DateTime.UtcNow} HTTP_ReceivePlayerProfile");
                    OnReceiveMyPlayerProfile?.Invoke(playerProfileDto.Value.PlayerProfileDto);
                    break;
                case 400:
                    var authenticationResult = JsonConvert.DeserializeObject<AuthenticationResultType>
                        (JsonConvert.DeserializeObject<GetAuthenticationResultDto>(resp.DataAsText).Message);
                    switch (authenticationResult)
                    {
                        case AuthenticationResultType.PlayerIsAuthorized:
                            Log($"{DateTime.UtcNow} Player has already authorized");
                            NetworkManager.Instance.SignOutWithProvider(NetworkManager.ChosenProvider);
                            NetworkManager.ChosenProvider = ExternalProviderName.None;
                            NotificationController.ShowNotification(NotificationController.NotificationType.PlayerHasAlreadyAuthorized);
                            break;
                        case AuthenticationResultType.PlayerNotFound:
                            NetworkManager.Instance.RegisterWithProvider
                            (
                                PlayerProfileData.Instance.Nickname,
                                PlayerProfileData.Instance.Gender,
                                PlayerProfileData.Instance.HandsSpriteType,
                                newProvider,
                                providerKey
                            );
                            break;
                    }
                    break;
                default:
                    Log($"{DateTime.UtcNow} Server is not working");
                    NetworkManager.Instance.SignOutWithProvider(NetworkManager.ChosenProvider);
                    NetworkManager.ChosenProvider = ExternalProviderName.None;
                    NotificationController.ShowNotification(NotificationController.NotificationType.ServerNotWorking);
                    break;
            }
        }
    }

    public static void SendConnectToTableRequest(TablesInWorlds tableTitle, long buyIn)
    {
        isConnectedToTable = false;
        var connectToTableOptions = new ConnectToTableByTitleOptions() 
        {
            TableTitle = (TableTitle)((int)SwitchDashTable(tableTitle, buyIn) + 1),
            PlayerId = PlayerProfileData.Instance.Id,
            PlayerConnectionId = ConnectionId,
            BuyInAmount = buyIn,
            IsAutoTop = GameModeSettings.Instance.gameMode == GameModes.SitNGo ? false : GameModeSettings.Instance.autoTop
        };
        Hub.SendAsync(ServerMethods.ConnectToTable, JsonConvert.SerializeObject(connectToTableOptions));
        Log($"{DateTime.UtcNow} {connectToTableOptions}");
    }

    public static void SendConnectToTableById(Guid tableId, Guid playerId, long buyIn)
    {
        isConnectedToTable = false;
        var connectToTableOptions = new TableConnectByIdOptions()
        {
            TableId = tableId,
            PlayerId = playerId,
            PlayerConnectionId = ConnectionId,
            BuyInAmount = buyIn,
            IsAutoTop = GameModeSettings.Instance.gameMode == GameModes.SitNGo ? false : GameModeSettings.Instance.autoTop
        };
        Hub.SendAsync(ServerMethods.ConnectToTableById, JsonConvert.SerializeObject(connectToTableOptions));
        Log($"{DateTime.UtcNow} SendConnectToTableRequestById");
    }

    public static void SendSwitchTableRequest(TablesInWorlds tableTitle, long buyIn)
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} SendSwitchTableRequest. Try to send request with empty ID.");
            return;
        }
        isConnectedToTable = false;
        var connectToTableOptions = new ConnectToTableByTitleOptions()
        {
            TableTitle = (TableTitle)((int)SwitchDashTable(tableTitle, buyIn) + 1),
            PlayerId = PlayerProfileData.Instance.Id,
            CurrentTableId = TableData.Id,
            PlayerConnectionId = ConnectionId,
            BuyInAmount = buyIn,
            IsAutoTop = GameModeSettings.Instance.gameMode == GameModes.SitNGo ? false : GameModeSettings.Instance.autoTop
        };
        Hub.SendAsync(tableTitle == 0 ? ServerMethods.SwitchDashTable : ServerMethods.SwitchTable, JsonConvert.SerializeObject(connectToTableOptions));
        Log($"{DateTime.UtcNow} Send SwitchTable request playerId: {connectToTableOptions}");
    }

    public static void SendPlayerAction(PlayerActionType actionType, long amount = 0)
    {
        OnReceivePlayerActionType?.Invoke(actionType);
        var playerAction = new PlayerAction
        {
            ActionType = actionType,
            Amount = amount,
            PlayerIndexNumber = MyPlayer.IndexNumber
        };
        var jsonAction = JsonConvert.SerializeObject(playerAction);
        var jsonTableId = JsonConvert.SerializeObject(TableData.Id);
        Hub.SendAsync(ServerMethods.ReceivePlayerActionFromClient, jsonAction, jsonTableId);
        Log($"{DateTime.UtcNow} Send my action: {playerAction.ActionType} with amount {playerAction.Amount}");
        LastPlayerAction = playerAction;
        IsMyFirstTurnInGame = false;
    }

    public static void SendActivePlayerStatus()
    {
        var tableId = TableData.Id.ToString();
        var playerId = MyPlayer.Id.ToString();
        Hub.SendAsync(ServerMethods.SendActivePlayerStatusRequest, tableId, playerId);
        Log($"{DateTime.UtcNow} Send ActivePlayerStatus: table guid {tableId} player guid {playerId}");
    }

    public static void SendShowWinnerCards()
    {
        var tableId = TableData.Id.ToString();
        Hub.SendAsync(ServerMethods.ShowWinnerCards, tableId);
        Log($"{DateTime.UtcNow} Send ShowWinnerCards: table guid {tableId}");
    }

    public static void SendQuickMessage(QuickMessage quickMessage)
    {
        var tableId = TableData.Id.ToString();
        var senderIndex = MyPlayer.IndexNumber.ToString();
        var message = JsonConvert.SerializeObject(quickMessage);
        Hub.SendAsync(ServerMethods.SendQuickMessage, message, tableId, senderIndex);
        Log($"{DateTime.UtcNow} Send ActivePlayerStatus: {message} table guid {tableId} player guid {senderIndex}");
    }

    public static void SendLeaveTable()
    {
        var tableId = JsonConvert.SerializeObject(TableData.Id);
        var playerId = JsonConvert.SerializeObject(MyPlayer.Id);
        Hub.SendAsync(ServerMethods.LeaveTable, tableId, playerId);
        Log($"{DateTime.UtcNow} Send LeaveTable: table guid {tableId} player guid {playerId}");
        TableData = null;
    }

    public static void SendAddStackMoney(int amount)
    {
        var jsonTableId = JsonConvert.SerializeObject(TableData.Id);
        var jsonPlayerId = JsonConvert.SerializeObject(MyPlayer.Id);
        var jsonMoneyAmount = JsonConvert.SerializeObject(amount);
        var jsonIsAutoTop = JsonConvert.SerializeObject(GameModeSettings.Instance.autoTop);
        Hub.SendAsync(ServerMethods.ReceiveNewBuyIn, jsonTableId, jsonPlayerId, jsonMoneyAmount, jsonIsAutoTop);
        Log($"{DateTime.UtcNow} Send AddStackMoney: table guid {jsonTableId} player guid {jsonPlayerId} moneyAmount {jsonMoneyAmount}");
    }

    public static void SendPresent(List<Guid> recepientIds, PresentName presentName)
    {
        PresentDto present = new PresentDto()
        {
           Name = presentName,
           SenderId = MyPlayer.Id,
           RecipientsIds = recepientIds
        };
        Hub.SendAsync(ServerMethods.SendPresent, JsonConvert.SerializeObject(present));
    }
    #endregion

    #region Receiving

    static void SetAvatarImage(PokerHand.Common.Helpers.Media.Avatar avatar)
    {
        var texture = new Texture2D(0, 0);
        if (!string.IsNullOrEmpty(avatar.BinaryImage))
            texture.LoadImage(Convert.FromBase64String(avatar.BinaryImage));
        else
            texture = Resources.Load<Texture2D>("Images/Avatar_standart");
        if (PlayerProfileData.Instance.Id == avatar.PlayerId)
        {
            ProfileImages[0] = texture;
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "AvatarImage.png"), texture.EncodeToPNG());
        }
        else
        {
            int playerIndex = TableData.Players.First(t => t.Id == avatar.PlayerId).LocalIndex;
            ProfileImages[playerIndex] = texture;
        }
    }

    private static void HTTP_PlayerProfileResponce(HTTPRequest req, HTTPResponse resp)
    {
        Log($"{DateTime.UtcNow} HTTP_GetPlayerProfileResponce: {resp.DataAsText}");
        var playerProfileDto = GetHTTPResponseValueAs<PlayerProfileDto>(resp);
        if (PlayerProfileData.Instance.Id == Guid.Empty || playerProfileDto.Id == PlayerProfileData.Instance.Id)
        {
            OnReceiveMyPlayerProfile?.Invoke(playerProfileDto);
            Log($"{DateTime.UtcNow} OnReceiveMyPlayerProfile - {playerProfileDto.Id}");
        }
        else
        {
            OnReceiveAnotherPlayerProfile?.Invoke(playerProfileDto);
            Log($"{DateTime.UtcNow} OnReceiveAnotherPlayerProfile - {playerProfileDto.Id}");
        }
    }

    public static void HTTP_GetPlayerRank(Guid playerGuid, long totalMoney)
    {
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Ranking/getPlayerRank/?playerId={playerGuid}&totalMoney={totalMoney}"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
            {
                RankDto rank = JsonConvert.DeserializeObject<RankDto>(resp.DataAsText);
                OnReceiveRank?.Invoke(rank);
            }
            else
                Log($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public static void HTTP_GetTopPlayers()
    {
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/Ranking/getTopPlayers/"), HTTPMethods.Get, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.IsSuccess)
            {
                IEnumerable<PlayerWithRankDto> ranks = JsonConvert.DeserializeObject<IEnumerable<PlayerWithRankDto>>(resp.DataAsText);
                OnReceivePlayersRank?.Invoke(ranks);
            }
            else
                Log($"{DateTime.UtcNow} {resp.StatusCode}");
        }
    }

    public static void HTTP_DeleteAccount()
    {
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            LogError($"{DateTime.UtcNow} HTTP_SendUpdatePlayerProfile. Try to send request with empty ID.");
            return;
        }
        var request = new HTTPRequest(new Uri($"{Hub.uriString}/api/profile/deletePlayer/?playerId={PlayerProfileData.Instance.Id}"), HTTPMethods.Delete, Responce);
        request.SetHeader("Authorization", $"Bearer {NetworkManager.Token}");
        request.Send();
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if(resp.StatusCode == 200)
            {
                DeleteUserProfile.DeleteFinal();
            }
        }
    }

    public static void ReceiveQuickMessage(string json)
    {
        var messageDto = JsonConvert.DeserializeObject<QuickMessageDto>(json);
        Log($"{DateTime.UtcNow} ReceiveQuickMessage: {json}");
        OnReceiveQuickMessage?.Invoke(messageDto);
    }

    public static void ReceiveInitialTableState(string jsonString, string jsonAvatars)
    {
        TableData = JsonConvert.DeserializeObject<TableDto>(jsonString);
        MyPlayer = TableData.Players.First(t => t.Id == PlayerProfileData.Instance.Id);
        IsMyPlayerActive = TableData.ActivePlayers.Any(t => t.Id == MyPlayer.Id);
        var avatars = JsonConvert.DeserializeObject<PokerHand.Common.Helpers.Media.Avatar[]>(jsonAvatars);
        foreach (var avatar in avatars)
            SetAvatarImage(avatar);
        isConnectedToTable = true;
        Log($"{DateTime.UtcNow} OnFirstTableDataReceived");
        OnFirstTableDataReceived?.Invoke();
        PlayerProfileData.Instance.UpdateTotalMoney(MyPlayer.TotalMoney);
    }

    public static void ReceiveNewPlayerImage(string avatar)
    {
        SetAvatarImage(JsonConvert.DeserializeObject<PokerHand.Common.Helpers.Media.Avatar>(avatar));
        OnReceiveMyProfileImage?.Invoke();
    }

    public static void ReceiveNewPlayerImage(PokerHand.Common.Helpers.Media.Avatar avatar)
    {
        SetAvatarImage(avatar);
        OnReceiveMyProfileImage?.Invoke();
    }

    public static void ReceiveTableState(string jsonString)
    {
        var prevIds = TableData.Players.Select(p => p.Id);
        var newTableData = JsonConvert.DeserializeObject<TableDto>(jsonString);
        var disconnectedPlayers = prevIds.Except(newTableData.Players.Select(t => t.Id));
        foreach (var player in disconnectedPlayers)
            ReceivePlayerDisconnected(player);
        TableData = newTableData;
        MyPlayer = TableData.Players.First(t => t.Id == PlayerProfileData.Instance.Id);
        if (TableData.ActivePlayers.Count != 0)
            OnReceiveTableState?.Invoke(TableData);
        var connectedPlayers = TableData.Players.Select(t => t.Id).Except(prevIds);
        foreach (var player in connectedPlayers)
        {
            OnAnotherPlayerConnected?.Invoke(TableData.Players.First(t => t.Id == player));
            ReceiveNewPlayerImage(new PokerHand.Common.Helpers.Media.Avatar { BinaryImage = null, PlayerId= player });
        }
        if (TableData.CurrentStage > RoundStageType.NotStarted)
            IsBlindsReleased = true;
        IsMyPlayerActive = TableData.ActivePlayers.Any(t => t.Id == MyPlayer.Id);
        Log($"{DateTime.UtcNow} Receive Table: {jsonString}");
        OnReceiveTable?.Invoke();
        PlayerProfileData.Instance.UpdateTotalMoney(MyPlayer.TotalMoney);
    }

    public static void ReceiveUpdatedPot(string json)
    {
        var value = JsonConvert.DeserializeObject<Pot>(json);
        TableData.Pot = value;
        Log($"{DateTime.UtcNow} ReceiveUpdatedPot: {json}");
        OnReceiveUpdatedPot?.Invoke(value);
    }

    public static void ReceiveCurrentPlayerId(string jsonString)
    {
        var currentPlayerId = JsonConvert.DeserializeObject<Guid>(jsonString);
        CurrentPlayer = TableData.ActivePlayers.First(t => t.Id == currentPlayerId);
        Log($"{DateTime.UtcNow} ReceiveCurrentPlayerId: " + jsonString);
        OnReceiveCurrentPlayerId?.Invoke();
    }

    public static void ReceivePrepareForGame(string json)
    {
        TableData = JsonConvert.DeserializeObject<TableDto>(json);
        IsBlindsReleased = false;
        IsMyFirstTurnInGame = true;
        if (TableData.ActivePlayers.Select(t => t.IndexNumber).Contains(MyPlayer.IndexNumber))
            MyPlayer.PocketCards = TableData.ActivePlayers.First(t => t.IndexNumber == MyPlayer.IndexNumber).PocketCards;
        else
            MyPlayer.PocketCards = null;
        Log($"{DateTime.UtcNow} ReceivePrepareForGame: {json}");
        OnReceivePrepareForGame?.Invoke();
    }

    public static void ReceivePresent(string json)
    {
        var present = JsonConvert.DeserializeObject<PresentDto>(json);
        Log($"{DateTime.UtcNow} ReceivePresent: {json}");
        OnReceivePresent?.Invoke(present);
    }

    public static void ReceiveTotalMoney(string json)
    {
        var totalMoney = JsonConvert.DeserializeObject<int>(json);
        Log($"{DateTime.UtcNow} ReceiveTotalMoney: {json}");
        OnReceiveMyPlayerTotalMoney?.Invoke(totalMoney);
    }

    public static void ReceiveErrorOnSendPresent(string json)
    {
        var error = JsonConvert.DeserializeObject<SendPresentError>(json);
        Log($"ReceiveErrorOnSendPresent: {json}");
    }

    public static void ReceiveDealCommunityCards(string jsonString)
    {
        CommunityCards = JsonConvert.DeserializeObject<List<CardDto>>(jsonString);
        // Keeps players current bets in actual state (at now tableState took up earlier then bets sets to 0)
        foreach (var p in TableData.Players)
            p.CurrentBet = 0;
        TableData.CurrentMaxBet = 0;
        Log($"{DateTime.UtcNow} ReceiveDealCommunityCards: {jsonString}");
        OnReceiveDealCommunityCards?.Invoke();
    }

    public static void ReceivePlayerAction(string jsonString)
    {
        LastPlayerAction = JsonConvert.DeserializeObject<PlayerAction>(jsonString);
        Log($"{DateTime.UtcNow} ReceivePlayerAction: {LastPlayerAction.ActionType} | Index {LastPlayerAction.PlayerIndexNumber} | Amount: {LastPlayerAction.Amount}");
        OnReceivePlayerAction?.Invoke();
    }

    public static void ReceivePlayerDisconnected(string jsonString)
    {
        var newTableData = JsonConvert.DeserializeObject<TableDto>(jsonString);
        var disconnectedPlayers = TableData.Players.Select(t => t.Id).Except(newTableData.Players.Select(t => t.Id));
        if (disconnectedPlayers.Count() == 0)
            return;
        foreach(var player in disconnectedPlayers)
            ReceivePlayerDisconnected(player);
        TableData = newTableData;
        OnReceiveTable?.Invoke();
    }

    public static void ReceivePlayerDisconnected(Guid disconnectedPlayer)
    {
        int disconnectedPlayerLocalIndex = TableData.Players.First(t => t.Id == disconnectedPlayer).LocalIndex;
        if (disconnectedPlayerLocalIndex == 0)
            return;
        Log($"{DateTime.UtcNow} Receive PlayerDisconnected with Guid {disconnectedPlayerLocalIndex}: {disconnectedPlayer}");
        ProfileImages[disconnectedPlayerLocalIndex] = null;
        OnPlayerDisconnected?.Invoke(disconnectedPlayerLocalIndex);
    }

    public static void ReceiveWinners(string json)
    {
        WinnersPots = JsonConvert.DeserializeObject<List<SidePotDto>>(json);
        Winners = WinnersPots.First(p => p.Type == SidePotType.Main)?.Winners;
        CurrentPlayer = null;
        if (Winners != null)
        {
            Log($"{DateTime.UtcNow} [WINNERS] ReceiveWinnersSidePots" + json);
            OnReceiveWinners?.Invoke();
            OnReceiveWinningList?.Invoke(Winners, WinnersPots);
        }
    }

    public static void ReceiveEndSitAndGoGame(string json)
    {
        int myPlace;
        int.TryParse(json, out myPlace);
        Log($"{DateTime.UtcNow} ReceiveEndSitAndGoGame {json}");
        OnReceiveEndSitAndGoGame?.Invoke(myPlace);
    }

    public static void ContinueRegistration()
    {
        Log($"{DateTime.UtcNow} ReceiveContinueRegistration");
        OnReceiveContinueRegistration?.Invoke();
    }

    public static void ReceiveOnLackOfStackMoney()
    {
        Log($"{DateTime.UtcNow} ReceiveOnLackOfStackMoney");
        OnReceiveOnLackOfStackMoney?.Invoke();
    }

    public static void ReceiveOnGameEnd()
    {
        Log($"{DateTime.UtcNow} ReceiveOnGameEnd");
        OnReceiveOnGameEnd?.Invoke();
    }

    public static void ShowWinnerCards()
    {
        Log($"{DateTime.UtcNow} Receive ShowWinnerCards");
        OnReceiveShowWinnerCards?.Invoke();
    }

    public static void ShowMessageOnLackOfMoneyInDash()
    {
        Log($"{DateTime.UtcNow} ShowMessageOnLackOfMoneyInDash");
        OnReceiveMessageOnLackOfMoneyInDash?.Invoke();
    }

    public static void ReceivePrivateMessage(string json)
    {
        Log($"{DateTime.UtcNow} ReceivePrivateMessage");
        var dictionary = ReceiveFriendsDataFromServer.allUnreadMessagesDictionary;
        var respJson = JsonConvert.DeserializeObject<PrivateMessageDto>(json);
        if (dictionary.ContainsKey(respJson.SenderId))
            dictionary[respJson.SenderId]++;
        else
            dictionary.Add(respJson.SenderId, 1);
        OnReceivePrivateMessage?.Invoke(respJson);
        if (!StaticRuntimeSets.Items.ContainsKey("ChatPanel(Clone)"))
            ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView?.Invoke();
    }
    #endregion

    public static PlayerDto GetPlayerByLocalIndex(int localIndex) => TableData.Players.First(t => t.LocalIndex == localIndex);

    public static bool IsMyTurnWillBeTheLast() => TableData.ActivePlayers.Count <= 2 || TableData.ActivePlayers.Count(p => p.PocketCards?.Count > 0) <= 2;

    public static bool IsPlayerExists(int localIndex) => TableData.Players.Count(p => p.LocalIndex == localIndex) > 0;

    public static bool IsTableInAllInState() => TableData.ActivePlayers.Any(t => t.StackMoney == 0);

    public static int ToLocalIndex(this int indexDto)
    {
        var localIndex = indexDto - MyPlayer.IndexNumber;
        if (localIndex < 0)
            localIndex += GameModeSettings.Instance.countPeople;
        return localIndex;
    }

    static T GetHTTPResponseValueAs<T>(HTTPResponse response) => 
        JsonConvert.DeserializeObject<T>(JsonConvert.DeserializeObject<ResponseContent>(response.DataAsText).Value.ToString());

    public static void SetCurrentPlayerToNull()
    {
        CurrentPlayer = null;
        MyPlayer = null;
        TableData = null;
    }

    public static void PressedDashTable() => OnPressedFoldDash?.Invoke();

    [Serializable]
    public class TryAuthenticateWithProviderVM
    {
        public string ProviderKey { get; set; }
        public string ConnectionId { get; set; }
    }

    private static TablesInWorlds SwitchDashTable(TablesInWorlds tableTitle, long buyIn)
    {
        if (tableTitle == 0)
        {
            switch (buyIn)
            {
                case 300000:
                    return TablesInWorlds.Dash300K;

                case 1000000:
                    return TablesInWorlds.Dash1M;

                case 10000000:
                    return TablesInWorlds.Dash10M;

                //case 100000000:
                //    return TablesInWorlds.Dash100M;
            }
        }
        return tableTitle;
    }
}