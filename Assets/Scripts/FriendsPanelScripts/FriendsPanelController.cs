using System;
using UnityEngine;
using UnityEngine.UI;
using static Client;

public class FriendsPanelController : MonoBehaviour
{
    [Header("Panels"), SerializeField] private GameObject _parentCanvas;
    [SerializeField] private GameObject _invitingPanel;
    [SerializeField] private GameObject _friendsPanel;
    [Header("Buttons"), SerializeField] Button showFriendsButton;
    private GameMode _gameMode;
    public static Action<PrivateMessageDto> OnUpdateChatView;

    void Start()
    {
        _gameMode = GetComponent<GameMode>();
        showFriendsButton.onClick.AddListener(ShowFriendsPanel);
        OnReceivePrivateMessage += ReceiveMessage;
        OnReceiveInvitationToTableFromFriend += ShowInvitationPanel;
        OnReceiveFriendsTableInfo += ConnectToFriendTable;
    }

    private void OnDestroy()
    {
        OnReceiveFriendsTableInfo -= ConnectToFriendTable;
        OnReceiveInvitationToTableFromFriend -= ShowInvitationPanel;
        OnReceivePrivateMessage -= ReceiveMessage;
    }

    private void ShowFriendsPanel() => Instantiate(_friendsPanel, _parentCanvas.transform);

    private void ShowInvitationPanel(GetInvitationDto getInvitationDto)
    {
        ReceiveFriendsDataFromServer.tableId = getInvitationDto.TableId;
        var panel = Instantiate(_invitingPanel, _parentCanvas.transform);
        var component = panel.GetComponent<FriendInviting>();
        component.SetTextData(getInvitationDto.PlayerName, getInvitationDto.TableTitle);
    }

    private void ReceiveMessage(PrivateMessageDto privateMessageDto)
    {
        try
        {
            OnUpdateChatView.Invoke(privateMessageDto);
        }
        catch
        {
            ReceiveFriendsDataFromServer.allChatsDictionary[privateMessageDto.SenderId].Add(privateMessageDto);
        }
    }

    private void ConnectToFriendTable(JoinTableDto joinTableDto)
    {
        var info = TablesInfo;
        ReceiveFriendsDataFromServer.tableId = joinTableDto.TableId;
        for (int i = 0; i < info.Count; i++)
            if (info[i].Title.Equals((TablesInWorlds)((int)joinTableDto.TableTitle - 1)))
                _gameMode.SetDataForInviting(info[i]);
        Destroy(StaticRuntimeSets.Items["MainFriendsWindow(Clone)"]);
        ReceiveFriendsDataFromServer._isConnectToTableById = true;
        _gameMode.ButtonClick();
        if (_gameMode.GameModeType == GameModes.SitNGo)
            StaticRuntimeSets.GetType<SitNGoWindowController>().OpenWindow();
        else
            StaticRuntimeSets.GetType<WindowWithMoneyController>().OpenWindow(WindowWithMoneyController.WindowWithMoneyType.GoToTable);
    }
}
