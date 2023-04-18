using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FriendInviting : MonoBehaviour
{
    [SerializeField] private Text invitingText;
    [SerializeField] private Text tableName;
    [SerializeField] private Text tableType;
    [SerializeField] private Text playerCount;
    [SerializeField] private Text notificationPanelText;
    [SerializeField] private Text acceptButtonText;
    [SerializeField] private Text refuseButtonText;
    [SerializeField] private Button declineButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button acceptButton;
    private GameMode _gameMode;

    private void Awake()
    {
        SetTexts();
        _gameMode = acceptButton.GetComponent<GameMode>();
        declineButton.onClick.AddListener(() => Destroy(gameObject));
        quitButton.onClick.AddListener(() => Destroy(gameObject));
        acceptButton.onClick.AddListener(AcceptInvite);
    }

    private void SetTexts()
    {
        acceptButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.Accept");
        refuseButtonText.text = SettingsManager.Instance.GetString("FriendsPanel.Refuse");
        notificationPanelText.text = SettingsManager.Instance.GetString("FriendsPanel.InvitationToFriend");
    }

    private void AcceptInvite()
    {
        if (StaticRuntimeSets.Items.ContainsKey("MainFriendsWindow(Clone)"))
            Destroy(StaticRuntimeSets.Items["MainFriendsWindow(Clone)"]);
        ReceiveFriendsDataFromServer._isConnectToTableById = true;
        _gameMode.ButtonClick();
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo)
            FindObjectOfType<SitNGoWindowController>().OpenWindow();
        else
            StaticRuntimeSets.GetType<WindowWithMoneyController>().OpenWindow(WindowWithMoneyController.WindowWithMoneyType.GoToTable);
        Destroy(gameObject);
    }

    public void SetTextData(string _nameOfFriend, TablesInWorlds _tableName)
    {
        TablesInWorlds tableTitle = (TablesInWorlds)((int)_tableName - 1);
        var info = Client.TablesInfo.First(t => t.Title == tableTitle);
        _gameMode.SetDataForInviting(info);
        invitingText.text = string.Format(SettingsManager.Instance.GetString("FriendsPanel.InvitingText"), _nameOfFriend);
        tableName.text = SettingsManager.Instance.GetString("FriendsPanel.TableName") + ": " + tableTitle;
        tableType.text = SettingsManager.Instance.GetString("FriendsPanel.TableType") + ": " + (GameModes)info.TableType;
        playerCount.text = SettingsManager.Instance.GetString("FriendsPanel.PlayerCount") + ": " + info.MaxPlayers;
    }
}
