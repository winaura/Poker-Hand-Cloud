using PokerHand.BusinessLogic.Services;
using PokerHand.Common.Dto;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.Player;
using PokerHand.Common.Helpers.Present;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using static Client;
using static UnityEngine.SceneManagement.SceneManager;

public class TableManager : MonoBehaviour
{
    public List<SpriteRenderer> _mySpritesCard = new List<SpriteRenderer>();
    private List<Animator> _playersAnimator = new List<Animator>();
    private List<Player> _players = new List<Player>();
    private ScreenData _screenData = new ScreenData();
    private List<Card> _tableCards = new List<Card>();
    private GameModeSettings _gameModeSettings;
    private CardManager _cardManager;
    private Player _myPlayer;
    private List<int> _winnerIndexes;
    private int _indexLastOpenCard;
    private int _currentPlayer;
    private long _tableBet;
    private int _awayCounter = 0;
    private long initialTotalMoney = 0;
    private Coroutine CoroutineCountdownToCheck;
    private List<PlayerDto> playersOnLastUpdate;
    private bool isFolded = false;
    [Header("Hand materials")]
    [SerializeField] Material[] handMaterials; // 0 - wm, 1 - bm, 2 - ww, 3 - bw
    [SerializeField] Material[] handTransparentMaterial; // the same but a transparent one
    [Header("Links")]
    [SerializeField] private GiftData _giftData;
    [SerializeField] private Animator _dealerHandsAnimator;
    [SerializeField] private GameObject chipPrefab;
    [HideInInspector] public List<GameObject> playersOnTable;
    [SerializeField] private List<GameObject> _fivePlayersOnTable;
    [SerializeField] private List<GameObject> _eightPlayersOnTable;
    [SerializeField] private List<CardUnit> _cardUnits;
    [SerializeField] private List<CardUnit> _tableCardUnits;
    [SerializeField] private GameObject _croupier, _smallBlind, _bigBlind, _dealer;
    [SerializeField] private OpenCardsChoiseWindowController openCardsChoiseController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private UIManager uiManager;
    private GameObject[] myFantomCardsObj = new GameObject[2];
    private List<Card> myFantomCards = new List<Card>(2);
    private bool isFantomMode = false;
    public event Action<int, bool> OnPlayerShowThinkTimer;

    private void Awake()
    {
        playersOnLastUpdate = new List<PlayerDto>();
        playersOnLastUpdate.AddRange(Client.TableData.Players);
        StartListenTableEvents();
        initialTotalMoney = MyPlayer.StackMoney + PlayerProfileData.Instance.TotalMoney;
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo)
            SitnGoTimer.StartTimer(Client.TableData.SitAndGoTimeout, LeaveTable);
    }

    private void Start()
    {
        SettingsManager.Instance.UpdateTextsEvent += MP_CheckMyCombination;
        SendActivePlayerStatus();
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= MP_CheckMyCombination;
        StopListenTableEvents();
    }

    private void OnDisable()
    {
        if (_gameModeSettings != null && _gameModeSettings.gameMode == GameModes.SitNGo)
            PlayerData.Instance.playerGetMoney = _gameModeSettings.minMoneyGet;
        StopAllCoroutines();
    }

    void StartListenTableEvents()
    {
        OnReceiveTable += MP_OnTableReceived;
        OnFirstTableDataReceived += MP_OnFirstTableDataReceived;
        OnFirstTableDataReceived += ReloadGame;
        OnReceiveUpdatedPot += MP_OnReceiveUpdatedPot;
        OnReceivePlayerDto += MP_OnReceivePlayerDto;
        OnReceivePrepareForGame += MP_OnPrepareForGame;
        OnReceiveDealCommunityCards += MP_OnDealCommunityCards;
        OnReceivePlayerAction += MP_OnPlayerActionReceived;
        OnReceiveCurrentPlayerId += MP_OnReceiveCurrentPlayerId;
        OnReceiveWinners += MP_OnReceiveWinners;
        OnPlayerDisconnected += MP_OnAnotherPlayerDisconnected;
        OnReceiveOnLackOfStackMoney += MP_OnLackOfMoney;
        OnReceiveOnGameEnd += MP_OnGameEnd;
        OnReceiveEndSitAndGoGame += MP_OnEndSitAndGoGame;
        OnReceiveProfileImage += MP_OnReceiveProfileImage;
        OnAnotherPlayerConnected += MP_OnAnotherPlayerConnected;
        OnReceiveMyPlayerProfile += MP_OnReceivePlayerProfile;
        OnReceivePresent += MP_OnReceivePresent;
        OnPlayerDisconnected += MP_OnRemovePresent;
        OnReceiveMyPlayerTotalMoney += MP_OnReceiveMyTotalMoney;
        OnReceiveQuickMessage += MP_OnReceiveQuickMessage;
        OnReceiveShowWinnerCards += MP_OnShowWinnerCards;
        OnReceiveAnotherPlayerProfile += MP_OnReceivePlayerProfile;
        OnReceiveMessageOnLackOfMoneyInDash += MP_OnShowMessageOnLackofMoneyInDash;
        OnKickPlayerFromTable += MP_KickFromTable;
        OnActFoldByInactivePlayer += MP_FoldCards;
        OnReceiveMyPlayerProfile += SetPlayerHandsMaterial;
        OnReceiveAnotherPlayerProfile += SetPlayerHandsMaterial;
    }

    void StopListenTableEvents()
    {
        OnReceiveTable -= MP_OnTableReceived;
        OnFirstTableDataReceived -= MP_OnFirstTableDataReceived;
        OnFirstTableDataReceived -= ReloadGame;
        OnReceiveUpdatedPot -= MP_OnReceiveUpdatedPot;
        OnReceivePlayerDto -= MP_OnReceivePlayerDto;
        OnReceivePrepareForGame -= MP_OnPrepareForGame;
        OnReceiveDealCommunityCards -= MP_OnDealCommunityCards;
        OnReceivePlayerAction -= MP_OnPlayerActionReceived;
        OnReceiveCurrentPlayerId -= MP_OnReceiveCurrentPlayerId;
        OnReceiveWinners -= MP_OnReceiveWinners;
        OnPlayerDisconnected -= MP_OnAnotherPlayerDisconnected;
        OnReceiveOnLackOfStackMoney -= MP_OnLackOfMoney;
        OnReceiveOnGameEnd -= MP_OnGameEnd;
        OnReceiveEndSitAndGoGame -= MP_OnEndSitAndGoGame;
        OnReceiveProfileImage -= MP_OnReceiveProfileImage;
        OnAnotherPlayerConnected -= MP_OnAnotherPlayerConnected;
        OnReceiveMyPlayerProfile -= MP_OnReceivePlayerProfile;
        OnReceivePresent -= MP_OnReceivePresent;
        OnReceiveMyPlayerTotalMoney -= MP_OnReceiveMyTotalMoney;
        OnReceiveQuickMessage -= MP_OnReceiveQuickMessage;
        OnReceiveShowWinnerCards -= MP_OnShowWinnerCards;
        OnReceiveAnotherPlayerProfile -= MP_OnReceivePlayerProfile;
        OnReceiveMessageOnLackOfMoneyInDash -= MP_OnShowMessageOnLackofMoneyInDash;
        OnKickPlayerFromTable -= MP_KickFromTable;
        OnActFoldByInactivePlayer -= MP_FoldCards;
        OnPlayerDisconnected -= MP_OnRemovePresent;
        OnReceiveMyPlayerProfile -= SetPlayerHandsMaterial;
        OnReceiveAnotherPlayerProfile -= SetPlayerHandsMaterial;
    }

    public void LeaveTable()
    {
        ConnectingWindow.CreateWindow(ConnectingType.DisconnectingFromTable);
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo && PlayerProfileData.Instance.SitNGoStarted)
        {
            PlayerProfileData.Instance.SitNGoPlace = Client.TableData.Players.Count;
            PlayerProfileData.Instance.SitNGoStarted = false;
        }
        else
            PlayerProfileData.Instance.SitNGoPlace = -1;
        SendLeaveTable();
        StopListenTableEvents();
        SetCurrentPlayerToNull();
        LoadScene(GameConstants.SceneMainMenu);
    }

    private void MP_KickFromTable()
    {
        MessagingSystem.AddMessage(MessageType.KickedAFK);
        SetCurrentPlayerToNull();
        StopListenTableEvents();
        LoadScene(GameConstants.SceneMainMenu);
    }

    private void ReloadGame() => SceneManager.LoadScene(GameConstants.SceneGame);

    private void MP_FoldCards() => MP_Fold(isServerInvoking: true);
   
    public void MP_OnEndSitAndGoGame(int place)
    {
        PlayerProfileData.Instance.SitNGoPlace = place;
        PlayerProfileData.Instance.SitNGoStarted = false;
        StopListenTableEvents();
        LoadScene(GameConstants.SceneMainMenu);
    }

    public void MP_InitGame()
    {
        if (GameModeSettings.Instance != null)
            _gameModeSettings = GameModeSettings.Instance;
        _players[0].level = new LevelCounter(PlayerProfileData.Instance.Experience).Level;
        _players[0].nickname = MyPlayer.UserName;
        _players[0].currentExp = PlayerProfileData.Instance.Experience;
        _players[0].money = MyPlayer.StackMoney;
        _myPlayer = _players[0];
        uiManager.UpdateData().SetData(_screenData);
        _screenData.UpdatePlayersMoneyUIAction();
        MP_OnConnectToTable();
        MP_UpdatePlayersState();
    }

    private void MP_OnShowMessageOnLackofMoneyInDash()
    {
        shopController.OpenWindow(true);
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = LoadSceneAsync(0);
        while (!asyncLoad.isDone)
            yield return null;
    }

    void MP_OnConnectToTable()
    {
        // Check on community cards exist on the table
        if (Client.TableData.CommunityCards != null && Client.TableData.CommunityCards.Count > 0)
            SetCommunityCardsImmediate();
        // Check on players cards exist on the table
        if (Client.TableData.CurrentStage > RoundStageType.PrepareTable)
        {
            MP_SetBlindsAndDealer();
            SetPocketCardsImmediate();
        }
        SetPresentsImmediately();
        _screenData.Prize = Client.TableData.Pot.TotalAmount;
        _screenData.PlayerCombin = string.Empty;
        foreach (var p in Client.TableData.Players)
            SetPlayerHandsMaterial(p.LocalIndex, p.HandsSprite);
        MP_CheckWaitingForPlayers();
    }

    void SetCommunityCardsImmediate()
    {
        var newTableCards = _cardManager.MP_ConvertCardsDtoToCards(Client.TableData.CommunityCards);
        _tableCards.AddRange(newTableCards);
        for (var i = 0; i < newTableCards.Count; i++)
        {
            _tableCardUnits.Add(GetFreeUnit());
            OpenTableCard();
        }
    }

    void SetPocketCardsImmediate()
    {
        foreach (var p in Client.TableData.ActivePlayers)
        {
            var index = p.LocalIndex;
            _players[index].CardsOnHand = _cardManager.MP_ConvertCardsDtoToCards(p.PocketCards);
            _players[index].myCards = new List<CardUnit>();
            for (var j = 0; j < _players[index].CardsHand.Count; j++)
            {
                var cardTrans = playersOnTable[index].transform.GetChild(j).transform;
                var cardUnit = SetCardUnit(_players[index].CardsHand[j], new Vector3(cardTrans.position.x, cardTrans.position.y, 0), cardTrans.rotation);
                if (j == 0)
                    cardUnit.transform.position = new Vector3(cardUnit.transform.position.x, cardUnit.transform.position.y, 1f);
                _players[index].myCards.Add(cardUnit);
            }
        }
    }

    void SetPresentsImmediately()
    {
        foreach (var p in Client.TableData.Players)
            if (p.PresentName != PresentName.None)
            {
                var sprite = _giftData.GiftsInfo.GetGiftSpriteByName(p.PresentName);
                _screenData.SetPresentImmediatly(p.LocalIndex, sprite);
            }
    }

    void MP_OnReceivePlayerProfile(PlayerProfileDto playerProfileDto) => _screenData.AnotherPlayerProfileInfo = playerProfileDto;

    void MP_OnRemovePresent(int playerIndex) => _screenData.RemovePresent(playerIndex);

    void MP_OnReceivePresent(PresentDto present)
    {
        float sizeMod;
        Vector3 centerOffset;
        switch(present.Name)
        {
            case PresentName.BottleWine:
                sizeMod = 1.4f;
                centerOffset = Vector3.zero;
                break;
            case PresentName.Lollipop:
                sizeMod = 1.4f;
                centerOffset = new Vector3(-7.4f, -29.1f, 0);
                break;
            case PresentName.Shampoo:
                sizeMod = 1.3f;
                centerOffset = Vector3.zero;
                break;
            case PresentName.Necklace:
                sizeMod = 1f;
                centerOffset = new Vector3(25f, 28f, 0);
                break;
            default:
                sizeMod = 1;
                centerOffset = Vector3.zero;
                break;
        }
        var from = Client.TableData.Players.First(t => t.Id == present.SenderId).LocalIndex;
        var sprite = _giftData.GiftsInfo.GetGiftSpriteByName(present.Name);
        foreach(var recipient in present.RecipientsIds)
        {
            var to = Client.TableData.Players.First(t => t.Id == recipient).LocalIndex;
            _screenData.SetPresent(from, to, sprite, sizeMod, centerOffset);
        }
    }

    void MP_OnReceiveMyTotalMoney(long newValue) => _screenData.SetTotalMoney(newValue);

    void MP_OnReceiveQuickMessage(QuickMessageDto messageDto)
    {
        var messageGroup = (int)messageDto.Message / 100;
        if (messageGroup == 1)
            _screenData.SetQuickMessage(messageDto);
        else if (messageGroup == 2)
            _screenData.SetEmoji(messageDto.SenderIndex.ToLocalIndex(), messageDto.Message);
    }

    void MP_OnAnotherPlayerConnected(PlayerDto playerDto) => SetPlayerHandsMaterial(playerDto.LocalIndex, playerDto.HandsSprite);

    void MP_OnShowWinnerCards()
    {
        var winnerIndex = Winners[0].LocalIndex;
        OnPlayerShowThinkTimer?.Invoke(winnerIndex, false);
        OpenPlayerCards(winnerIndex);
        _players[winnerIndex].combination.Clear();
        _players[winnerIndex].combination.AddRange(_cardManager.MP_ConvertCardsDtoToCards(Winners[0].HandCombinationCards));
        if (Winners[0].Hand == HandType.HighCard)
            _players[winnerIndex].combination.AddRange(_tableCards);
        _players[winnerIndex].nameOfCombination = Winners[0].Hand.ToCombinationString();
        _screenData.SetPlayerWinText(winnerIndex, SettingsManager.Instance.GetString(_players[winnerIndex].nameOfCombination));
        LightOnWinCombination(winnerIndex);
        _screenData.GlowPlayer(winnerIndex, true);
    }

    void SetPlayerHandsMaterial(PlayerProfileDto playerProfileDto)
    {
        if (playerProfileDto.TryGetPlayerProfileLocalIndex(out int localIndex))
            SetPlayerHandsMaterial(localIndex, playerProfileDto.HandsSprite);
    }


    void SetPlayerHandsMaterial(int index, HandsSpriteType handsSpriteType)
    {
        int handIndex;
        switch (handsSpriteType)
        {
            case HandsSpriteType.WhiteMan: handIndex = 0; break;
            case HandsSpriteType.BlackMan: handIndex = 1; break;
            case HandsSpriteType.WhiteWoman: handIndex = 2; break;
            case HandsSpriteType.BlackWoman: handIndex = 3; break;
            default: handIndex = 0; break;
        }
        var playerObjInfo = playersOnTable[index].GetComponent<PlayerObjectInfo>();
        playerObjInfo.handsRenderer.material = handMaterials[handIndex];
        playerObjInfo.handsTransparentRenderer.material = handTransparentMaterial[handIndex];
    }

    void MP_OnTableReceived()
    {
        if (_mySpritesCard.Count == 0)
            MP_OpenMyCards();
        else if (_mySpritesCard[0].size != GameConstants.FullCardSize)
            foreach (var c in _mySpritesCard)
                c.size = GameConstants.FullCardSize;
        MP_CheckWaitingForPlayers();
        MP_CheckWaitingForNewGame();
        MP_UpdateBetColors();
        MP_UpdatePlayersState();
        MP_UpdatePlayersInfo();
        foreach (var p in Client.TableData.Players)
        {
            if (!p.IsActive())
                p.CurrentBet = 0;
            _screenData.PlayerBet = (p.CurrentBet, p.LocalIndex);
        }
        _screenData.Prize = Client.TableData.Pot.TotalAmount;
        _screenData.PlayerMoneyCount = MyPlayer.StackMoney;
        _screenData.UpdatePlayersMoneyUIAction();
    }

    void MP_OnFirstTableDataReceived() // Called in case of switch table
    {
        SendActivePlayerStatus();
        for (var i = 0; i < GameModeSettings.Instance.countPeople; i++)
            _screenData.ResetPlayerGift(i);
    }

    void MP_OnReceiveProfileImage(int localIndex) => _screenData.SetAvatar(localIndex);

    void MP_OnReceivePlayerDto() => UpdateMyStackMoney();

    void UpdateMyStackMoney()
    {
        _players[0].money = MyPlayer.StackMoney;
        _screenData.PlayerMoneyCount = MyPlayer.StackMoney;
        _screenData.UpdatePlayerMoneyUIAction(0);
    }

    void MP_CheckWaitingForPlayers()
    {
        if (Client.TableData.CurrentStage == RoundStageType.NotStarted || Client.TableData.CurrentStage == RoundStageType.Created || Client.TableData.Players.Count == 1)
        {
            _screenData.WaitingForPlayers();
            print($"{DateTime.UtcNow} Waiting for players...");
        }
    }

    void MP_CheckWaitingForNewGame()
    {
        if (!IsMyPlayerActive &&
            Client.TableData.CurrentStage > RoundStageType.NotStarted &&
            Client.TableData.CurrentStage < RoundStageType.Refresh &&
            !_players[0].fold)
        {
            _screenData.WaitForNewGame();
            Debug.Log($"{DateTime.UtcNow} Waiting for new game...");
        }
    }

    void MP_OnGameEnd() => MP_ResetGame();

    void MP_OnPrepareForGame()
    {
        StopAllCoroutines();
        MP_ResetGame();
        MP_SetBlindsAndDealer();
        StartCoroutine(MP_PrepareToStart());
    }

    IEnumerator MP_PrepareToStart()
    {
        yield return StartCoroutine(MP_PlayersCardsDistribution());
        MP_OpenMyCards();
    }

    void MP_OpenMyCards()
    {
        if (_players.Count == 0 ||
            _players[0].myCards == null)
            return;
        if (_players[0].myCards.Count != 0)
        {
            ChangeMyPlayerCards();
            OpenPlayerCards(0);
        }
    }

    public long MP_GetCurrentWinMoneyOnThisGame()
    {
        long currentMoney = MyPlayer.StackMoney + PlayerProfileData.Instance.TotalMoney;
        return currentMoney - initialTotalMoney;
    }

    void MP_SetBlindsAndDealer()
    {
        foreach (var p in Client.TableData.ActivePlayers)
        {
            var objInfo = playersOnTable[p.LocalIndex].GetComponent<PlayerObjectInfo>();
            if (p.IndexNumber == Client.TableData.DealerIndex)
                _dealer.transform.position = objInfo.ButtonPoint.position;
            else if (p.IndexNumber == Client.TableData.SmallBlindIndex)
                _smallBlind.transform.position = objInfo.ButtonPoint.position;
            else if (p.IndexNumber == Client.TableData.BigBlindIndex)
                _bigBlind.transform.position = objInfo.ButtonPoint.position;
        }
        _screenData.UpdatePlayersMoneyUIAction();
    }

    void MP_ResetDealerAndBlinds()
    {
        _dealer.transform.position = GameConstants.DefaultOutOfViewPosition;
        _smallBlind.transform.position = GameConstants.DefaultOutOfViewPosition;
        _bigBlind.transform.position = GameConstants.DefaultOutOfViewPosition;
    }

    void MP_OnReceiveUpdatedPot(Pot pot)
    {
        _screenData.Prize = pot.TotalAmount;
        _screenData.ResetPlayersBet();
        TurnOffBankHighLight();
    }

    void MP_OnReceiveWinners()
    {
        isFolded = false;
        _screenData.Countdown = 0;
        _screenData.ResetPlayersBet();
        uiManager.CloseAllScreen();
        uiManager.DisableTimer();
        if (_players.Count(p => p.CardsHand != null) > 1)
        {
            foreach (var p in Client.TableData.ActivePlayers)
                OpenPlayerCards(p.LocalIndex);
            foreach (var winner in Winners)
            {
                int winnerLocalIndex = winner.LocalIndex;
                _players[winnerLocalIndex].combination.Clear();
                switch(winner.Hand)
                {
                    case HandType.HighCard:
                        _players[winnerLocalIndex].combination.AddRange(_cardManager.MP_ConvertCardsDtoToCards(winner.PocketCards));
                        _players[winnerLocalIndex].combination.AddRange(_tableCards);
                        break;
                    default:
                        _players[winnerLocalIndex].combination.AddRange(_cardManager.MP_ConvertCardsDtoToCards(winner.HandCombinationCards.GetRange(0, winner.Hand.CardsCombinationCount())));
                        break;
                }
                _players[winnerLocalIndex].nameOfCombination = winner.Hand.ToCombinationString();
                _screenData.SetPlayerWinText(winnerLocalIndex, SettingsManager.Instance.GetString(_players[winnerLocalIndex].nameOfCombination));
                LightOnWinCombination(winnerLocalIndex);
                _screenData.GlowPlayer(winnerLocalIndex, true);
            }
        }
        else
        {
            if (Winners[0].LocalIndex == 0)
                openCardsChoiseController.Open(_mySpritesCard);
            else
                OnPlayerShowThinkTimer?.Invoke(Winners[0].LocalIndex, true);
        }
        StartCoroutine(MP_FinishedGame());
    }

    void MP_OnDealCommunityCards() => StartCoroutine(MP_SetTableCards());

    void MP_OnPlayerActionReceived()
    {
        var action = LastPlayerAction;
        if (action.PlayerIndexNumber == MyPlayer.IndexNumber)
            return;
        switch (action.ActionType)
        {
            case PlayerActionType.Bet:
            case PlayerActionType.Raise:
            case PlayerActionType.AllIn:
                MP_MakeABetOrRaise(action.Amount ?? 0);
                break;
            case PlayerActionType.Check:
                MP_Check();
                break;
            case PlayerActionType.Fold:
                MP_Fold();
                break;
            case PlayerActionType.Call:
                MP_Call(action.Amount ?? 0);
                break;
        }
    }

    void MP_OnReceiveCurrentPlayerId()
    {
        StopCountdownToCheck();
        MP_CheckPreActionAvailability();
        if (MyPlayer.Id == CurrentPlayer.Id && _screenData.PlayerSetPreAction == PlayerPreActionType.None)
        {
            if (!isFolded)
                uiManager.OpenScreen(ScreenId.PlayerTurnScreen).SetData(_screenData);
            _screenData.AnotherPlayerProfileInfo = null;
        }
        else if (!isFolded)
            uiManager.OpenScreen(ScreenId.PlayerWaitHisTurnScreen).SetData(_screenData);
        _currentPlayer = CurrentPlayer.LocalIndex;
        SetThinkingAnimation();
        MP_RunCurrentPlayerTurn();
    }

    void MP_OnAnotherPlayerDisconnected(int index)
    {
        _screenData.ResetPlayerBet(index);
        _screenData.ResetPlayerCountdown(index);
        _screenData.ResetPlayerGift(index);
        _screenData.SetAvatar(index);
        StartCoroutine(DealerGrabDelay(0f, GameConstants.AnimGrab));
        FoldPlayerCards(index, 0);
        if (CoroutineCountdownToCheck != null)
            StopCoroutine(CoroutineCountdownToCheck);
    }

    void MP_RunCurrentPlayerTurn()
    {
        _screenData.Countdown = GameConstants.PlayerTurnTime;
        _screenData.IndexCurrentPlayer = _currentPlayer;
        if (MyPlayer.IndexNumber == CurrentPlayer.IndexNumber && PlayerData.Instance?.isVibrationOn == true)
            Vibration.Vibrate(80);
        if (_currentPlayer == MyPlayer.LocalIndex && _screenData.PlayerSetPreAction != PlayerPreActionType.None)
        {
            StartCoroutine(MP_PreActionSimulatedDelay());
            return;
        }
    }

    IEnumerator MP_PreActionSimulatedDelay()
    {
        yield return GameConstants.WaitSeconds_05;
        MP_ReleasePreAction();
    }

    void MP_CheckPreActionAvailability()
    {
        var betDelta = Client.TableData.CurrentMaxBet - MyPlayer.CurrentBet;
        if (betDelta > 0)
        {
            if (_screenData.PlayerSetPreAction == PlayerPreActionType.Check)
                _screenData.PlayerSetPreAction = PlayerPreActionType.None;
            else if (MyPlayer.StackMoney == 0)
            {
                if (_screenData.PlayerSetPreAction == PlayerPreActionType.CallCurrent ||
                    _screenData.PlayerSetPreAction == PlayerPreActionType.CallAny)
                    _screenData.PlayerSetPreAction = PlayerPreActionType.None;
            }
            else if (betDelta > MyPlayer.StackMoney)
            {
                if (IsTableInAllInState())
                {
                    if (_screenData.PlayerSetPreAction == PlayerPreActionType.CallCurrent)
                        _screenData.PlayerSetPreAction = PlayerPreActionType.None;
                }
                else if (_screenData.PlayerSetPreAction == PlayerPreActionType.CallCurrent ||
                         _screenData.PlayerSetPreAction == PlayerPreActionType.CallAny)
                {
                    _screenData.PlayerSetPreAction = PlayerPreActionType.None;
                }
            }
        }
    }

    void MP_ReleasePreAction()
    {
        var amount = Client.TableData.CurrentMaxBet - MyPlayer.CurrentBet;
        switch (_screenData.PlayerSetPreAction)
        {
            case PlayerPreActionType.CheckFold:
                if (amount == 0)
                    MP_Check();
                else
                {
                    switch (GameModeSettings.Instance.TablesInWorlds)
                    {
                        case TablesInWorlds.Dash:
                        case TablesInWorlds.Dash300K:
                        case TablesInWorlds.Dash1M:
                        case TablesInWorlds.Dash10M:
                        //case TablesInWorlds.Dash100M:
                            PressedDashTable();
                            return;
                    }
                    MP_Fold();
                }
                break;
            case PlayerPreActionType.Check: MP_Check(); break;
            case PlayerPreActionType.CallCurrent:
            case PlayerPreActionType.CallAny:
                if (amount == 0)
                    MP_Check();
                else
                    MP_Call(amount);
                break;
        }
        var isCallAny = _screenData.PlayerSetPreAction == PlayerPreActionType.CallAny;
        var isCheckFoldAllowed = _screenData.PlayerSetPreAction == PlayerPreActionType.CheckFold && amount == 0;
        if (!isCallAny && !isCheckFoldAllowed)
            _screenData.PlayerSetPreAction = PlayerPreActionType.None;
    }

    IEnumerator MP_SetTableCards()
    {
        var newTableCards = _cardManager.MP_ConvertCardsDtoToCards(CommunityCards);
        _tableCards.AddRange(newTableCards);
        StartCoroutine(DealCards(newTableCards.Count, 0.3f));
        for (var i = 0; i < newTableCards.Count; i++)
        {
            _tableCardUnits.Add(GetFreeUnit());
            OpenTableCard();
            yield return GameConstants.WaitSeconds_02;
        }
        MP_CheckMyCombination();
    }

    void MP_UpdateBetColors()
    {
        if (IsBlindsReleased && Client.TableData.CurrentPlayer != null)
        {
            foreach (var ap in Client.TableData.ActivePlayers)
            {
                if (ap.IndexNumber == Client.TableData.CurrentPlayer.IndexNumber)
                {
                    var index = ap.LocalIndex;
                    if (ap.StackMoney == 0)
                        _screenData.HighLightBankImage(index, Color.red);
                    else
                        _screenData.HighLightBankImage(index, Color.blue);
                    break;
                }
            }
        }
    }

    public void MP_UpdatePlayersState()
    {
        for (var i = 0; i < playersOnTable.Count; i++)
        {
            var isExists = IsPlayerExists(i);
            playersOnTable[i].SetActive(isExists);
            _screenData.SetPlayerState(i, isExists);
        }
    }

    public void MP_UpdatePlayersInfo()
    {
        foreach (var p in Client.TableData.Players)
        {
            var index = p.LocalIndex;
            _players[index].nickname = p.UserName;
            _players[index].money = p.StackMoney;
            _players[index].bet = p.CurrentBet;
        }
    }

    IEnumerator MP_PlayersCardsDistribution()
    {
        StartCoroutine(DealCards(Client.TableData.ActivePlayers.Count, 0.3f));
        foreach (var p in Client.TableData.ActivePlayers)
        {
            if (p.PocketCards == null)
                continue;
            var index = p.LocalIndex;
            _players[index].CardsOnHand = _cardManager.MP_ConvertCardsDtoToCards(p.PocketCards);
            var objInfo = playersOnTable[index].GetComponent<PlayerObjectInfo>();
            for (var j = 0; j < _players[index].CardsHand.Count; j++)
            {
                var newCard = SetCardUnit(_players[index].CardsHand[j], new Vector3(0, 1.35f, 0), transform.rotation);
                AudioManager.Instance.PlaySound(Clips.CardDistribution, newCard.audioSource, 0.33f);
                LeanTween.moveX(newCard.gameObject, objInfo.CardPoints[j].position.x, 0.5f);
                LeanTween.moveY(newCard.gameObject, objInfo.CardPoints[j].position.y, 0.5f);
                LeanTween.rotate(newCard.gameObject, objInfo.CardPoints[j].eulerAngles, 0.5f);
                if (j == 0)
                    newCard.gameObject.transform.position = new Vector3(newCard.gameObject.transform.position.x, newCard.gameObject.transform.position.y, 1f);
                _players[index].myCards.Add(newCard);
            }
            yield return GameConstants.WaitSeconds_03;
        }
        MP_CheckMyCombination();
    }

    private void AddDummyPlayer(int index)
    {
        if (_players.Count <= playersOnTable.Count)
        {
            var player = new Player();
            _players.Insert(index, player);
            var objInfo = playersOnTable[index].GetComponent<PlayerObjectInfo>();
            objInfo.HandsGo.SetActive(PlayerData.Instance.isHandsOn);
            var animationTemp = objInfo.HandsGo.GetComponent<Animator>();
            _playersAnimator.Add(animationTemp);
        }
    }

    public void CreateSession(int countPeople)
    {
        playersOnTable = countPeople == 8 ? _eightPlayersOnTable : _fivePlayersOnTable;
        for (var i = 0; i < playersOnTable.Count; i++)
            if (!playersOnTable[i].activeSelf)
                AddDummyPlayer(i);
    }

    public void MP_MakeABetOrRaise(long count)
    {
        if (CurrentPlayer == null)
            return;
        int currentPlayerIndex;
        if (MyPlayer != null && CurrentPlayer.IndexNumber == MyPlayer.IndexNumber)
        {
            currentPlayerIndex = MyPlayer.LocalIndex;
            if (MyPlayer.StackMoney <= count)
                SendPlayerAction(PlayerActionType.AllIn);
            else if (Client.TableData.CurrentMaxBet == 0)
                SendPlayerAction(PlayerActionType.Bet, count);
            else
                SendPlayerAction(PlayerActionType.Raise, count);
        }
        else
            currentPlayerIndex = LastPlayerAction.PlayerIndexNumber.ToLocalIndex();
        MP_SetRaiseAnimation(currentPlayerIndex);
        _screenData.MyPlayerBetEqual = MyPlayer.CurrentBet == Client.TableData.CurrentMaxBet;
        if (CurrentPlayer.CurrentBet == Client.TableData.BigBlind)
            StartCoroutine(PlaySoundAfterDelay(Clips.BlindBet, 0.8f));
        else
            StartCoroutine(PlaySoundAfterDelay(Clips.Raise, 0.8f));
        StopCountdownToCheck();
    }

    public void MP_Check(bool isAutoAction = false)
    {
        if (CurrentPlayer == null)
            return;
        int currentPlayerIndex = -1;
        if (MyPlayer != null && CurrentPlayer.IndexNumber == MyPlayer.IndexNumber)
        {
            SendPlayerAction(PlayerActionType.Check);
            currentPlayerIndex = MyPlayer.LocalIndex;
            if (isAutoAction)
                IncreaseAFKCounter();
        }
        else
            currentPlayerIndex = LastPlayerAction.PlayerIndexNumber.ToLocalIndex();
        if (_players[currentPlayerIndex].money <= 0)
        {
            if (_tableBet < _players[currentPlayerIndex].money)
            {
                _tableBet = _players[currentPlayerIndex].money;
                _screenData.CallBet = _players[currentPlayerIndex].money;
                _screenData.MinSliderCallBet = _players[currentPlayerIndex].money;
                _myPlayer.call = false;
            }
        }
        _screenData.MyPlayerBetEqual = _myPlayer.bet == _tableBet ? true : false;
        AudioManager.Instance.PlaySound(Clips.CheckCard, _players[currentPlayerIndex].myCards[0].audioSource);
        MP_SetCheckAnimation(currentPlayerIndex);
        StopCountdownToCheck();
    }

    public void MP_Fold(bool isAutoAction = false, bool isServerInvoking=false)
    {
        if (MyPlayer.LocalIndex == _currentPlayer)
        {
            isFolded = true;
            if (isAutoAction)
                IncreaseAFKCounter();
            if(!isServerInvoking)
                SendPlayerAction(PlayerActionType.Fold);
        }
        if (_currentPlayer == 0 &&
            !IsMyTurnWillBeTheLast())
            SetFantomMode(true);
        AudioManager.Instance.PlaySound(Clips.FoldCard, _players[_currentPlayer].myCards[0].audioSource);
        StartCoroutine(DealerGrabDelay(0f, GameConstants.AnimGrab));
        MP_SetFoldAnimation(_currentPlayer);
        FoldPlayerCards(_currentPlayer);
        StopCountdownToCheck();
    }

    public void MP_Call(long amount)
    {
        if (CurrentPlayer == null)
            return;
        int currentPlayerIndex;
        if (MyPlayer != null && CurrentPlayer.IndexNumber == MyPlayer.IndexNumber)
        {
            currentPlayerIndex = MyPlayer.LocalIndex;
            if (MyPlayer.StackMoney <= amount)
                SendPlayerAction(PlayerActionType.AllIn);
            else
                SendPlayerAction(PlayerActionType.Call, amount);
        }
        else
            currentPlayerIndex = LastPlayerAction.PlayerIndexNumber.ToLocalIndex();
        if (_players[currentPlayerIndex].money <= amount)
        {
            if (_tableBet < _players[currentPlayerIndex].money)
            {
                _screenData.CallBet = _players[currentPlayerIndex].money;
                _screenData.MinSliderCallBet = _players[currentPlayerIndex].money;
                _myPlayer.call = false;
            }
        }
        else if (_tableBet < amount)
        {
            _tableBet = amount;
            _screenData.CallBet = amount;
            _screenData.MinSliderCallBet = amount;
            _myPlayer.call = false;
        }
        _screenData.MyPlayerBetEqual = _myPlayer.bet == _tableBet ? true : false;
        if (_players[currentPlayerIndex].bet == _gameModeSettings.bigBlind)
            StartCoroutine(PlaySoundAfterDelay(Clips.BlindBet, 0.8f));
        else
            StartCoroutine(PlaySoundAfterDelay(Clips.Raise, 0.8f));
        MP_SetRaiseAnimation(currentPlayerIndex);
        StopCountdownToCheck();
    }

    private IEnumerator PlaySoundAfterDelay(Clips clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.PlaySound(clip, _players[_currentPlayer].myCards[0].audioSource);
    }

    void IncreaseAFKCounter()
    {
        _awayCounter++;
        if (_awayCounter >= 2)
        {
            MessagingSystem.AddMessage(MessageType.KickedAFK);
            _awayCounter = 0;
            LeaveTable();
        }
    }

    public void MP_CheckMyCombination()
    {
        if (!isFantomMode && (MyPlayer.PocketCards == null || MyPlayer.PocketCards.Count == 0 || !MyPlayer.IsActive()))
            return;
        var openedCards = new List<Card>();
        openedCards.AddRange(_tableCards);
        var handCards = isFantomMode
            ? myFantomCards
            : _players[0].CardsHand;
        CardEvaluationService.FindCombination(handCards, openedCards, out string combinationName, out List<Card> temp);
        _myPlayer.combination = temp;
        _myPlayer.nameOfCombination = combinationName;
        _screenData.PlayerCombin = $"{SettingsManager.Instance.GetString(combinationName)}";
    }

    public void MP_GetPlayerCombination(int index)
    {
        var openedCards = new List<Card>();
        openedCards.AddRange(_tableCards);
        CardEvaluationService.FindCombination(_players[index].CardsHand, openedCards, out string combinationName, out List<Card> winCards);
        _players[index].combination = winCards;
        _players[index].nameOfCombination = combinationName;
    }

    private void MP_ResetGame()
    {
        for (var i = 0; i < _cardUnits.Count; i++)
            _cardUnits[i].Clear();
        SetCardsDefault();
        MP_ResetDealerAndBlinds();
        if (isFantomMode)
            SetFantomMode(false);
        _cardManager.ResetBlackList();
        _winnerIndexes = new List<int>();
        _tableCardUnits.Clear();
        _tableCards.Clear();
        _indexLastOpenCard = 0;
        _currentPlayer = 0;
        _tableBet = 0;
        _awayCounter = 0;
        _screenData.PlayerSetPreAction = PlayerPreActionType.None;
        _screenData.PlayerCombin = string.Empty;
        _screenData.PlayerMoneyCount = _myPlayer.money;
        _screenData.ResetGame();
        _screenData.CallBet = _tableBet;
        _screenData.MinSliderCallBet = _tableBet;
        _screenData.RestartGameUIAction();
        _screenData.ResetBlinds();
        _screenData.Prize = 0;
        _screenData.MyPlayerBetEqual = false;
        for (var i = 0; i < _players.Count; i++)
        {
            _screenData.SetPlayerWinText(i, string.Empty);
            _screenData.GlowPlayer(i, false);
            _players[i].FullReset();
        }
        ResetCardsChanges();
        ResetMyCardsChanges();
        TurnOffBankHighLight();
        DisableCardsLight();
    }

    private void TurnOffBankHighLight()
    {
        for (var i = 0; i < playersOnTable.Count; i++)
            _screenData.HighLightBankImage(i, Color.gray);
    }

    private CardUnit SetCardUnit(Card card, Vector3 position, Quaternion quaternion)
    {
        CardUnit cardUnit;
        cardUnit = _cardUnits.Find(e => !e.isUse);
        cardUnit.SetData(card);
        cardUnit.transform.position = position;
        cardUnit.transform.rotation = quaternion;
        return cardUnit;
    }

    private CardUnit GetFreeUnit()
    {
        CardUnit cardUnit;
        cardUnit = _cardUnits.Find(e => !e.isUse);
        cardUnit.isUse = true;
        return cardUnit;
    }

    private void OpenTableCard()
    {
        if (_indexLastOpenCard >= 5)
            return;
        _tableCardUnits[_indexLastOpenCard].SetData(
            _tableCards[_indexLastOpenCard],
             new Vector3(0, 1.35f, 0),
            Quaternion.identity);
        var newCard = _tableCardUnits[_indexLastOpenCard];
        newCard.gameObject.SetActive(true);
        LeanTween.moveX(newCard.gameObject, _croupier.transform.GetChild(_indexLastOpenCard).transform.position.x, 0.3f);
        LeanTween.moveY(newCard.gameObject, _croupier.transform.GetChild(_indexLastOpenCard).transform.position.y, 0.3f);
        newCard.front.size = GameConstants.FullCardSize;
        OpenNextCardOnTable();
    }

    private void OpenPlayerCards(int index)
    {
        if (index == 0)
        {
            foreach (var card in _players[index].myCards)
                card.IsShowCardBack(true);
            if (_players[index].myCards.Count != 0)
                AudioManager.Instance.PlaySound(Clips.OpenCard, _players[index].myCards[0].audioSource);
        }
        else
        {
            for (var i = 0; i < _players[index].myCards.Count; i++)
            {
                AudioManager.Instance.PlaySound(Clips.OpenCard, _players[index].myCards[0].audioSource);
                _players[index].myCards[i].IsShowCardBack(true);
                _players[index].myCards[i].front.size = GameConstants.FullCardSize;
                _players[index].myCards[i].back.size = GameConstants.FullCardSize;

                var objInfo = playersOnTable[index].GetComponent<PlayerObjectInfo>();
                _players[index].myCards[i].transform.position = objInfo.OpenCardPoints[i].position;
                _players[index].myCards[i].transform.rotation = objInfo.OpenCardPoints[i].rotation;
            }
        }
    }

    private void OpenNextCardOnTable()
    {
        AudioManager.Instance.PlaySound(Clips.TableCardDistribution, _tableCardUnits[_indexLastOpenCard].audioSource, 0.3f);
        _tableCardUnits[_indexLastOpenCard++].IsShowCardBack(true);
    }

    private IEnumerator DealCards(int count, float delay)
    {
        _dealerHandsAnimator.SetBool(GameConstants.AnimIdle, false);
        _dealerHandsAnimator.SetTrigger(GameConstants.AnimStartDeal);
        for (var i = 0; i < count; i++)
        {
            _dealerHandsAnimator.SetTrigger(GameConstants.AnimDealAction);
            yield return new WaitForSeconds(delay);
        }
        _dealerHandsAnimator.SetBool(GameConstants.AnimIdle, true);
        _dealerHandsAnimator.SetTrigger(GameConstants.AnimEndDeal);
    }

    public void StopCountdownToCheck()
    {
        uiManager.StopCountdown();
        if (CoroutineCountdownToCheck != null)
            StopCoroutine(CoroutineCountdownToCheck);
    }

    public void SetCardCollection(List<Card> listCard)
    {
        _cardManager = new CardManager();
        _cardManager.SetCards(listCard);
    }

    private void FoldPlayerCards(int playerIndex, float delay = 0.5f)
    {
        if (_players[playerIndex].myCards == null || _players[playerIndex].myCards.Count == 0)
            return;
        _players[playerIndex].CardsHand = null;
        _players[playerIndex].fold = true;
        _screenData.ResetPlayerBet(playerIndex);
        StartCoroutine(FoldCardsAfterDelay(delay, playerIndex));
        IEnumerator FoldCardsAfterDelay(float foldDelay, int index)
        {
            if (foldDelay > 0)
                yield return new WaitForSeconds(foldDelay);
            var seq = LeanTween.sequence();
            LeanTween.move(_players[index].myCards[0].gameObject, GameConstants.DealerPosition, 0.5f);
            seq.append(LeanTween.move(_players[index].myCards[1].gameObject, GameConstants.DealerPosition, 0.5f));
            if (_players[index].myCards != null && _players[index].myCards[0] != null && _players[index].myCards[1] != null)
            {
                seq.append(() =>
                {
                    if (_players[index].myCards.Any(t => t == null))
                        return;
                    _players[index].myCards[0].gameObject.SetActive(false);
                    _players[index].myCards[1].gameObject.SetActive(false);
                });
                _players[index].myCards[0].IsShowCardBack(false);
                _players[index].myCards[1].IsShowCardBack(false);
            }
        }
    }

    private void SetFantomMode(bool state)
    {
        if (state == true)
        {
            if (_players[0].CardsHand == null)
                return;
            myFantomCards.AddRange(_players[0].CardsHand);
            var cards = _players[0].myCards;
            myFantomCardsObj[0] = Instantiate(cards[0].gameObject, cards[0].transform.parent);
            myFantomCardsObj[1] = Instantiate(cards[1].gameObject, cards[1].transform.parent);
            for (var i = 0; i < myFantomCardsObj.Length; i++)
                myFantomCardsObj[i].GetComponent<CardUnit>().SetColor(GameConstants.WhiteHalfTransparentColor);
            _screenData.ResetWaitText();
        }
        else
        {
            myFantomCards.Clear();
            for (var i = 0; i < myFantomCardsObj.Length; i++)
                Destroy(myFantomCardsObj[i]);
        }
        isFantomMode = state;
    }

    private void LightOnWinCombination(int index)
    {
        foreach (Card tempCard in _players[index].combination)
        {
            for(int i = 0; i < _players[index].myCards.Count; ++i)
            {
                CardUnit playerCard = _players[index].myCards[i];
                if (playerCard.card.value == tempCard.value && playerCard.card.suite == tempCard.suite)
                {
                    playerCard.border.gameObject.SetActive(true);
                    playerCard.border.size = GameConstants.CardLightBorderSize;
                    playerCard.border.sortingOrder = (i + 1) * 2;
                    playerCard.front.sortingOrder = (i + 1) * 2 + 1;
                }
            }
            var combinationTableCards = _tableCardUnits.Where(t => t.card.value == tempCard.value && t.card.suite == tempCard.suite);
            foreach(CardUnit tableCard in combinationTableCards)
            {
                tableCard.border.gameObject.SetActive(true);
                tableCard.border.size = GameConstants.CardLightBorderSize;
                tableCard.front.sortingOrder = 1;
                tableCard.border.sortingOrder = 0;
            }
        }
    }

    private void DisableCardsLight()
    {
        foreach (CardUnit card in _cardUnits)
        {
            card.border.gameObject.SetActive(false);
            card.border.size = GameConstants.DefaultCardSize;
        }
    }

    private void ResetCardsChanges()
    {
        foreach (CardUnit card in _cardUnits)
        {
            card.back.size = GameConstants.DefaultCardSize;
            card.front.size = GameConstants.DefaultCardSize;
            card.border.gameObject.SetActive(false);
        }
    }

    private void ResetMyCardsChanges()
    {
        if (_mySpritesCard.Count == 0 ||
            _players[_currentPlayer] != _myPlayer)
            return;
        _mySpritesCard[0].size = GameConstants.DefaultCardSize;
        _mySpritesCard[1].size = GameConstants.DefaultCardSize;
        _mySpritesCard[2].size = GameConstants.DefaultCardSize;
        _mySpritesCard[3].size = GameConstants.DefaultCardSize;
        _mySpritesCard = new List<SpriteRenderer>();
    }

    private void SetCardsDefault()
    {
        for (var i = 0; i < _cardUnits.Count; i++)
        {
            _cardUnits[i].gameObject.SetActive(true);
            _cardUnits[i].transform.rotation = GameConstants.ZeroQuaternion;
        }
    }

    private void ChangeMyPlayerCards()
    {
        // ToDo: optimize this piece of unstable nuclear reactor
        _mySpritesCard.Add(_players[0].myCards[0].transform.GetChild(0).GetComponent<SpriteRenderer>());
        _mySpritesCard.Add(_players[0].myCards[0].transform.GetChild(1).GetComponent<SpriteRenderer>());
        _mySpritesCard.Add(_players[0].myCards[1].transform.GetChild(0).GetComponent<SpriteRenderer>());
        _mySpritesCard.Add(_players[0].myCards[1].transform.GetChild(1).GetComponent<SpriteRenderer>());
        _mySpritesCard[0].size = GameConstants.FullCardSize;
        _mySpritesCard[1].size = GameConstants.FullCardSize;
        _mySpritesCard[2].size = GameConstants.FullCardSize;
        _mySpritesCard[3].size = GameConstants.FullCardSize;
    }

    public string GetPlayersName(int index) => _players[index].nickname;

    public long GetPlayersMoney(int index) => _players[index].money;

    private void MP_SetCheckAnimation(int playerIndex)
    {
        _playersAnimator[playerIndex].SetTrigger(GameConstants.AnimCheck);
        _playersAnimator[playerIndex].SetBool(GameConstants.AnimThinking, false);
    }

    private void SetThinkingAnimation() => _playersAnimator[_currentPlayer].SetBool(GameConstants.AnimThinking, true);

    private void MP_SetFoldAnimation(int playerIndex)
    {
        _playersAnimator[playerIndex]?.SetTrigger(GameConstants.AnimFold);
        _playersAnimator[playerIndex].SetBool(GameConstants.AnimThinking, false);
    }

    private IEnumerator DealerGrabDelay(float delay, string triggerTag)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        _dealerHandsAnimator.SetTrigger(triggerTag);
    }

    private void MP_SetRaiseAnimation(int playerIndex)
    {
        _playersAnimator[playerIndex].SetTrigger(GameConstants.AnimRaise);
        _playersAnimator[playerIndex].SetBool(GameConstants.AnimThinking, false);
    }

    private void MP_OnLackOfMoney()
    {
        if (PlayerProfileData.Instance.TotalMoney > TableInfo.MinBuyIn)
            _screenData.MyPlayerNotEnoughtMoney();
        else
        {
            ShopController.wasLackOfMoney = true;
            LeaveTable();
        }
    }

    private IEnumerator MP_FinishedGame()
    {
        yield return GameConstants.WaitSeconds_2;
        var chipsForEachWinner = Client.TableData.ActivePlayers.Count / Client.Winners.Count;
        for (var i = 0; i < Client.Winners.Count; i++)
        {
            var winnerIndex = Client.Winners[i].LocalIndex;
            StartCoroutine(SendChipsToPlayer(winnerIndex, chipsForEachWinner));
        }
        foreach (var p in Client.TableData.ActivePlayers)
            _players[p.LocalIndex].money = p.StackMoney;
        _screenData.UpdatePlayersMoneyUIAction();
        _screenData.Prize = 0;
        yield return GameConstants.WaitSeconds_3;
        if (isFantomMode)
            SetFantomMode(false);
        StartCoroutine(DealerGrabDelay(0f, GameConstants.AnimGrabCards));
        foreach (var p in Client.TableData.ActivePlayers)
        {
            var index = p.LocalIndex;
            if (_players[index].CardsHand == null)
                continue;
            AudioManager.Instance.PlaySound(Clips.FoldCard, _players[index].myCards[0].audioSource);
            MP_SetFoldAnimation(index);
            FoldPlayerCards(index);
        }
        foreach (var cardUnit in _tableCardUnits)
        {
            LeanTween.move(cardUnit.gameObject, new Vector3(0, 2f, 0), 0.2f);
            cardUnit.gameObject.SetActive(false);
        }
        yield return GameConstants.WaitSeconds_2;
    }

    private IEnumerator SendChipsToPlayer(int playerIndex, int chipsAmount)
    {
        for (var j = 0; j < chipsAmount; j++)
        {
            var chip = Instantiate(chipPrefab, GameConstants.DealerChipsPosition, Quaternion.identity);
            LeanTween.moveX(chip, playersOnTable[playerIndex].transform.position.x, GameConstants.ChipsLifetime);
            LeanTween.moveY(chip, playersOnTable[playerIndex].transform.position.y, GameConstants.ChipsLifetime);
            LeanTween.rotate(chip, playersOnTable[playerIndex].transform.eulerAngles, GameConstants.ChipsLifetime);
            if (_players[0].myCards != null &&
                _players[0].myCards.Count != 0)
                AudioManager.Instance.PlaySound(Clips.ChipsIntoPlayer, _players[0].myCards[0]?.audioSource);
            Destroy(chip, GameConstants.ChipsLifetime);
            yield return GameConstants.WaitSeconds_02; // dont forget to sync this value with backend on changed ([number of active players] * [this waiting time])
        }
    }

    public void ApplySettings()
    {
        for (var i = 0; i < playersOnTable.Count; i++)
        {
            var hands = playersOnTable[i].transform.GetChild(3);
            hands.gameObject.SetActive(PlayerData.Instance.isHandsOn);
        }
    }
}