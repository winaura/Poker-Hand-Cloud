using PokerHand.Common.Dto;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Client;

public class GameMode : MonoBehaviour
{
    [SerializeField] private Text _blindText;
    [SerializeField] private GameModes _gameMode;
    [SerializeField] private TablesInWorlds _tablesInWorlds;
    [SerializeField] private float tableExperience;
    [SerializeField] private int _smallBlind;
    [SerializeField] private int _bigBlind;
    [SerializeField] private int _minMoneyGet;
    [SerializeField] private int _maxMoneyGet;
    [SerializeField] private int _countPeople;
    [Header("Sit n Go Mode Settings"), SerializeField] private int _minPrizeSitNGo;
    [SerializeField] private int _maxPrizeSitNGo;
    [SerializeField] private int _startingChipsSitNGo;
    private GameModeSettings _gameModeSettings;
    private TableInfoDto tableInfo;
    public GameModes GameModeType => _gameMode;
    void Start() => _gameModeSettings = GameModeSettings.Instance;

    private void OnEnable()
    {
        if (!IsTableInfoExist())
            OnReceiveAllTablesInfo += OnTablesInfoReceived;
        else
            UpdateTableInfo();
    }

    private void OnDisable() => OnReceiveAllTablesInfo -= OnTablesInfoReceived;

    public void SetDataForInviting(TableInfoDto tableInfo) 
    {
        print($"{DateTime.UtcNow} SetDataForInviting");
        _tablesInWorlds = tableInfo.Title;
        _gameMode = (GameModes)tableInfo.TableType;
        _smallBlind = tableInfo.SmallBlind;
        _bigBlind = tableInfo.BigBlind;
        _minMoneyGet = tableInfo.MinBuyIn;
        _maxMoneyGet = tableInfo.MaxBuyIn;
        _countPeople = tableInfo.MaxPlayers;
        tableExperience = tableInfo.Experience;
        _minPrizeSitNGo = tableInfo.SecondPlacePrize;
        _maxPrizeSitNGo = tableInfo.FirstPlacePrize;
        _startingChipsSitNGo = tableInfo.InitialStack;
        UpdateTableText();
    }

    public void ButtonClick()
    {
        _gameModeSettings.TablesInWorlds = _tablesInWorlds;
        _gameModeSettings.smallBlind = _smallBlind;
        _gameModeSettings.bigBlind = _bigBlind;
        _gameModeSettings.minMoneyGet = _minMoneyGet;
        _gameModeSettings.maxMoneyGet = _maxMoneyGet;
        _gameModeSettings.countPeople = _countPeople;
        _gameModeSettings.minPrizeSitNGo = _minPrizeSitNGo;
        _gameModeSettings.maxPrizeSitNGo = _maxPrizeSitNGo;
        _gameModeSettings.sitNGoMoneyGet = _startingChipsSitNGo;
        _gameModeSettings.gameMode = _gameMode;
        _gameModeSettings.tableExperience = tableExperience;
        PlayerProfileData.Instance.playerGetMoney = _minMoneyGet;
        PlayerProfileData.Instance.MinMoneyNeed = _minMoneyGet;
    }

    void OnTablesInfoReceived()
    {
        if (IsTableInfoExist())
        {
            UpdateTableInfo();
            OnReceiveAllTablesInfo -= OnTablesInfoReceived;
        }
    }

    bool IsTableInfoExist()
    {
        if (TablesInfo == null)
            return false;
        tableInfo = TablesInfo.Find(t => t.Title == _tablesInWorlds);
        if (tableInfo == null)
        {
            Debug.LogError($"{DateTime.UtcNow} Table not found {_tablesInWorlds}");
            return false;
        }
        return true;
    }

    void UpdateTableInfo()
    {
        _tablesInWorlds = tableInfo.Title;
        _gameMode = (GameModes)tableInfo.TableType;
        _smallBlind = tableInfo.SmallBlind;
        _bigBlind = tableInfo.BigBlind;
        _minMoneyGet = tableInfo.MinBuyIn;
        _maxMoneyGet = tableInfo.MaxBuyIn;
        _countPeople = tableInfo.MaxPlayers;
        tableExperience = tableInfo.Experience;
        _minPrizeSitNGo = tableInfo.SecondPlacePrize;
        _maxPrizeSitNGo = tableInfo.FirstPlacePrize;
        _startingChipsSitNGo = tableInfo.InitialStack;
        UpdateTableText();
    }

    void UpdateTableText()
    {
        if (_gameMode == GameModes.lowball ||
            _gameMode == GameModes.Dash ||
            _tablesInWorlds == TablesInWorlds.Private)
            return;
        if (_blindText != null)
            _blindText.text = _minMoneyGet == _maxMoneyGet
                ? $"${_minMoneyGet.IntoCluttered()}"
                : $"${_minMoneyGet.IntoCluttered()}/${_maxMoneyGet.IntoCluttered()}";
    }
}