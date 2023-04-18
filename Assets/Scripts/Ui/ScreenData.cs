using PokerHand.Common.Dto;
using PokerHand.Common.Helpers.QuickChat;
using System;
using UnityEngine;

[Serializable]
public class ScreenData
{
    public void SetPlayerWinText(int index, string value) { OnSetPlayerWinnerText?.Invoke(index, value); }
    public event Action<int, string> OnSetPlayerWinnerText;
    public string PlayerCombin { get { return playerCombin; } set { playerCombin = value; OnPlayerCombinChanged?.Invoke(playerCombin); } }
    public event Action<string> OnPlayerCombinChanged;
    private string playerCombin;
    public (long, int) PlayerBet { get { return playerBet; } set { playerBet = value; OnPlayerBetChanged?.Invoke(playerBet); } }
    public event Action<(long, int)> OnPlayerBetChanged;
    private (long, int) playerBet;
    public PlayerProfileDto AnotherPlayerProfileInfo { get { return playerProfile; } set { playerProfile = value; OnGetPlayerProfileInfo?.Invoke(playerProfile); } }
    public event Action<PlayerProfileDto> OnGetPlayerProfileInfo;
    private PlayerProfileDto playerProfile;
    public (int, int) BlindPrice { get { return blindPrice; } set { blindPrice = value; OnBlindPriceChanged?.Invoke(blindPrice); } }
    public event Action<(int, int)> OnBlindPriceChanged;
    private (int, int) blindPrice;
    public long PlayerMoneyCount { get { return playerMoneyCount; } set { playerMoneyCount = value; OnPlayerMoneyCountChanged?.Invoke(playerMoneyCount); } }
    public event Action<long> OnPlayerMoneyCountChanged;
    private long playerMoneyCount;
    public int IndexCurrentPlayer { get { return indexCurrentPlayer; } set {  OnIndexCurrentPlayerChanged?.Invoke(indexCurrentPlayer, value); indexCurrentPlayer = value; } }
    public event Action<int, int> OnIndexCurrentPlayerChanged;
    private int indexCurrentPlayer;
    public long Prize { get { return prizeCount; } set { prizeCount = value; OnPrizeChanged?.Invoke(prizeCount); } }
    public event Action<long> OnPrizeChanged;
    private long prizeCount;
    public long CallBet { get { return callBet; } set { callBet = value; OnCallBetChanged?.Invoke(callBet); } }
    public event Action<long> OnCallBetChanged;
    private long callBet;
    public long MinSliderCallBet { get { return minSliderCallBet; } set { minSliderCallBet = value; OnMinSliderCallBetChanged?.Invoke(minSliderCallBet); } }
    public event Action<long> OnMinSliderCallBetChanged;
    private long minSliderCallBet;
    public int Countdown { get { return countdown; } set { countdown = value; OnCountdownChanged?.Invoke(countdown); } }
    public event Action<int> OnCountdownChanged;
    private int countdown;
    public bool MyPlayerBetEqual { get { return myPlayerBetNotEqual; } set { myPlayerBetNotEqual = value; OnMyPlayerBetChangedEqual?.Invoke(myPlayerBetNotEqual); } }
    public event Action<bool> OnMyPlayerBetChangedEqual;
    private bool myPlayerBetNotEqual;
    public bool PlayerSetFoldButton { get { return playerSetFoldButton; } set { playerSetFoldButton = value; OnMyPlayerSetFoldButton?.Invoke(playerSetFoldButton); } }
    public event Action<bool> OnMyPlayerSetFoldButton;
    private bool playerSetFoldButton;
    public bool PlayerSetCallButton { get { return playerSetCallButton; } set { playerSetCallButton = value; OnMyPlayerSetCallButton?.Invoke(playerSetCallButton); } }
    public event Action<bool> OnMyPlayerSetCallButton;
    private bool playerSetCallButton;
    public PlayerPreActionType PlayerSetPreAction { get; set; } = PlayerPreActionType.None;
    public bool PlayerSetCallAnyButton { get { return playerSetCallAnyButton; } set { playerSetCallAnyButton = value; OnMyPlayerSetCallAnyButton?.Invoke(playerSetCallAnyButton); } }
    public event Action<bool> OnMyPlayerSetCallAnyButton;
    private bool playerSetCallAnyButton;
    public void HighLightBankImage(int index, Color color) { OnHighLightBankImage?.Invoke(index, color); }
    public event Action<int, Color> OnHighLightBankImage;
    public void GlowPlayer(int index, bool state) { OnGlowPlayer?.Invoke(index, state); }
    public event Action<int, bool> OnGlowPlayer;
    public void ResetGame() { OnResetGame?.Invoke(); }
    public event Action OnResetGame;    
    public void RestoreMoney() { OnRestoreMoney?.Invoke(); }
    public event Action OnRestoreMoney;
    public void ResetPlayersBet() { OnResetPlayersBet?.Invoke(); }
    public event Action OnResetPlayersBet;
    public void ResetPlayerBet(int index) { OnResetPlayerBet?.Invoke(index); }
    public event Action<int> OnResetPlayerBet;
    public void ResetPlayerCountdown(int index) { OnResetPlayerCountdown?.Invoke(index); }
    public event Action<int> OnResetPlayerCountdown;
    public void ResetPlayerGift(int index) { OnResetPlayerGift?.Invoke(index); }
    public event Action<int> OnResetPlayerGift;
    public void ResetBlinds() { OnResetBlinds?.Invoke(); }
    public event Action OnResetBlinds;
    public void ResetWaitText() { OnResetWaitText?.Invoke(); }
    public event Action OnResetWaitText;
    public void MyPlayerLose() { OnMyPlayerLose?.Invoke(); }
    public event Action OnMyPlayerLose;
    public void MyPlayerNotEnoughtMoney() { OnMyPlayerNotEnoughtMoney?.Invoke(); }
    public event Action OnMyPlayerNotEnoughtMoney;
    public void MyPlayerFinishedSitNGo() { OnMyPlayerFinishedSitNGo?.Invoke(); }
    public event Action OnMyPlayerFinishedSitNGo;
    public void RestartGameUIAction() { OnRestartGameUIAction?.Invoke(); }
    public event Action OnRestartGameUIAction;
    public void UpdatePlayersMoneyUIAction() { OnUpdatePlayersMoneyUIAction?.Invoke(); }
    public event Action OnUpdatePlayersMoneyUIAction;
    public void UpdatePlayerMoneyUIAction(int index) { OnUpdatePlayerMoneyUIAction?.Invoke(index); }
    public event Action<int> OnUpdatePlayerMoneyUIAction;
    public void WaitingForPlayers() { OnWaitingForPlayers?.Invoke(); }
    public event Action OnWaitingForPlayers;
    public void WaitForNewGame() { OnWaitingForNewGame?.Invoke(); }
    public event Action OnWaitingForNewGame;
    public void SetPlayerState(int index, bool newState) { OnSetPlayerState?.Invoke(index, newState); }
    public event Action<int, bool> OnSetPlayerState;
    public void SetAvatar(int index) { OnSetAvatar?.Invoke(index); }
    public event Action<int> OnSetAvatar;
    public void SetPresent(int fromIndex, int toIndex, Sprite sprite, float sizeMod, Vector3 centerOffset) { OnSetPresent?.Invoke(fromIndex, toIndex, sprite, sizeMod, centerOffset); }
    public void RemovePresent(int playerIndex) { OnRemovePresent?.Invoke(playerIndex); }
    public event Action<int, int, Sprite, float, Vector3> OnSetPresent;
    public event Action<int> OnRemovePresent;
    public void SetEmoji(int index, QuickMessage emoji) { OnSetEmoji?.Invoke(index, emoji); }
    public event Action<int, QuickMessage> OnSetEmoji;
    public void SetPresentImmediatly(int index, Sprite sprite) { OnSetPresentImmediately?.Invoke(index, sprite); }
    public event Action<int, Sprite> OnSetPresentImmediately;
    public void SetTotalMoney(long newValue) { OnSetNewTotalMoney?.Invoke(newValue); }
    public event Action<long> OnSetNewTotalMoney;
    public void SetQuickMessage(QuickMessageDto messageDto) { OnSetQuickMessage?.Invoke(messageDto); }
    public event Action<QuickMessageDto> OnSetQuickMessage;
}