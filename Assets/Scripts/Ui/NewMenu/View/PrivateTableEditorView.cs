using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PrivateTableEditorView : MonoBehaviour
{
    [Header("Private Table UI Elements")]
    [SerializeField] private Image _background;
    [SerializeField] private Text _howMuchMoneyText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Button _closeWindowWithMoney;
    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _buttonPlus;
    [SerializeField] private Button _buttonMinus;
    [SerializeField] private Slider _moneyScroll;
    [SerializeField] private Text _buyInAllText;
    [SerializeField] private InputField _minValue;
    [SerializeField] private InputField _maxValue;
    [SerializeField] private Text _countPeopleText;
    [SerializeField] private InputField _bigBlindInput;
    [SerializeField] private InputField _smallBlindInput;
    [SerializeField] private List<Sprite> _backgroundsSprites;
    [SerializeField] private Dropdown _gameModesDropdown;
    private GameModeSettings _gameModeSettings;
    private PlayerData _playerData;
    public UnityAction<float> OnChangedScrollValue { get; set; }
    public event Action OnPlusButtonPressed;
    public event Action OnMinusButtonPressed;
    public event Action OnEnterButton;
    public event Action OnCloseButtonPressed;

    private void Awake()
    {
        _gameModesDropdown.onValueChanged.AddListener(delegate { SwitchMode(_gameModesDropdown); });
        _closeWindowWithMoney.onClick.AddListener(() => OnCloseButtonPressed?.Invoke());
        _moneyScroll.onValueChanged.AddListener(OnChangedScrollValue);
        _buttonPlus.onClick.AddListener(() => OnPlusButtonPressed?.Invoke());
        _buttonMinus.onClick.AddListener(() => OnMinusButtonPressed?.Invoke());
        _enterButton.onClick.AddListener(() => OnEnterButton?.Invoke());
        _minValue.onEndEdit.AddListener(delegate { EndEditMinValue(); });
        _maxValue.onEndEdit.AddListener(delegate { EndEditMaxValue(); });
        _smallBlindInput.onEndEdit.AddListener(delegate { EndEditSmallBlind(); });
        _bigBlindInput.onEndEdit.AddListener(delegate { EndEditBigBlind(); });
    }

    private void Start()
    {
        _playerData = PlayerData.Instance;
        _gameModeSettings = GameModeSettings.Instance;
        _gameModeSettings.maxMoneyGet = PlayerData.Instance.money;
        SwitchMode(null);
    }

    private void SwitchMode(Dropdown dropdown)
    {
        int gameModeIndex = 0;
        if (dropdown != null)
            gameModeIndex = dropdown.value;
        _minValue.text = $"${ _gameModeSettings.minMoneyGet}";
        _moneyScroll.minValue = _gameModeSettings.minMoneyGet;
        _maxValue.text = $"${ _gameModeSettings.maxMoneyGet}";
        _moneyScroll.maxValue = _gameModeSettings.maxMoneyGet;
        _bigBlindInput.text = $"${_gameModeSettings.bigBlind}";
        _smallBlindInput.text = $"${_gameModeSettings.smallBlind}";
        _countPeopleText.text = $"{_gameModeSettings.countPeople}";
        _buyInAllText.text = _playerData.money < _gameModeSettings.minMoneyGet ? "YOU DON`T HAVE ENOUGH MONEY" : "BUY-IN";
        switch ((GameModes)gameModeIndex)
        {
            case GameModes.Joker:
                _descriptionText.text = "JOKER POKER";
                _background.sprite = _backgroundsSprites[2];
                _gameModeSettings.gameMode = GameModes.Joker;
                break;
            case GameModes.Royal:
                _descriptionText.text = "ROYAL POKER";
                _background.sprite = _backgroundsSprites[1];
                _gameModeSettings.gameMode = GameModes.Royal;
                break;
            case GameModes.lowball:
                _descriptionText.text = "LOWBALL POKER";
                _background.sprite = _backgroundsSprites[0];
                _gameModeSettings.gameMode = GameModes.lowball;
                break;
            case GameModes.Texas:
                _descriptionText.text = "TEXAS POKER";
                _background.sprite = _backgroundsSprites[0];
                _gameModeSettings.gameMode = GameModes.Texas;
                break;
            case GameModes.Dash:
                _descriptionText.text = "DASH POKER";
                _background.sprite = _backgroundsSprites[0];
                _gameModeSettings.gameMode = GameModes.Dash;
                break;
        }
    }

    private void EndEditMinValue()
    {
        var maxValue = _maxValue.text.Remove(_maxValue.text.Length - 1);
        if (Convert.ToInt32(_minValue.text) >= Convert.ToInt32(maxValue))
            _minValue.text = (Convert.ToInt32(maxValue) - 1).ToString();
        _gameModeSettings.minMoneyGet = Convert.ToInt32(_minValue.text);
        _moneyScroll.minValue = Convert.ToInt32(_minValue.text);
        _minValue.text = "$" + _minValue.text;
    }

    private void EndEditMaxValue()
    {
        var minValue = _minValue.text.Remove(_minValue.text.Length - 1);
        if (Convert.ToInt32(minValue) >= Convert.ToInt32(_maxValue.text))
            _maxValue.text = (Convert.ToInt32(minValue) + 1).ToString();
        _gameModeSettings.maxMoneyGet = Convert.ToInt32(_maxValue.text);
        _moneyScroll.maxValue = Convert.ToInt32(_maxValue.text);
        _maxValue.text = "$" + _maxValue.text;
    }

    private void EndEditSmallBlind()
    {
        var bigBlindValue = _bigBlindInput.text.Remove(_bigBlindInput.text.Length - 1);
        if (Convert.ToInt32(_smallBlindInput.text) >= Convert.ToInt32(bigBlindValue))
            _smallBlindInput.text = (Convert.ToInt32(bigBlindValue) - 1).ToString();
        _gameModeSettings.smallBlind = Convert.ToInt32(_smallBlindInput.text);
        _smallBlindInput.text = "$" + _smallBlindInput.text;

    }

    private void EndEditBigBlind()
    {
        var smallBlindValue = _smallBlindInput.text.Remove(_smallBlindInput.text.Length - 1);
        if (Convert.ToInt32(smallBlindValue) >= Convert.ToInt32(_bigBlindInput.text))
        {
            _bigBlindInput.text = "$" + (Convert.ToInt32(smallBlindValue) + 1).ToString();
        }
        _gameModeSettings.bigBlind = Convert.ToInt32(_bigBlindInput.text);
    }

    public void UpdateMoneyScrollValue(int value) => _moneyScroll.value = value;

    public void PlusButton(int value) => _moneyScroll.value += value;

    public void MinusButton(int value) => _moneyScroll.value -= value;

    public void UpdateHowMuchMoneyText(string newString) => _howMuchMoneyText.text = newString;
}