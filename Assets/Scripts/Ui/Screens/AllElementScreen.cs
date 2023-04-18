using PokerHand.Common.Dto;
using PokerHand.Common.Helpers.QuickChat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Client;

public class AllElementScreen : UIScreen
{
    [Header("Other"), SerializeField] private Button _closeAllWindows;
    [SerializeField] private TableManager _tableManager;
    [SerializeField] private PlayerProfileWindowController profileWindowController;
    [SerializeField] private Text _prizeText;
    [SerializeField] private Text _dealerText;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _changeTableButton;
    [SerializeField] private Button _chatButton;
    [SerializeField] private Color _grayColor;
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _blueColor;
    [SerializeField] private ShopController _shopController;
    [SerializeField] private GameObject giftTablePrefab;
    [SerializeField] private Transform _giftsParentTransform;
    [Header("Players"), SerializeField] private GameObject _fivePlayerObject;
    [SerializeField] private GameObject _eightPlayerObject;
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private Text _waitText;
    [SerializeField] private Text _comboTextFivePeople;
    [SerializeField] private Text _comboTextEightPeople;
    [Header("Five Players"), SerializeField] private GameObject[] _fivePlayersObjects;
    [SerializeField] private List<GameObject> _fiveTimerImagePlayer;
    [SerializeField] private List<Text> _fiveNamePlayer;
    [SerializeField] private List<RawImage> _fiveAvatarPlayer;
    [SerializeField] private EmojiImageObj[] _fiveEmojiObjPlayer;
    [SerializeField] private List<Image> _fiveGlowOutlinePlayer;
    [SerializeField] private List<Image> _fiveBankBackgroundImage;
    [SerializeField] private List<Text> _fiveMoneyPlayer;
    [SerializeField] private List<GameObject> _fivePlayersBet;
    [SerializeField] private List<Text> _fivePlayersBetText;
    [SerializeField] private List<Image> _fivePlayersBetImages;
    [SerializeField] private List<Text> _fivePlayersBlindText;
    [SerializeField] private List<AudioSource> _fivePlayersAudioSource;
    [SerializeField] private List<GameObject> _fivePlayersChip;
    [SerializeField] private List<Text> _fivePlayersWinnerText;
    [SerializeField] private Transform[] _fivePlayersPresentPoint;
    [SerializeField] private Transform[] _fivePlayersChatMessagePoint;
    [SerializeField] private PlayerThinkTimer[] _fivePlayersThinkTimer;
    [Header("Eight Players"), SerializeField] private GameObject[] _eightPlayersObjects;
    [SerializeField] private List<GameObject> _eightTimerImagePlayer;
    [SerializeField] private List<Text> _eightNamePlayer;
    [SerializeField] private List<RawImage> _eightAvatarPlayer;
    [SerializeField] private EmojiImageObj[] _eightEmojiObjPlayer;
    [SerializeField] private List<Image> _eightGlowOutlinePlayer;
    [SerializeField] private List<Image> _eightBankBackgroundImage;
    [SerializeField] private List<Text> _eightMoneyPlayer;
    [SerializeField] private List<GameObject> _eightPlayersBet;
    [SerializeField] private List<Text> _eightPlayersBetText;
    [SerializeField] private List<Image> _eightPlayersBetImages;
    [SerializeField] private List<Text> _eightPlayersBlindText;
    [SerializeField] private List<AudioSource> _eightPlayersAudioSource;
    [SerializeField] private List<GameObject> _eightPlayersChip;
    [SerializeField] private List<Text> _eightPlayersWinnerText;
    [SerializeField] private Transform[] _eightPlayersPresentPoint;
    [SerializeField] private Transform[] _eightPlayersChatMessagePoint;
    [SerializeField] private PlayerThinkTimer[] _eightPlayersThinkTimer;
    [Header("Localization Buttons"), SerializeField] private List<Button> _languageButtons;
    [Header("HiddenWindow"), SerializeField] private GameObject _hiddenMenu;
    [SerializeField] private Button _toLobbyButton;
    [SerializeField] private Button _leaveTableButton;
    [SerializeField] private Button _switchTableButton;
    [SerializeField] private Image _nonInteractableImage;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Text _exitToLobbyText;
    [SerializeField] private Text _leaveTableText;
    [SerializeField] private Text _switchTableText;
    [SerializeField] private Text _settingsText;
    [Header("PlayerInfoPanel"), SerializeField] private Text _playerChips;
    [Header("SettingsWindow"), SerializeField] private SettingsController settingsController;
    [Header("ConfirmButton"), SerializeField] private GameObject _confirmWindow;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private Text _areYouSureButtonText;
    [SerializeField] private Text _yesButtonText;
    [SerializeField] private Text _noButtonText;
    [SerializeField] private Text _winAmountText;
    [Header("WindowWithMoneyAfterSwitch"), SerializeField] private WindowWithMoneyController _windowWithMoneyController;
    [Header("Gifts"), SerializeField] private GiftsWindowController _giftWindowController;
    [Header("Chat"), SerializeField] private ChatWindowController _chatWindowController;
    [SerializeField] private GameObject messageObjectPrefab;
    [SerializeField] private Transform messageObjectParent;
    private GameObject _currentGameObjectImage;
    private Image _currentImage;
    private int _countDownTimer;
    private int _currentSliderMoney;
    private int _currentSliderAmount;
    private bool _isSetData;
    private PlayerData playerData = null;
    private GameModeSettings gameManager = null;
    private Language _language;
    private Vector3[] _fivePlayersChipsStartPositions = new Vector3[5];
    private Vector3[] _eightPlayersChipsStartPositions = new Vector3[8];
    private GameObject[] _giftsObj = new GameObject[8];
    private Coroutine timerCoroutine;
    private Coroutine windowWithMoneyRoutine;
    int giftSelectedPlayerIndex = 0;

    private void Start()
    {
        #region Buttons
        _menuButton.onClick.AddListener(ChangeStateHiddenWindow);
        _chatButton.onClick.AddListener(OpenChat);
        _settingsButton.onClick.AddListener(ChangeStateSettingsWindow);
        _noButton.onClick.AddListener(CloseConfirmWindow);
        _toLobbyButton.onClick.AddListener(() => OpenConfirmWindow(GoToLobby));
        if (GameModeSettings.Instance.gameMode != GameModes.Dash)
            _leaveTableButton.onClick.AddListener(() => OpenConfirmWindow(GoToWorld));
        else
            _leaveTableButton.onClick.AddListener(() => OpenConfirmWindow(GoToLobby));
        _closeAllWindows.onClick.AddListener(CloseAllWindows);
        _switchTableButton.onClick.AddListener(OpenWindowWithMoneyAfterSwitch);
        #endregion
        #region DeactiveModalWindows
        _hiddenMenu.SetActive(false);
        _closeAllWindows.gameObject.SetActive(false);
        #endregion
        OnReceiveMyPlayerProfile += UpdatePlayerName;
        _tableManager.OnPlayerShowThinkTimer += ShowThinkTimer;
        _windowWithMoneyController.OnEnterPressed += ChangeTable;
        OnPressedFoldDash+= ChangeTable;
        _giftWindowController.OnGiftToAll += SendGiftsToAll;
        _giftWindowController.OnGiftToPlayer += SendGiftToPlayer;
        if (PlayerData.Instance != null)
            playerData = PlayerData.Instance;
        if (GameModeSettings.Instance != null)
            gameManager = GameModeSettings.Instance;
        if(GameModeSettings.Instance.gameMode == GameModes.SitNGo)
            TurnOffSwithTableButton();
        _language = PlayerData.Instance.language;
        MP_SetAvatar();
        CheckPlayerCount(gameManager);
        _comboTextEightPeople.gameObject.SetActive(PlayerData.Instance.isHintsOn);
        _comboTextFivePeople.gameObject.SetActive(PlayerData.Instance.isHintsOn);
        SettingsManager.Instance.UpdateTextsEvent += UpdateTexts;       
        UpdateTexts();
        UpdateMoney();
        for (var i = 0; i < 5; i++)
            _fivePlayersChipsStartPositions[i] = _fivePlayersChip[i].transform.position;        
        for (var i = 0; i < 8; i++)
            _eightPlayersChipsStartPositions[i] = _eightPlayersChip[i].transform.position;
        OnReceivePlayerAction += HideCurrentTimer;
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= UpdateTexts;
        _windowWithMoneyController.OnEnterPressed -= ChangeTable;
        OnPressedFoldDash -= ChangeTable;
        OnReceiveMyProfileImage -= MP_SetAvatar;
        OnReceiveMyPlayerProfile -= UpdatePlayerName;
        OnReceivePlayerAction -= HideCurrentTimer;
        _tableManager.OnPlayerShowThinkTimer -= ShowThinkTimer;
    }

    private void ShowThinkTimer(int playerIndex, bool isShow)
    {
        if (_tableManager.playersOnTable.Count == 8)
            _eightPlayersThinkTimer[playerIndex].gameObject.SetActive(isShow);
        else
            _fivePlayersThinkTimer[playerIndex].gameObject.SetActive(isShow);
    }
    protected override void UpdateScreen(ScreenData dat)
    {
        if (!_isSetData)
        {
            _isSetData = true;
            dat.OnUpdatePlayersMoneyUIAction += UpdatePlayersMoney;
            dat.OnUpdatePlayerMoneyUIAction += UpdatePlayerMoney;
            dat.OnUpdatePlayersMoneyUIAction += UpdatePlayersName;
            dat.OnMyPlayerFinishedSitNGo += GoToWorld;
            dat.OnIndexCurrentPlayerChanged += UpdatePlayerInfo;
            dat.OnPrizeChanged += PrizeChanged;
            dat.OnCountdownChanged += ChangeCountdown;
            dat.OnSetPlayerWinnerText += SetWinnerPlayerText;
            dat.OnPlayerCombinChanged += SetPlayerCombinText;
            dat.OnPlayerBetChanged += SetPlayerBet;
            dat.OnBlindPriceChanged += SetBlindPrice;
            dat.OnResetBlinds += ResetBlindsPrice;
            dat.OnResetPlayersBet += ResetPlayersBetPrice;
            dat.OnResetPlayerBet += ResetPlayerBetPrice;
            dat.OnResetPlayerCountdown += ResetPlayerCountdown;
            dat.OnResetPlayerGift += ResetPlayerGift;
            dat.OnResetGame += () => OnReset();
            dat.OnResetWaitText += () => SetWaitText(false, string.Empty);
            dat.OnWaitingForPlayers += () => SetWaitText(true, GameConstants.WaitingForPlayersMessage);
            dat.OnWaitingForNewGame += () => SetWaitText(true, GameConstants.WaitingNewGameMessage);
            dat.OnRestoreMoney += UpdateMoney;
            dat.OnMyPlayerNotEnoughtMoney += MP_OpenWindowWithMoney;
            dat.OnGlowPlayer += GlowWinner;
            dat.OnHighLightBankImage += HighLightBankImage;
            dat.OnSetAvatar += MP_SetAvatar;
            OnReceiveMyProfileImage += MP_SetAvatar;
            dat.OnSetPresent += MP_SetPresent;
            dat.OnRemovePresent += MP_RemovePresent;
            dat.OnSetEmoji += MP_SetEmoji;
            dat.OnSetPresentImmediately += MP_SetPresentImmediately;
            dat.OnSetPlayerState += MP_SetPlayerState;
            dat.OnSetNewTotalMoney += SetTotalMoney;
            dat.OnSetQuickMessage += SetQuickMessage;
        }
    }

    private void OnReset()
    {
        SetWaitText(false, string.Empty);

        if (_tableManager.playersOnTable.Count == 5)
            for (int i = 0; i < _fivePlayersChip.Count; i++)
                _fivePlayersChip[i].transform.position = _fivePlayersChipsStartPositions[i];
        else
            for (int i = 0; i < _eightPlayersChip.Count; i++)
                _eightPlayersChip[i].transform.position = _eightPlayersChipsStartPositions[i];
    }

    private void TurnOffSwithTableButton()
    {
        _nonInteractableImage.gameObject.SetActive(true);
        _switchTableButton.interactable = false;
    }
    private void SetPlayerBet((long, int) obj)
    {
        var newValue = obj.Item1 > 0 ? $"{obj.Item1.IntoCluttered()}" : string.Empty;
        var newState = obj.Item1 > 0;
        if (_tableManager.playersOnTable.Count == 8)
        {
            _eightPlayersBetText[obj.Item2].text = "$" + newValue;
            _eightPlayersBet[obj.Item2].SetActive(newState);
            _eightPlayersBetImages[obj.Item2].enabled = newState;
            _eightPlayersChip[obj.Item2].gameObject.SetActive(newState);
        }
        else
        {
            _fivePlayersBetText[obj.Item2].text = "$" + newValue;
            _fivePlayersBet[obj.Item2].SetActive(newState);
            _fivePlayersBetImages[obj.Item2].enabled = newState;
            _fivePlayersChip[obj.Item2].gameObject.SetActive(newState);
        }
    }

    private void SetBlindPrice((int, int) obj)
    {
        if (_tableManager.playersOnTable.Count == 8)
        {
            _eightPlayersBlindText[obj.Item2].gameObject.SetActive(true);
            _eightPlayersBlindText[obj.Item2].text = $"{obj.Item1.IntoCluttered()}";
        }
        else
        {
            _fivePlayersBlindText[obj.Item2].gameObject.SetActive(true);
            _fivePlayersBlindText[obj.Item2].text = $"{obj.Item1.IntoCluttered()}";
        }
    }

    private void ResetBlindsPrice()
    {
        if (_tableManager.playersOnTable.Count == 8)
            for (int i = 0; i < _eightPlayersBlindText.Count; i++)
                _eightPlayersBlindText[i].gameObject.SetActive(false);
        else
            for (int i = 0; i < _fivePlayersBlindText.Count; i++)
                _fivePlayersBlindText[i].gameObject.SetActive(false);
    }
    private void UpdateMoney() => _playerChips.text = $"${PlayerProfileData.Instance.TotalMoney.IntoCluttered()}";

    private void SetTotalMoney(long newValue) => _playerChips.text = $"${newValue.IntoCluttered()}";

    private void SetQuickMessage(QuickMessageDto messageDto)
    {
        int index = messageDto.SenderIndex.ToLocalIndex();
        Vector3 pos;
        if (_tableManager.playersOnTable.Count == 5)
            pos = _fivePlayersChatMessagePoint[index].position;
        else
            pos = _eightPlayersChatMessagePoint[index].position;
        var messageObj = Instantiate(messageObjectPrefab, pos, Quaternion.identity, messageObjectParent);
        messageObj.GetComponent<QuickMessageObj>().SetText(messageDto.Message);
    }

    private void ResetPlayersBetPrice() => StartCoroutine(ChipAnimation());

    private void ResetPlayerBetPrice(int index) => StartCoroutine(ChipAnimation(index));

    private void ResetPlayerCountdown(int index)
    {
        if (_tableManager.playersOnTable.Count == 8)
        {
            _eightTimerImagePlayer[index].gameObject.SetActive(false);
            _currentGameObjectImage = _eightTimerImagePlayer[0];
        }
        else
        {
            _fiveTimerImagePlayer[index].gameObject.SetActive(false);
            _currentGameObjectImage = _fiveTimerImagePlayer[0];
        }
    }
    
    private void ResetPlayerGift(int index)
    {
        if (_giftsObj[index] != null)
            Destroy(_giftsObj[index]);
    }

    private void UpdatePlayerInfo(int oldObj, int newObj)
    {
        ChangeImage(oldObj, newObj);
        UpdatePlayersName();
        UpdatePlayersMoney();
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(Timer());
    }

    private void HideCurrentTimer()
    {
        if (_currentGameObjectImage != null)
            _currentGameObjectImage.SetActive(false);
    }

    private void UpdateTexts() => _dealerText.text = SettingsManager.Instance.GetString("OnTable.Dealer");

    private void UpdatePlayersName()
    {
        for (int i = 0; i < _tableManager.playersOnTable.Count; i++)
            if (_tableManager.playersOnTable.Count == 8)
            {
                if (_tableManager.playersOnTable[i].activeSelf)
                    _eightNamePlayer[i].text = _tableManager.GetPlayersName(i);
                else
                    _eightNamePlayer[i].text = string.Empty;
            }
            else
            {
                if (_tableManager.playersOnTable[i].activeSelf)
                    _fiveNamePlayer[i].text = _tableManager.GetPlayersName(i);
                else
                    _fiveNamePlayer[i].text = string.Empty;
            }
    }

    private void MP_SetPlayerState(int index, bool newState)
    {
        if (_tableManager.playersOnTable.Count == 8)
            _eightPlayersObjects[index].SetActive(newState);
        else
            _fivePlayersObjects[index].SetActive(newState);
    }
    
    private void MP_SetAvatar(int index)
    {
        if (_tableManager.playersOnTable.Count == 8)
            _eightAvatarPlayer[index].texture = ProfileImages[index];
        else
            _fiveAvatarPlayer[index].texture = ProfileImages[index];
    }

    private void MP_SetAvatar()
    {
        if (_tableManager.playersOnTable.Count == 8)
            for (int i = 0; i < 8; i++)
                _eightAvatarPlayer[i].texture = ProfileImages[i];
        else
            for (int i = 0; i < 5; i++)
                _fiveAvatarPlayer[i].texture = ProfileImages[i];
    }

    private void MP_SetPresent(int from, int to, Sprite sprite, float sizeMod, Vector3 centerOffset)
    {
        var targetFrom = _tableManager.playersOnTable.Count == 5
            ? _fivePlayersPresentPoint[from]
            : _eightPlayersPresentPoint[from];

        var targetTo = _tableManager.playersOnTable.Count == 5
            ? _fivePlayersPresentPoint[to]
            : _eightPlayersPresentPoint[to];
        var giftObj = Instantiate(giftTablePrefab, targetFrom.position, Quaternion.identity, _giftsParentTransform);
        if (_giftsObj[to] != null)
            Destroy(_giftsObj[to]);
        _giftsObj[to] = giftObj;
        var giftImage = giftObj.GetComponent<Image>();
        giftImage.sprite = sprite;
        if (sizeMod!=1)
            giftImage.transform.localScale *= sizeMod;
        RectTransform giftRectTransform = giftObj.GetComponent<RectTransform>();
        LeanTween.move(giftObj, giftRectTransform.TransformPoint(giftRectTransform.InverseTransformPoint(targetTo.position) + centerOffset), GameConstants.GiftsFlightTime);
    }

    private void MP_RemovePresent(int playerIndex)
    {
        if (_giftsObj[playerIndex] != null)
        {
            Destroy(_giftsObj[playerIndex]);
            _giftsObj[playerIndex] = null;
        }
    }

    private void MP_SetEmoji(int index, QuickMessage emoji)
    {
        var targetAvatar = _tableManager.playersOnTable.Count == 5
            ? _fiveAvatarPlayer[index]
            : _eightAvatarPlayer[index];

        var targetEmojiObj = _tableManager.playersOnTable.Count == 5
            ? _fiveEmojiObjPlayer[index]
            : _eightEmojiObjPlayer[index];
        targetEmojiObj.RunWith(emoji);
    }
    
    private void MP_SetPresentImmediately(int index, Sprite sprite)
    {
        var targetTransform = _tableManager.playersOnTable.Count == 5
            ? _fivePlayersPresentPoint[index]
            : _eightPlayersPresentPoint[index];
        var giftObj = Instantiate(giftTablePrefab, targetTransform);
        ResetPlayerGift(index);
        _giftsObj[index] = giftObj;
        giftObj.GetComponent<Image>().sprite = sprite;
    }

    private void UpdatePlayersMoney()
    {
        for (int i = 0; i < _tableManager.playersOnTable.Count; i++)
            if (_tableManager.playersOnTable.Count == 8)
            {
                if (_tableManager.playersOnTable[i].activeSelf)
                    _eightMoneyPlayer[i].text = $"${_tableManager.GetPlayersMoney(i).IntoCluttered()}";
                else
                    _eightMoneyPlayer[i].text = string.Empty;
            }
            else
            {
                if (_tableManager.playersOnTable[i].activeSelf)
                    _fiveMoneyPlayer[i].text = $"${_tableManager.GetPlayersMoney(i).IntoCluttered()}";
                else
                    _fiveMoneyPlayer[i].text = string.Empty;
            }
    }

    private void UpdatePlayerMoney(int index)
    {
        if (_tableManager.playersOnTable[index] == null)
            return;
        if (_tableManager.playersOnTable.Count == 8)
        {
            if (_tableManager.playersOnTable[index].activeSelf)
                _eightMoneyPlayer[index].text = $"${_tableManager.GetPlayersMoney(index).IntoCluttered()}";
            else
                _eightMoneyPlayer[index].text = string.Empty;
        }
        else
        {
            if (_tableManager.playersOnTable[index].activeSelf)
                _fiveMoneyPlayer[index].text = $"${_tableManager.GetPlayersMoney(index).IntoCluttered()}";
            else
                _fiveMoneyPlayer[index].text = string.Empty;
        }
    }

    private void OpenConfirmWindow(UnityEngine.Events.UnityAction action)
    {
        _areYouSureButtonText.text = SettingsManager.Instance.GetString("ConfirmWindow.AreYouSure");
        _yesButtonText.text = SettingsManager.Instance.GetString("ConfirmWindow.Yes");
        _noButtonText.text = SettingsManager.Instance.GetString("ConfirmWindow.No");
        var winAmount = _tableManager.MP_GetCurrentWinMoneyOnThisGame();
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo && PlayerProfileData.Instance.SitNGoStarted)
        {
            _winAmountText.text = SettingsManager.Instance.GetString("ConfirmWindow.SitNGo");
        }
        else if (winAmount > 0)
        {
            _winAmountText.text = $"{SettingsManager.Instance.GetString("ConfirmWindow.WinMoney")} ${winAmount.IntoCluttered()}";
        }
        _yesButton.onClick.AddListener(action);
        _confirmWindow.SetActive(true);
    }

    private void CloseConfirmWindow()
    {
        _yesButton.onClick.RemoveAllListeners();
        _confirmWindow.SetActive(false);
    }

    public void MP_OpenWindowWithMoney()
    {
        _windowWithMoneyController.OpenWindow(WindowWithMoneyController.WindowWithMoneyType.ContinueInTable);
        if (GameModeSettings.Instance.gameMode != GameModes.Dash)
            _windowWithMoneyController.SetSliderValue(MyPlayer.CurrentBuyIn > PlayerProfileData.Instance.TotalMoney + MyPlayer.StackMoney
                ? PlayerProfileData.Instance.TotalMoney + MyPlayer.StackMoney
                : MyPlayer.CurrentBuyIn);
    }

    private string GetDescriptionForWindowWithMoney()
    {
        if (MyPlayer.TotalMoney < GameModeSettings.Instance.bigBlind)
            return SettingsManager.Instance.GetString("WindowWithMoney.NotEnoughMoney");
        if (GameModeSettings.Instance.minMoneyGet == GameModeSettings.Instance.maxMoneyGet)
            return SettingsManager.Instance.GetString("WindowWithMoney.FixBlindTake");
        return SettingsManager.Instance.GetString("WindowWithMoney.HowMuchTake");
    }

    private void ChangeImage(int oldObj, int newObj)
    {
        if (_tableManager.playersOnTable.Count == 8)
        {
            _eightTimerImagePlayer[oldObj].gameObject.SetActive(false);
            _currentGameObjectImage = _eightTimerImagePlayer[newObj];
        }
        else
        {
            _fiveTimerImagePlayer[oldObj].gameObject.SetActive(false);
            _currentGameObjectImage = _fiveTimerImagePlayer[newObj];
        }
        _currentGameObjectImage.SetActive(true);
        _currentImage = _currentGameObjectImage.GetComponent<Image>();
    }

    private void GlowWinner(int obj, bool state)
    {
        if (_tableManager.playersOnTable.Count == 8)
            _eightGlowOutlinePlayer[obj].gameObject.SetActive(state);
        else
            _fiveGlowOutlinePlayer[obj].gameObject.SetActive(state);
    }

    public void SetWaitText(bool state, string textKey)
    {
        _messagePanel.SetActive(state);
        _waitText.text = textKey == string.Empty
                ? string.Empty
                : SettingsManager.Instance.GetString(textKey);
        if (!state && GameModeSettings.Instance.gameMode == GameModes.SitNGo)
            PlayerProfileData.Instance.SitNGoStarted = true;
    }

    private void CloseAllWindows()
    {
        _closeAllWindows.gameObject.SetActive(false);
        _hiddenMenu.SetActive(false);
        settingsController.CloseWindow();
    }

    private void ChangeStateHiddenWindow()
    {
        UpdateHiddenMenuTexts();
        _closeAllWindows.gameObject.SetActive(!_closeAllWindows.gameObject.activeSelf);
        _hiddenMenu.SetActive(!_hiddenMenu.activeSelf);
    }

    private void OpenChat() => _chatWindowController.Open();

    private void SetPlayerCombinText(string value)
    {
        if (_tableManager.playersOnTable.Count == 8)
        {
            if (_fivePlayersWinnerText[0].text == string.Empty)
                _comboTextEightPeople.text = value;
        }
        else
        {
            if (_eightPlayersWinnerText[0].text == string.Empty)
                _comboTextFivePeople.text = value;
        }
    }

    private void CheckPlayerCount(GameModeSettings gameModeSettings)
    {
        if (gameModeSettings != null)
        {
            if (gameModeSettings.countPeople == 8)
                _eightPlayerObject.SetActive(true);
            else
                _fivePlayerObject.SetActive(true);
        }
        else
        {
            if (_tableManager.playersOnTable.Count == 8)
                _eightPlayerObject.SetActive(true);
            else
                _fivePlayerObject.SetActive(true);
        }
    }

    public void DeactiveTimer() => _currentImage?.gameObject.SetActive(false);

    private void PrizeChanged(long obj) => _prizeText.text = $"${obj.IntoClutteredForTablePrize()}";

    private void ChangeCountdown(int obj) => _countDownTimer = obj;

    private void SetWinnerPlayerText(int playerIndex, string value)
    {
        var resultString = string.Empty;
        if (value != string.Empty)
        {
            var spaceIndex = value.IndexOf(' ');
            if (spaceIndex > 0)
                resultString = string.Format($"{value.Substring(0, spaceIndex)}\n{value.Substring(spaceIndex + 1)}");
            else
                resultString = value;
        }
        if (GameModeSettings.Instance.countPeople == 5)
        {
            if (playerIndex == 0 && resultString != string.Empty)
                _comboTextFivePeople.text = string.Empty;
            _fivePlayersWinnerText[playerIndex].text = resultString;
        }
        else
        {
            if (playerIndex == 0 && resultString != string.Empty)
                _comboTextEightPeople.text = string.Empty;
            _eightPlayersWinnerText[playerIndex].text = resultString;
        }
    }

    private void ChangeStateSettingsWindow()
    {
        settingsController.OpenWindow();
    }

    private void UpdatePlayerName(PlayerProfileDto playerDto)
    {
        if (GameModeSettings.Instance.countPeople == 5)
            _fiveNamePlayer[0].text = playerDto.UserName;
        else
            _eightNamePlayer[0].text = playerDto.UserName;
    }

    private void UpdateHiddenMenuTexts()
    {
        _exitToLobbyText.text = SettingsManager.Instance.GetString("HiddenMenu.ExitToLobby");
        _leaveTableText.text = SettingsManager.Instance.GetString("HiddenMenu.LeaveTable");
        _switchTableText.text = SettingsManager.Instance.GetString("HiddenMenu.SwitchTable");
        _settingsText.text = SettingsManager.Instance.GetString("HiddenMenu.Settings");
    }
    private void GoToWorld() => _tableManager.LeaveTable();

    private void GoToLobby()
    {
        if (GameModeSettings.Instance != null)
            GameModeSettings.Instance.world = Worlds.None;
        GoToWorld();
    }

    private void OpenWindowWithMoneyAfterSwitch()
    {
        _windowWithMoneyController.OpenWindow(WindowWithMoneyController.WindowWithMoneyType.GoToTable);
        if (GameModeSettings.Instance.gameMode != GameModes.Dash)
            _windowWithMoneyController.SetSliderValue(MyPlayer.CurrentBuyIn > PlayerProfileData.Instance.TotalMoney + MyPlayer.StackMoney
                ? PlayerProfileData.Instance.TotalMoney + MyPlayer.StackMoney
                : MyPlayer.CurrentBuyIn);
    }

    private void ChangeTable()
    {
        for (var i = 1; i < ProfileImages.Length; ++i)
            ProfileImages[i] = null;
        SendSwitchTableRequest(GameModeSettings.Instance.TablesInWorlds, PlayerProfileData.Instance.playerGetMoney);
    }

    private IEnumerator ChipAnimation()
    {
        if (_tableManager.playersOnTable.Count == 8)
            foreach(var p in Client.TableData.ActivePlayers)
            {
                var i = p.LocalIndex;
                LeanTween.moveX(_eightPlayersChip[i].gameObject, 0f, 0.2f);
                LeanTween.moveY(_eightPlayersChip[i].gameObject, 2f, 0.2f);
                if (_eightPlayersBetText[i].IsActive())
                    AudioManager.Instance.PlaySound(Clips.ChipsIntoBank, _eightPlayersAudioSource[i]);
                _eightPlayersBetText[i].text = string.Empty;
                _eightPlayersBetImages[i].enabled = false;
                yield return GameConstants.WaitSeconds_03;
                _eightPlayersBet[i].SetActive(false);
                _eightPlayersChip[i].gameObject.SetActive(false);
                _eightPlayersChip[i].transform.position = _eightPlayersChipsStartPositions[i];
            }
        else
            foreach (var p in Client.TableData.ActivePlayers)
            {
                var i = p.LocalIndex;
                LeanTween.moveX(_fivePlayersChip[i].gameObject, 0f, 0.2f);
                LeanTween.moveY(_fivePlayersChip[i].gameObject, 2f, 0.2f);
                if (_fivePlayersBetText[i].IsActive())
                    AudioManager.Instance.PlaySound(Clips.ChipsIntoBank, _fivePlayersAudioSource[i]);
                _fivePlayersBetText[i].text = string.Empty;
                _fivePlayersBetImages[i].enabled = false;
                yield return GameConstants.WaitSeconds_03;
                _fivePlayersBet[i].SetActive(false);
                _fivePlayersChip[i].gameObject.SetActive(false);
                _fivePlayersChip[i].transform.position = _fivePlayersChipsStartPositions[i];
            }
    }

    private IEnumerator ChipAnimation(int index)
    {
        if (_tableManager.playersOnTable.Count == 8)
        {
            var pos = _eightPlayersChip[index].transform.position;
            LeanTween.moveX(_eightPlayersChip[index].gameObject, 0f, 0.2f);
            LeanTween.moveY(_eightPlayersChip[index].gameObject, 2f, 0.2f);
            if (_eightPlayersBetText[index].IsActive())
                AudioManager.Instance.PlaySound(Clips.ChipsIntoBank, _eightPlayersAudioSource[index]);
            _eightPlayersBetText[index].text = string.Empty;
            _eightPlayersBetImages[index].enabled = false;
            yield return GameConstants.WaitSeconds_03;
            _eightPlayersBet[index].SetActive(false);
            _eightPlayersChip[index].gameObject.SetActive(false);
            _eightPlayersChip[index].transform.position = pos;
        }
        else
        {
            var pos = _fivePlayersChip[index].transform.position;
            LeanTween.moveX(_fivePlayersChip[index].gameObject, 0f, 0.2f);
            LeanTween.moveY(_fivePlayersChip[index].gameObject, 2f, 0.2f);
            if (_fivePlayersBetText[index].IsActive())
                AudioManager.Instance.PlaySound(Clips.ChipsIntoBank, _fivePlayersAudioSource[index]);
            _fivePlayersBetText[index].text = string.Empty;
            _fivePlayersBetImages[index].enabled = false;
            yield return GameConstants.WaitSeconds_03;
            _fivePlayersBet[index].SetActive(false);
            _fivePlayersChip[index].gameObject.SetActive(false);
            _fivePlayersChip[index].transform.position = pos;
        }
    }

    private void HighLightBankImage(int index, Color color)
    {
        var resultColor = _grayColor;
        if (color == Color.gray)
            resultColor = _grayColor;
        else if (color == Color.red)
            resultColor = _redColor;
        else if (color == Color.blue)
            resultColor = _blueColor;
        if (_tableManager.playersOnTable.Count == 8)
            _eightBankBackgroundImage[index].color = resultColor;
        else
            _fiveBankBackgroundImage[index].color = resultColor;
    }

    private IEnumerator Timer()
    {
        _currentImage.fillAmount = 1;
        while (_currentImage.fillAmount >= 0.01f)
        {
            yield return GameConstants.WaitSeconds_0_02;
            _currentImage.fillAmount = Mathf.Clamp01(_currentImage.fillAmount - (1f / _countDownTimer * 0.033f));
        }
    }

    public void OnPlayerAvatarClick(int localIndex)
    {
        if (localIndex == 0)
        {
            profileWindowController.OpenWindow();
            return;
        }
        AnotherPlayerProfileController.Instance.SetPickedTexture(ProfileImages[localIndex]);
        var guid = GetPlayerByLocalIndex(localIndex).Id;
        HTTP_SendGetPlayerProfile(guid);
    }

    public void OnGiftButtonClick(int playerIndex)
    {
        giftSelectedPlayerIndex = playerIndex;
        _giftWindowController.Open();
    }

    private void SendGiftToPlayer(PresentInfoDto presentInfo)
    {
        var guidPlayer = GetPlayerByLocalIndex(giftSelectedPlayerIndex).Id;
        var guids = new List<Guid>() { guidPlayer };
        SendPresent(guids, presentInfo.Name);
    }

    private void SendGiftsToAll(PresentInfoDto presentInfo)
    {
        var guids = new List<Guid>();
        foreach (var p in Client.TableData.Players)
            guids.Add(p.Id);
        SendPresent(guids, presentInfo.Name);
    }
}