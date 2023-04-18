using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MoneyWindowView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Text _howMuchMoneyText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Button _closeWindowWithMoney;
    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _buttonPlus;
    [SerializeField] private Button _buttonMinus;
    [SerializeField] private Button _autoTopButton;
    [SerializeField] private Button _autoTopInfoButton;
    [SerializeField] private Image _autoTopImage;
    [SerializeField] private Slider _moneyScroll;
    [SerializeField] private Text _buyInAllText;
    [SerializeField] private Text _minValue;
    [SerializeField] private Text _maxValue;
    [SerializeField] private Text _countPeople;
    [SerializeField] private Text _blindsCountText;
    [SerializeField] private Text _blindsText;
    [SerializeField] private Text _letsGoText;
    [SerializeField] private Text _autoTopText;
    [SerializeField] private List<Sprite> _backgroundsSprites;
    public UnityAction<float> OnChangedScrollValue { get; set; }
    public event Action OnPlusButtonPressed;
    public event Action OnMinusButtonPressed;
    public event Action OnEnterButton;
    public event Action OnCloseButtonPressed;
    public event Action OnAutoTopPressed;

    private void Awake()
    {
        _closeWindowWithMoney.onClick.AddListener(() => OnCloseButtonPressed.Invoke());
        _moneyScroll.onValueChanged.AddListener(OnChangedScrollValue);
        _buttonPlus.onClick.AddListener(() => OnPlusButtonPressed.Invoke());
        _buttonMinus.onClick.AddListener(() => OnMinusButtonPressed.Invoke());
        _enterButton.onClick.AddListener(() => OnEnterButton.Invoke());
        _autoTopButton.onClick.AddListener(() => OnAutoTopPressed.Invoke());
    }

    private void OnEnable()
    {
        switch (GameModeSettings.Instance.TablesInWorlds)
        {
            case TablesInWorlds.Dash300K:
            case TablesInWorlds.Dash:
                _moneyScroll.value = 0;
                UpdateBlinds();
                break;
            case TablesInWorlds.Dash1M:
                _moneyScroll.value = 1;
                UpdateBlinds();
                break;
            case TablesInWorlds.Dash10M:
                _moneyScroll.value = 2;
                UpdateBlinds();
                break;
            default:
                _moneyScroll.value = _moneyScroll.minValue;
                break;
                //case TablesInWorlds.Dash100M:
                //    _moneyScroll.value = 3;
                //    break;
        }
        if (GameModeSettings.Instance.gameMode != GameModes.Dash && GameModeSettings.Instance.gameMode != GameModes.lowball)
            UpdateHowMuchMoneyText($"${((int)_moneyScroll.minValue).IntoCluttered()}");
        SetAutoTop();
    }

    private void HideSliderElements(bool state)
    {
        _moneyScroll.gameObject.SetActive(!state);
        _minValue.gameObject.SetActive(!state);
        _maxValue.gameObject.SetActive(!state);
        _buttonPlus.gameObject.SetActive(!state);
        _buttonMinus.gameObject.SetActive(!state);
    }

    private void HideMinNMaxTexts(bool state)
    {
        _minValue.gameObject.SetActive(!state);
        _maxValue.gameObject.SetActive(!state);
    }

    private void HideAutoTop(bool state)
    {
        _autoTopButton.gameObject.SetActive(!state);
        _autoTopText.gameObject.SetActive(!state);
        _autoTopInfoButton.gameObject.SetActive(!state);
    }

    public void UpdateAllText(PlayerProfileData playerData, GameModeSettings gameModeSettings)
    {
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
        {
            HideMinNMaxTexts(true);
            HideAutoTop(true);
            _moneyScroll.gameObject.SetActive(true);
            _minValue.text = $"${ gameModeSettings.minMoneyGet.IntoCluttered()}";
            _moneyScroll.minValue = 0;

            _maxValue.text = $"${gameModeSettings.maxMoneyGet.IntoCluttered()}";
            _moneyScroll.maxValue = 3;
        }
        else
        {
            HideSliderElements(gameModeSettings.minMoneyGet == gameModeSettings.maxMoneyGet);
            HideAutoTop(false);
            _minValue.text = $"${ gameModeSettings.minMoneyGet.IntoCluttered()}";
            _moneyScroll.minValue = gameModeSettings.minMoneyGet;
            _maxValue.text = $"${gameModeSettings.maxMoneyGet.IntoCluttered()}";
            _moneyScroll.maxValue = gameModeSettings.maxMoneyGet;
        }
        _autoTopText.text = _autoTopImage.rectTransform.anchoredPosition.x < 0 
            ? SettingsManager.Instance.GetString("WindowWithMoney.AutoTopOff") 
            : SettingsManager.Instance.GetString("WindowWithMoney.AutoTopOn");
        _blindsCountText.text = $"${gameModeSettings.smallBlind.IntoCluttered()} / ${gameModeSettings.bigBlind.IntoCluttered()}";
        _countPeople.text = $"{gameModeSettings.countPeople}";
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        _buyInAllText.text = playerData.TotalMoney + playerStackMoney < gameModeSettings.minMoneyGet
            ? SettingsManager.Instance.GetString("WindowWithMoney.NotEnoughMoney")
            : SettingsManager.Instance.GetString("WindowWithMoney.BuyIn");
        switch (gameModeSettings.gameMode)
        {
            case GameModes.Joker:
                _descriptionText.text = SettingsManager.Instance.GetString("WindowWithMoney.JokerPoker");
                _background.sprite = _backgroundsSprites[2];
                break;
            case GameModes.Royal:
                _descriptionText.text = SettingsManager.Instance.GetString("WindowWithMoney.RoyalPoker");
                _background.sprite = _backgroundsSprites[1];
                break;
            case GameModes.lowball:
                _descriptionText.text = SettingsManager.Instance.GetString("WindowWithMoney.LowballPoker");
                _background.sprite = _backgroundsSprites[0];
                break;
            case GameModes.Texas:
                _descriptionText.text = SettingsManager.Instance.GetString("WindowWithMoney.TexasPoker");
                _background.sprite = _backgroundsSprites[0];
                break;
            case GameModes.Dash:
                _descriptionText.text = SettingsManager.Instance.GetString("WindowWithMoney.DashPoker");
                _background.sprite = _backgroundsSprites[0];
                break;
        }
        SetWindowTexts();
    }

    public void UpdateBlinds()
    {
        if (_moneyScroll.value >= 0 && _moneyScroll.value < 1)
        {
            GameModeSettings.Instance.smallBlind = 7500;
            GameModeSettings.Instance.bigBlind = 15000;
            GameModeSettings.Instance.minMoneyGet = 300000;
            GameModeSettings.Instance.maxMoneyGet = 300000;
            PlayerProfileData.Instance.playerGetMoney = 300000;
        }
        else if (_moneyScroll.value >= 1 && _moneyScroll.value < 2)
        {
            GameModeSettings.Instance.smallBlind = 25000;
            GameModeSettings.Instance.bigBlind = 50000;
            GameModeSettings.Instance.minMoneyGet = 1000000;
            GameModeSettings.Instance.maxMoneyGet = 1000000;
            PlayerProfileData.Instance.playerGetMoney = 1000000;
        }
        else if (_moneyScroll.value >= 2 && _moneyScroll.value < 3)
        {
            GameModeSettings.Instance.smallBlind = 150000;
            GameModeSettings.Instance.bigBlind = 300000;
            GameModeSettings.Instance.minMoneyGet = 10000000;
            GameModeSettings.Instance.maxMoneyGet = 10000000;
            PlayerProfileData.Instance.playerGetMoney = 10000000;
        }
        else if (_moneyScroll.value >= 3 && _moneyScroll.value < 4)
        {
            GameModeSettings.Instance.smallBlind = 2000000;
            GameModeSettings.Instance.bigBlind = 4000000;
            GameModeSettings.Instance.minMoneyGet = 100000000;
            GameModeSettings.Instance.maxMoneyGet = 100000000;
            PlayerProfileData.Instance.playerGetMoney = 100000000;
        }
        else if (_moneyScroll.value >= 4 && _moneyScroll.value < 5)
        {
            GameModeSettings.Instance.smallBlind = 150000;
            GameModeSettings.Instance.bigBlind = 300000;
            GameModeSettings.Instance.minMoneyGet = 10000000;
            GameModeSettings.Instance.maxMoneyGet = 10000000;
            PlayerProfileData.Instance.playerGetMoney = 10000000;
        }
        else if (_moneyScroll.value >= 5)
        {
            GameModeSettings.Instance.smallBlind = 2000000;
            GameModeSettings.Instance.bigBlind = 4000000;
            GameModeSettings.Instance.minMoneyGet = 100000000;
            GameModeSettings.Instance.maxMoneyGet = 100000000;
            PlayerProfileData.Instance.playerGetMoney = 100000000;
        }
        UpdateHowMuchMoneyText("$" + PlayerProfileData.Instance.playerGetMoney.IntoCluttered());
        UpdateAllText(PlayerProfileData.Instance, GameModeSettings.Instance);
    }

    public void UpdateMoneyScrollValue(int value) => _moneyScroll.value = value;

    public void PlusButton(int value) => _moneyScroll.value += value;

    public void MinusButton(int value) => _moneyScroll.value -= value;

    public void UpdateHowMuchMoneyText(string newString) => _howMuchMoneyText.text = newString;

    public void SetAutoTop()
    {
        var newAnchorX = GameModeSettings.Instance.autoTop
            ? Mathf.Abs(_autoTopImage.rectTransform.anchoredPosition.x)
            : -Mathf.Abs(_autoTopImage.rectTransform.anchoredPosition.x);
        _autoTopImage.rectTransform.anchoredPosition = new Vector2(newAnchorX, _autoTopImage.rectTransform.anchoredPosition.y);
        _autoTopText.text = _autoTopImage.rectTransform.anchoredPosition.x < 0
            ? SettingsManager.Instance.GetString(GameConstants.AutoTopOff)
            : SettingsManager.Instance.GetString(GameConstants.AutoTopOn);
    }

    public void SetWindowTexts()
    {
        _blindsText.text = SettingsManager.Instance.GetString("WindowWithMoney.Blinds");
        _letsGoText.text = SettingsManager.Instance.GetString("WindowWithMoney.LetsGo");
    }
}