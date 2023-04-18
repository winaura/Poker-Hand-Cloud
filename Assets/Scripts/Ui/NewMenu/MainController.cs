using static UnityEngine.Debug;
using static Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using PokerHand.Common.Dto;

public class MainController : MonoBehaviour
{
    private PlayerPanelController _playerPanelController;
    private WindowWithMoneyController _windowWithMoneyController;
    private SitNGoWindowController _sitNGoWindowController;
    private PlayerProfileWindowController _playerProfileWindowController;
    private SettingsController _settingsController;
    private ShopController _shopController;
    private PrivateTableEditorWindowController _privateTableEditorWindowController;
    private DailyEventsWindowController _dailyEventsWindowController;
    private DailyRewardWindowController _dailyRewardWindowController;
    private WonSitNGoWindowController _wonSitNGoWindowController;
    private LuckySpinController _luckySpinController;
    private MainLobbyController _mainLobbyController;
    private static bool firstLoad = true;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _playerPanelController = GetComponent<PlayerPanelController>();
        _windowWithMoneyController = GetComponent<WindowWithMoneyController>();
        _privateTableEditorWindowController = GetComponent<PrivateTableEditorWindowController>();
        _playerProfileWindowController = GetComponent<PlayerProfileWindowController>();
        _sitNGoWindowController = GetComponent<SitNGoWindowController>();
        _settingsController = GetComponent<SettingsController>();
        _shopController = GetComponent<ShopController>();
        _dailyEventsWindowController = GetComponent<DailyEventsWindowController>();
        _dailyRewardWindowController = GetComponent<DailyRewardWindowController>();
        _wonSitNGoWindowController = GetComponent<WonSitNGoWindowController>();
        _luckySpinController = GetComponent<LuckySpinController>();
        _mainLobbyController = GetComponent<MainLobbyController>();
    }

    private void Start()
    {
        if (firstLoad)
            firstLoad = false;
        else
            AppodealController.Instance.ShowInterstitial();
    }

    private void OnEnable()
    {
        _playerPanelController.OnInfoButtonClick += () => _playerProfileWindowController.OpenWindow();
        _playerPanelController.OnSettingButtonClick += _settingsController.OpenWindow;
        _playerPanelController.OnDailyTasksButtonClick += _dailyEventsWindowController.OpenWindow;
        _playerPanelController.OnShopButtonClick += _shopController.OpenWindow;
        _sitNGoWindowController.OnEnterButton += GoOnTable;
        _wonSitNGoWindowController.OnEnterButton += GoOnTable;
        _windowWithMoneyController.OnEnterPressed += GoOnTable;
        _privateTableEditorWindowController.OnEnterPressed += GoOnTable;
        OnPressedFoldDash += GoOnTable;
        AudioManager.Instance.PlayMusic(Clips.MenuMusic);
        SettingsManager.Instance.InitializeLanguage();
        OnRewardedTimeReceived += _dailyRewardWindowController.OpenWindow;
        OnReceiveMyPlayerProfile += CheckNewPlayer;
        if (PlayerProfileData.Instance != null)
            HTTP_UpdateTotalMoneyAndExperienceRequest();
        DeleteUserProfile.Instance.DeleteUser += DeleteUserNoAds;
    }
    private void OnDisable()
    {
        _playerPanelController.OnInfoButtonClick -= () => _playerProfileWindowController.OpenWindow();
        _playerPanelController.OnSettingButtonClick -= _settingsController.OpenWindow;
        _playerPanelController.OnDailyTasksButtonClick -= _dailyEventsWindowController.OpenWindow;
        _playerPanelController.OnShopButtonClick -= _shopController.OpenWindow;
        _sitNGoWindowController.OnEnterButton -= GoOnTable;
        _wonSitNGoWindowController.OnEnterButton -= GoOnTable;
        _windowWithMoneyController.OnEnterPressed -= GoOnTable;
        _privateTableEditorWindowController.OnEnterPressed -= GoOnTable;
        OnPressedFoldDash -= GoOnTable;
        OnRewardedTimeReceived -= _dailyRewardWindowController.OpenWindow;
        OnReceiveMyPlayerProfile -= CheckNewPlayer;
        DeleteUserProfile.Instance.DeleteUser -= DeleteUserNoAds;
    }
    private void DeleteUserNoAds()
    {
        firstLoad = true;
    }

    public void OpenWindowWithMoney() => _windowWithMoneyController.OpenWindow(WindowWithMoneyController.WindowWithMoneyType.GoToTable);

    private void CheckNewPlayer(PlayerProfileDto playerProfileDto)
    {
        if (DateTime.UtcNow.Subtract(playerProfileDto.RegistrationDate).TotalSeconds < 100 && !PlayerPrefs.HasKey("NewPlayer"))
        {
            PlayerPrefs.SetInt("NewPlayer", 1);
            DailyRewardWindowView.CreateWindow
            (
                SettingsManager.Instance.GetString("NewPlayer.Welcome"),
                SettingsManager.Instance.GetString("NewPlayer.Reward"),
                SettingsManager.Instance.GetString("DailyReward.Claim"),
                "20K",
                sortingOrder: 1
            );
        }
        ReceiveFriendsDataFromServer.PersonalCode = playerProfileDto.PersonalCode;
    }

    private void GoOnTable()
    {
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo)
        {
            if (PlayerProfileData.Instance.playerGetMoney < GameModeSettings.Instance.minMoneyGet ||
                PlayerProfileData.Instance.playerGetMoney > PlayerProfileData.Instance.TotalMoney)
            {
                _wonSitNGoWindowController.Close();
                _sitNGoWindowController.OpenWindow();
                return;
            }
            PlayerProfileData.Instance.playerGetMoney = GameModeSettings.Instance.sitNGoMoneyGet;
        }
        OnReceiveTableInfo += OnGetTableInfo;
        HTTP_SendTableInfoRequest(GameModeSettings.Instance.TablesInWorlds);
    }

    private void LoadGame()
    {
        OnFirstTableDataReceived -= LoadGame;
        SceneManager.LoadScene(GameConstants.SceneGame);
    }

    private void OnGetTableInfo()
    {
        var gameSettings = GameModeSettings.Instance;
        gameSettings.bigBlind = TableInfo.BigBlind;
        gameSettings.smallBlind = TableInfo.SmallBlind;
        gameSettings.countPeople = TableInfo.MaxPlayers;
        OnReceiveTableInfo -= OnGetTableInfo;
        OnFirstTableDataReceived += LoadGame;
        if (!ReceiveFriendsDataFromServer._isConnectToTableById)
        {
            SendConnectToTableRequest(GameModeSettings.Instance.TablesInWorlds, PlayerProfileData.Instance.playerGetMoney);
            Log($"{DateTime.UtcNow} SendConnectToTableRequest");
        }
        else
        {
            SendConnectToTableById(ReceiveFriendsDataFromServer.tableId, PlayerProfileData.Instance.Id, PlayerProfileData.Instance.playerGetMoney);
            ReceiveFriendsDataFromServer._isConnectToTableById = false;
            Log($"{DateTime.UtcNow} SendConnectToTableById");
        }
    }
}