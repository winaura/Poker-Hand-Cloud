using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.Debug;

public static class ServerMethods
{
    public const string ReceivePlayerActionFromClient = "ReceivePlayerActionFromClient"; // string PlayerAction, string tableId
    public const string SendActivePlayerStatusRequest = "ReceiveActivePlayerStatus"; // Guid tableId, Guid playerId
    // Auth
    public const string Authenticate = "Authenticate"; // Guid playerId
    public const string RegisterAsGuest = "RegisterAsGuest"; // string userNameJson, string genderJson, string playerHandsTypeJson
    public const string RegisterWithExternalProvider = "RegisterWithExternalProvider";
    // Profile
    public const string RemoveProfileImage = "RemoveProfileImage"; // string imageJson
    public const string SendPlayerProfile = "SendPlayerProfile"; // Guid playerId
    // Table
    public const string ConnectToTable = "ConnectToTable"; // string tableTitleJson, string playerIdJson, string buyInJson, string isAutoTopJson
    public const string LeaveTable = "LeaveTable"; // tableId, playerId
    public const string SwitchTable = "SwitchTable"; // string tableTitleJson, string playerIdJson, string currentTableIdJson, string buyInJson, string isAutoTopJson
    public const string SwitchDashTable = "SwitchDashTable";
    public const string GetTableInfo = "GetTableInfo"; // string tableName
    public const string GetAllTablesInfo = "GetAllTablesInfo"; // -
    public const string ShowMessageOnLackOfMoneyInDash = "ShowMessageOnLackOfMoneyInDash";
    // Other
    public const string ShowWinnerCards = "ShowWinnerCards"; // string tableId
    public const string ReceiveNewBuyIn = "ReceiveNewBuyIn"; // string tableId, string playerId, string amount, string isAutoTop
    public const string SendPresent = "SendPresent"; // string senderIdJson, string recipientsIdsJson, string presentNameJson
    public const string SendQuickMessage = "SendQuickMessage"; // 
    public const string ReceivePrivateMessage = "ReceivePrivateMessage";
    public const string SendPrivateMessage = "SendPrivateMessage";
    public const string ReceiveNewFriendRequest = "ReceiveNewFriendRequest";
    public const string AddFriendByPersonalCode = "AddFriendByPersonalCode";
    public const string RemoveFriend = "RemoveFriend";
    public const string AcceptFriendRequest = "AcceptFriendRequest";
    public const string DeclineFriendRequest = "DeclineFriendRequest";
    public const string InviteFriendToTable = "InviteFriendToTable";
    public const string ConnectToTableById = "ConnectToTableById";
    public const string JoinFriendsTable = "JoinFriendsTable";
    public const string RemoveMessageNotification = "RemoveMessageNotification";
    public const string SendReconnectionToServer = "OnReconnected";
}

public static class Hub
{
    static public Action OnConnectedToServer;
    static public Action OnDisconnectedFromServer;
    static HubConnection hub;
    public static string uriString = "https://pokerhandprod.azurewebsites.net";
    public static string serverVersion => "20221005";
    // "http://86.57.245.73:3634";
    // "https://pokerhandwindows.azurewebsites.net/";
    // "https://testpokerhand.azurewebsites.net";

    public static ConnectionStates ConnectionState => hub == null ? ConnectionStates.Initial : hub.State;

    public static void ConnectAsync()
    {
        var request = new HTTPRequest(new Uri($"{uriString}/version"), HTTPMethods.Get, Responce);
        request.SetHeader("Access-Control-Allow-Origin", "*");
        request.Send();
        Log($"{DateTime.UtcNow} HTTP_SendGetServerVersion");
        void Responce(HTTPRequest req, HTTPResponse resp)
        {
            if (resp.StatusCode == 200 && resp.DataAsText == serverVersion)
            {
                hub = new HubConnection(new Uri($"{uriString}/game"), new JsonProtocol(new LitJsonEncoder()));
                hub.AuthenticationProvider = new BestHTTP.SignalRCore.Authentication.PokerHandAccessTokenAuthenticator(hub);
                //hub.Headers.Add("token", tokenValue);
                hub.OnConnected += (hub) => OnConnected();
                hub.OnConnected += (hub) => Log($"{DateTime.UtcNow} Connected to hub");
                hub.OnClosed += (hub) => Log($"{DateTime.UtcNow} Hub closed");
                hub.OnClosed += (hub) => OnClosed();
                hub.OnError += (hub, reason) => Log($"{DateTime.UtcNow} Error on hub: {reason}");
                hub.OnError += (hub, reason) => Reconnect();
                hub.OnReconnected += (hub) => Log($"{DateTime.UtcNow} Hub reconnected");
                hub.OnReconnected += (hub) => Client.Reconnected();
                hub.OnReconnecting += (hub, reason) => Log($"{DateTime.UtcNow} Hub reconnecting: {reason}");
                hub.OnReconnecting += (hub, reason) => Reconnect();
                hub.ReconnectPolicy = new CustomRetryPolicy();
                hub.Options.PingInterval = new TimeSpan(0, 0, 120);
                hub.Options.PingTimeoutInterval = new TimeSpan(0, 0, 150);
                hub.ConnectAsync();
            }
            else
            {
                NotificationController.ShowNotification(NotificationController.NotificationType.ServerVersionDismatch);
            }
        }
    }

    static void Reconnect()
    {
        SceneManager.LoadScene("MainMenu");
    }

    static void OnConnected()
    {
        Log($"{DateTime.UtcNow} Connected to hub");
        hub.On<string, string>("ReceiveInitialTableState", (json, avatars) => Client.ReceiveInitialTableState(json, avatars));
        hub.On<string>("ReceiveNewPlayerImage", (avatar) => Client.ReceiveNewPlayerImage(avatar));
        hub.On<string>("ReceiveTableState", json => Client.ReceiveTableState(json));
        hub.On<string>("ReceiveCurrentPlayerIdInWagering", json => Client.ReceiveCurrentPlayerId(json));
        hub.On<string>("DealCommunityCards", json => Client.ReceiveDealCommunityCards(json));
        hub.On<string>("PlayerDisconnected", json => Client.ReceivePlayerDisconnected(json));
        hub.On<string>("ReceivePlayerAction", json => Client.ReceivePlayerAction(json));
        hub.On<string>("ReceiveWinners", json => Client.ReceiveWinners(json));
        hub.On<string>("EndSitAndGoGame", json => Client.ReceiveEndSitAndGoGame(json));
        hub.On<string>("ReceiveUpdatedPot", json => Client.ReceiveUpdatedPot(json));
        hub.On<string>("PrepareForGame", json => Client.ReceivePrepareForGame(json));
        hub.On<string>("ReceivePresent", json => Client.ReceivePresent(json));
        hub.On<string>("ReceiveTotalMoney", json => Client.ReceiveTotalMoney(json));
        hub.On<string>("ErrorOnSendPresent", json => Client.ReceiveErrorOnSendPresent(json));
        hub.On<string>("ReceiveQuickMessage", json => Client.ReceiveQuickMessage(json));
        hub.On<string>("ReceivePrivateMessage", json => Client.ReceivePrivateMessage(json));
        hub.On<string>("ReceiveNewFriendRequest", json => Client.ReceiveNewFriendRequest(json));
        hub.On<string>("UpdateFriendsListOnRemove", json => Client.UpdateFriendsListOnRemove(json));
        hub.On<string>("UpdateFriendsListOnAdd", json => Client.UpdateFriendsListOnAdd(json));
        hub.On<string>("ReceiveInvitationToTableFromFriend", json => Client.ReceiveInvitationToTableFromFriend(json));
        hub.On<string>("ReceiveFriendsTableInfo", json => Client.ReceiveFriendsTableInfo(json));
        hub.On<string>("ErrorOnAddFriendByPersonalCode", json => Client.ErrorOnAddFriendByPersonalCode(json));
        hub.On<string>("ErrorOnAddFriendById", json => Client.ErrorOnAddFriendById(json));
        hub.On<string>("ErrorOnInviteFriendToTable", json => Client.ErrorOnInviteFriendToTable(json));
        hub.On<string>("ErrorOnJoinFriendsTable", json => Client.ErrorOnJoinFriendsTable(json));
        hub.On<int>("SuccessOnPayPalPurchase", json => Client.SuccessOnPayPalPurchase(json));
        hub.On<string>("ErrorOnConnectionToTableById", json => Client.ErrorOnConnectionToTableById(json));
        hub.On("SuccessOnAddFriendById", () => Client.SuccessOnAddFriendById());
        hub.On("SuccessOnAddFriendByPersonalCode", () => Client.SuccessOnAddFriendByPersonalCode());
        hub.On("SuccessOnInviteFriendToTable", () => Client.SuccessOnInviteFriendToTable());
        hub.On("OnLackOfStackMoney", () => Client.ReceiveOnLackOfStackMoney());
        hub.On("OnGameEnd", () => Client.ReceiveOnGameEnd());
        hub.On("ShowWinnerCards", () => Client.ShowWinnerCards());
        hub.On("ShowMessageOnLackOfMoneyInDash", () => Client.ShowMessageOnLackOfMoneyInDash());
        hub.On("KickPlayerFromTable", () => Client.KickPlayerFromTable());
        hub.On("ActFoldByInactivePlayer", () => Client.ActFoldByInactivePlayer());
        hub.On("OnPurchaseCancelled", () => Client.PurchaseCancelledPayPal());
        Client.OnReceiveMyPlayerProfile += Client.HTTP_GetLastDailyRewardTimeRequest;
        Client.OnReceiveMyPlayerProfile += Client.HTTP_GetLastFreeSpinTimeRequest;
        Client.OnReceiveMyPlayerProfile += Client.HTTP_GetDailyTasksRequest;
        OnConnectedToServer?.Invoke();
        Client.ReceiveConnectionID();
    }

    static void OnClosed()
    {
        Log($"{DateTime.UtcNow} Disconnected from hub");
        OnDisconnectedFromServer?.Invoke();
    }

    public static string ConnectionID => hub.NegotiationResult.ConnectionId;

    async public static void SendAsync(string target, params object[] args) => await hub.SendAsync(target, args);

#if CSHARP_7_OR_LATER
    async public static void CloseAsync() => await hub?.CloseAsync();
#endif
}