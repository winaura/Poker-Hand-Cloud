using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowWithMoneyView : MonoBehaviour
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
    [SerializeField] private Button _tableInfoButton;
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
    public bool isContinue = false;
    public UnityAction<float> OnChangedScrollValue { get; set; }
    public event Action OnPlusButtonPressed;
    public event Action OnMinusButtonPressed;
    public event Action OnEnterButton;
    public event Action OnCloseButtonPressed;
    public event Action OnAutoTopPressed;
    public event Action OnAutoTopInfoPressed;
    public event Action OnTableInfoPressed;

    private void Awake()
    {
        _closeWindowWithMoney.onClick.AddListener(() => OnCloseButtonPressed.Invoke());
        _moneyScroll.onValueChanged.AddListener(OnChangedScrollValue);
        _buttonPlus.onClick.AddListener(() => OnPlusButtonPressed.Invoke());
        _buttonMinus.onClick.AddListener(() => OnMinusButtonPressed.Invoke());
        _enterButton.onClick.AddListener(() => OnEnterButton.Invoke());
        _autoTopButton.onClick.AddListener(() => OnAutoTopPressed.Invoke());
        _autoTopInfoButton.onClick.AddListener(() => OnAutoTopInfoPressed.Invoke());
        _tableInfoButton.onClick.AddListener(() => OnTableInfoPressed.Invoke());
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
        UpdateWindowElementState();
    }

    private void UpdateWindowElementState()
    {
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        if (PlayerProfileData.Instance.MinMoneyNeed > PlayerProfileData.Instance.TotalMoney + playerStackMoney)
        {
            _buttonPlus.interactable = false;
            _buttonMinus.interactable = false;
            _moneyScroll.interactable = false;
        }
        else
        {
            _buttonPlus.interactable = true;
            _buttonMinus.interactable = true;
            _moneyScroll.interactable = true;
        }
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
        {
            _minValue.gameObject.SetActive(false);
            _maxValue.gameObject.SetActive(false);
            _autoTopButton.gameObject.SetActive(false);
            _autoTopText.gameObject.SetActive(false);
            _moneyScroll.gameObject.SetActive(false);
            if (ReceiveFriendsDataFromServer._isConnectToTableById || isContinue)
            {
                _buttonPlus.gameObject.SetActive(false);
                _buttonMinus.gameObject.SetActive(false);
            }
            else
            {
                _buttonPlus.gameObject.SetActive(true);
                _buttonMinus.gameObject.SetActive(true);
            }
            return;
        }
        if (GameModeSettings.Instance.minMoneyGet == GameModeSettings.Instance.maxMoneyGet)
        {
            _minValue.gameObject.SetActive(false);
            _maxValue.gameObject.SetActive(false);
            _moneyScroll.gameObject.SetActive(false);
            _buttonPlus.gameObject.SetActive(false);
            _buttonMinus.gameObject.SetActive(false);
            _autoTopButton.gameObject.SetActive(true);
            _autoTopText.gameObject.SetActive(true);
            return;
        }
        _minValue.gameObject.SetActive(true);
        _maxValue.gameObject.SetActive(true);
        _moneyScroll.gameObject.SetActive(true);
        _buttonPlus.gameObject.SetActive(true);
        _buttonMinus.gameObject.SetActive(true);
        _autoTopButton.gameObject.SetActive(true);
        _autoTopText.gameObject.SetActive(true);
    }

    public int GetSelectedMoney()
    {
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
        {
            switch ((int)_moneyScroll.value)
            {
                case 0:
                    return 300_000;
                case 1:
                    return 1_000_000;
                case 2:
                    return 10_000_000;
                case 3:
                    return 100_000_000;
            }
            return 0;
        }
        else
            return (int)_moneyScroll.value;
    }

    public void UpdateAllText(PlayerProfileData playerData, GameModeSettings gameModeSettings)
    {
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
        {
            _minValue.text = $"${ gameModeSettings.minMoneyGet.IntoCluttered()}";
            _moneyScroll.minValue = 0;
            _maxValue.text = $"${gameModeSettings.maxMoneyGet.IntoCluttered()}";
            _moneyScroll.maxValue = 3;
        }
        else
        {
            _minValue.text = $"${ gameModeSettings.minMoneyGet.IntoCluttered()}";
            _moneyScroll.minValue = gameModeSettings.minMoneyGet;
            _maxValue.text = $"${gameModeSettings.maxMoneyGet.IntoCluttered()}";
            _moneyScroll.maxValue = gameModeSettings.maxMoneyGet;
        }
        _autoTopText.text = _autoTopImage.rectTransform.anchoredPosition.x < 0 ?
            SettingsManager.Instance.GetString("WindowWithMoney.AutoTopOff") : SettingsManager.Instance.GetString("WindowWithMoney.AutoTopOn");
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
        if (isContinue)
            _descriptionText.text = SettingsManager.Instance.GetString("WindowWithMoney.FixBlindTake");
        SetWindowTexts();
    }

    public void UpdateBlinds()
    {
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        if (_moneyScroll.value >= 0 && _moneyScroll.value < 1)
        {
            GameModeSettings.Instance.smallBlind = 7_500;
            GameModeSettings.Instance.bigBlind = 15_000;
            GameModeSettings.Instance.minMoneyGet = 300_000;
            GameModeSettings.Instance.maxMoneyGet = 300_000;
            PlayerProfileData.Instance.playerGetMoney = 300_000;
            PlayerProfileData.Instance.MinMoneyNeed = 300_000;
            GameModeSettings.Instance.TablesInWorlds = TablesInWorlds.Dash300K;
        }
        else if (_moneyScroll.value >= 1 && _moneyScroll.value < 2)
        {
            if (1_000_000 > PlayerProfileData.Instance.TotalMoney + playerStackMoney && !ReceiveFriendsDataFromServer._isConnectToTableById)
            {
                _moneyScroll.value -= 1;
                return;
            }
            GameModeSettings.Instance.smallBlind = 25_000;
            GameModeSettings.Instance.bigBlind = 50_000;
            GameModeSettings.Instance.minMoneyGet = 1_000_000;
            GameModeSettings.Instance.maxMoneyGet = 1_000_000;
            PlayerProfileData.Instance.playerGetMoney = 1_000_000;
            PlayerProfileData.Instance.MinMoneyNeed = 1_000_000;
            GameModeSettings.Instance.TablesInWorlds = TablesInWorlds.Dash1M;
        }
        else if (_moneyScroll.value >= 2 && _moneyScroll.value < 3)
        {
            if (10_000_000 > PlayerProfileData.Instance.TotalMoney + playerStackMoney && !ReceiveFriendsDataFromServer._isConnectToTableById)
            {
                _moneyScroll.value -= 1;
                return;
            }
            GameModeSettings.Instance.smallBlind = 150_000;
            GameModeSettings.Instance.bigBlind = 300_000;
            GameModeSettings.Instance.minMoneyGet = 10_000_000;
            GameModeSettings.Instance.maxMoneyGet = 10_000_000;
            PlayerProfileData.Instance.playerGetMoney = 10_000_000;
            PlayerProfileData.Instance.MinMoneyNeed = 10_000_000;
            GameModeSettings.Instance.TablesInWorlds = TablesInWorlds.Dash10M;
        }
        //else if (_moneyScroll.value >= 3 && _moneyScroll.value < 4)
        //{
        //    if (100_000_000 > PlayerProfileData.Instance.TotalMoney + playerStackMoney && !ReceiveFriendsDataFromServer._isConnectToTableById)
        //    {
        //        _moneyScroll.value -= 1;
        //        return;
        //    }
        //    GameModeSettings.Instance.smallBlind = 2_000_000;
        //    GameModeSettings.Instance.bigBlind = 4_000_000;
        //    GameModeSettings.Instance.minMoneyGet = 100_000_000;
        //    GameModeSettings.Instance.maxMoneyGet = 100_000_000;
        //    PlayerProfileData.Instance.playerGetMoney = 100_000_000;
        //    PlayerProfileData.Instance.MinMoneyNeed = 100_000_000;
        //    GameModeSettings.Instance.TablesInWorlds = TablesInWorlds.Dash100M;
        //}
        UpdateHowMuchMoneyText("$" + PlayerProfileData.Instance.playerGetMoney.IntoCluttered());
        UpdateAllText(PlayerProfileData.Instance, GameModeSettings.Instance);
    }

    public void UpdateMoneyScrollValue(long value) => _moneyScroll.value = value;

    public void PlusButton(long value) => _moneyScroll.value += value;

    public void MinusButton(long value) => _moneyScroll.value -= value;

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

    public void SetSliderValue(long value) => _moneyScroll.value = value;
}