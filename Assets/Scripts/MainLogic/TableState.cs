using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableState
{
    private List<Animator> _playersAnimator = new List<Animator>();
    private List<Player> _players = new List<Player>();
    private UiConnector _screenData;
    private List<Card> _tableCards = new List<Card>();
    private GameModeSettings _gameModeSettings;
    private CardManager _cardManager;
    private PlayerData _playerData;
    private Player _myPlayer;
    private List<int> _values;
    private List<int> _alreadyGetBank;
    private List<int> _winnerIndexes;
    private Status _status;
    private bool _isFirstStep;
    private int _prize;
    private int _tableID;
    private int _value;
    private int _indexLastOpenCard;
    private int _currentPlayer;
    private int _tableBet;
    private int _dealerCount;
    private int _smallBlindCount = 5;
    private int _bigBlindCount = 10;
    private int _timerPlayerTurn = 5;
    private int _lastCheckPlayerIndex;
    private int _winnerIndex;
    private int _step = 0;
    private int _stepPlayerCounter = 0;
    private bool _isJokerGame;
}
