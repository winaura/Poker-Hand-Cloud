using EnhancedScrollerDemos.SnappingDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundForClose;
    [SerializeField] private GameObject _mainLobby;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private List<GameObject> _locations;
    [Header("Player Panel"), SerializeField] private GameObject _playerInfoPanel;
    [SerializeField] private Text _playerLevelText;
    [SerializeField] private Text _playerMoneyText;
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Text _bigCoinText;
    [Header("Main Buttons"), SerializeField] private Button _shopButton;
    [SerializeField] private Button _moneyBoxButton;
    [SerializeField] private Button _ratingsButton;
    [SerializeField] private Button _friendsButton;
    [SerializeField] private Button _hiddenMenuButton;
    [SerializeField] private Button _openPlayerInfoButton;
    [Header("Hidden menu"), SerializeField] private GameObject _hiddenWindow;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _helpButton;
    [Header("Setting Window"), SerializeField] private GameObject _settingWindow;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _vibration;
    [SerializeField] private Button _handsButton;
    [SerializeField] private Button _hints;
    [SerializeField] private Button _settingsFriendsButton;
    [SerializeField] private List<Button> _languages;
    [Header("Window With Money"), SerializeField] private GameObject _windowWithMoney;
    [SerializeField] private Image _background;
    [SerializeField] private Text _howMuchMoneyText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Button _closeWindowWithMoney;
    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _buttonPlus;
    [SerializeField] private Button _buttonMinus;
    [SerializeField] private Slider _moneyScroll;
    [SerializeField] private Text _buyInAllText;
    [SerializeField] private Text _minValue;
    [SerializeField] private Text _maxValue;
    [SerializeField] private Text _countPeople;
    [SerializeField] private Text _blindsCountText;
    [SerializeField] private List<Sprite> _backgroundsSprites;
    [Header("Window With Money Sit n Go"), SerializeField] private GameObject _sitNGoWindow;
    [SerializeField] private Button _sitNGoGoToLevelButton;
    [SerializeField] private Button _closeWindowSitNGo;
    [SerializeField] private Text _buyinText;
    [SerializeField] private Text _buyinMoneyText;
    [SerializeField] private Text _firstPlacePrizeText;
    [SerializeField] private Text _secondPlacePrizeText;
    [SerializeField] private Text _blindingText;
    [SerializeField] private Text _startingChipsText;
    [Header("Shop"), SerializeField] private GameObject _scroller;
    [SerializeField] private GameObject _shopGameObject;
    [SerializeField] private Button _closeButton;
    [SerializeField] private List<Button> _shopButtons;
    [SerializeField] private List<Image> _buttonsImage;
    [SerializeField] private List<Sprite> _activeButtonImage;
    [SerializeField] private List<Sprite> _deactiveButtonImage;
    [SerializeField] private List<GameObject> _shopsGameObjects;
    [Header("Spin"), SerializeField] private SnappingDemo _snappingDemo;
    [SerializeField] private GameObject _slotScroller;
    [SerializeField] private List<GameObject> _spinGameObjects;
    [SerializeField] private List<Image> _spinImage;
    [SerializeField] private List<Sprite> _spinDeactive;
    [SerializeField] private List<Sprite> _spinActive;
    [SerializeField] private List<Button> _spinButtons;
    [SerializeField] private List<GameObject> _effectsPos;
    private List<bool> _isActiveButtonInMenu = new List<bool> { true, false, false };
    private List<Vector2> _startPosSpins = new List<Vector2>();
    private bool _readyToRoll;
    private bool _finishRoll;
    private PlayerData _playerData;
    private GameModeSettings _gameModeSettings;
    private RarityMenuButtons _rarity;
    Coroutine ActiveSpin;

    private void Start()
    {
        _playerData = PlayerData.Instance;
        _gameModeSettings = GameModeSettings.Instance;
        #region Texts
        _playerMoneyText.text = $"${_playerData.money}";
        _playerLevelText.text = $"Level {_playerData.level}";
        _playerNameText.text = $"{_playerData.nickname}";
        _bigCoinText.text = $"{_playerData.bitcoins}";
        #endregion
        #region Buttons
        _hiddenMenuButton.onClick.AddListener(OpenSettingsMenu);
        _settingsButton.onClick.AddListener(SetSettingWindow);
        _closeWindowWithMoney.onClick.AddListener(CloseAllWindow);
        _closeWindowWithMoney.onClick.AddListener(ResetMoneyWindow);
        _enterButton.onClick.AddListener(GoToLevel);
        _sitNGoGoToLevelButton.onClick.AddListener(GoToLevelSitnGo);
        _shopButton.onClick.AddListener(OpenShop);
        _closeButton.onClick.AddListener(DefaultShop);
        _closeButton.onClick.AddListener(CloseShop);
        _vibration.onClick.AddListener(VibrationCheck);
        _closeWindowSitNGo.onClick.AddListener(CloseAllWindow);
        _openPlayerInfoButton.onClick.AddListener(OpenPlayerInfo);
        _buttonPlus.onClick.AddListener(PlusButton);
        _buttonMinus.onClick.AddListener(MinusButton);
        #endregion
        CloseAllWindow();
        _shopGameObject.SetActive(false);
        _loadingScreen.SetActive(false);
        _moneyScroll.onValueChanged.AddListener(OnChangedScrollValue);
        for (int i = 0; i < _spinGameObjects.Count; i++)
            _startPosSpins.Add(_spinGameObjects[i].transform.localPosition);
        if (_gameModeSettings.world == Worlds.None)
            OpenMainLobby();
        else
            SetLocation((int)_gameModeSettings.world);
        DefaultShop();
        CloseShop();
        ChangeIconVibration();
        SetLanguage(0);
    }

    #region All Method On Buttons
    //This method you can find on language buttons
    public void SetLanguage(int localizationIndex)
    {
        for (int i = 0; i < _languages.Count; i++)
            _languages[i].transform.GetChild(0).gameObject.SetActive(false);
        _languages[localizationIndex].transform.GetChild(0).gameObject.SetActive(true);
    }

    //This method you can find on worlds buttons
    public void SetLocation(int index)
    {
        foreach (var location in _locations)
            location.SetActive(false);
        _mainLobby.SetActive(false);
        _locations[index].SetActive(true);
    }

    //This method you can find on Chips in Location buttons
    public void OpenWindowWithMoney()
    {
        _backgroundForClose.SetActive(true);
        if (_gameModeSettings.gameMode == GameModes.SitNGo)
            OpenSitNGoMoneyWindow();
        else
            OpenMoneyWindow();
    }

    //This method you can find on Chips in Location buttons
    public void SetGameMode(int enumIndexOfGameMode) => _gameModeSettings.gameMode = (GameModes)enumIndexOfGameMode;

    //This method you can find on shop Buttons
    public void ToggleWindow(int Index)
    {
        if (_isActiveButtonInMenu[Index]) 
            return;
        DisableAllInMenu();
        _buttonsImage[Index].sprite = _activeButtonImage[Index];
        _buttonsImage[Index].SetNativeSize();
        _isActiveButtonInMenu[Index] = true;
        _shopsGameObjects[Index].SetActive(true);
    }

    //This method you can find in shop on spin Buttons
    public void ToSpin(int index)
    {
        if (!_readyToRoll && !_finishRoll)
        {
            for (int i = 0; i < _buttonsImage.Count; i++)
            {
                _buttonsImage[i].gameObject.SetActive(false);
                _shopsGameObjects[i].SetActive(false);
            }
            for (int i = 0; i < _spinGameObjects.Count; i++)
                _spinGameObjects[i].gameObject.SetActive(false);
            _shopsGameObjects[_shopsGameObjects.Count - 1].SetActive(true);
            _spinGameObjects[index].gameObject.SetActive(true);
            _spinGameObjects[index].transform.localPosition = new Vector2(0, 110);
            _spinButtons[index].transform.GetChild(0).GetComponent<Text>().text = "$99.99";
            _readyToRoll = true;
            _rarity = (RarityMenuButtons)index;
            _snappingDemo.SetRarity(_rarity);
            _spinGameObjects[index].transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            _slotScroller.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
        else if (_readyToRoll && !_finishRoll)
        {
            _scroller.SetActive(true);
            _spinImage[index].sprite = _spinActive[index];
            _spinGameObjects[index].transform.GetChild(1).gameObject.SetActive(false);
            _spinGameObjects[index].transform.GetChild(2).gameObject.SetActive(false);
            _spinGameObjects[index].transform.GetChild(3).gameObject.SetActive(false);
            _spinGameObjects[index].transform.GetChild(4).gameObject.SetActive(false);
            _spinButtons[index].gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            ActiveSpin = StartCoroutine(SpinActive(index));
            _readyToRoll = false;
            _snappingDemo.PullLeverButton_OnClick();
        }
        else if (!_readyToRoll && _finishRoll)
        {
            StopCoroutine(ActiveSpin);
            DefaultShop();
            ToggleWindow(2);
        }
    }

    //This method on all back buttons in locations
    public void OpenMainLobby()
    {
        foreach (var location in _locations)
            location.SetActive(false);
        _mainLobby.SetActive(true);
    }
    #endregion

    public void CloseAllWindow()
    {
        _hiddenWindow.SetActive(false);
        _settingWindow.SetActive(false);
        _windowWithMoney.SetActive(false);
        _backgroundForClose.SetActive(false);
        _sitNGoWindow.SetActive(false);
        _playerInfoPanel.SetActive(false);
    }

    private void GoToLevel()
    {
        if (_playerData.playerGetMoney < _gameModeSettings.minMoneyGet) 
            return;
        _playerData.money -= _playerData.playerGetMoney;
        _loadingScreen.SetActive(true);
        SceneManager.LoadScene(1);
    }

    private void GoToLevelSitnGo()
    {
        if (_playerData.money < _gameModeSettings.minMoneyGet) 
            return;
        _playerData.playerGetMoney = _gameModeSettings.sitNGoMoneyGet;
        _playerData.money -= _gameModeSettings.minMoneyGet;
        _loadingScreen.SetActive(true);
        SceneManager.LoadScene(1);
    }

    private void OnChangedScrollValue(float value)
    {
        var newValue = Mathf.RoundToInt(value);
        if (value > _playerData.money)
        {
            _howMuchMoneyText.text = $"${_playerData.money}";
            _playerData.playerGetMoney = _playerData.money;
            _moneyScroll.value = _playerData.money;
            return;
        }
        _howMuchMoneyText.text = $"${(int)value}";
        _playerData.playerGetMoney = (int)value;
    }

    private void OpenHiddenMenu()
    {
        _hiddenWindow.SetActive(true);
        _backgroundForClose.SetActive(true);
    }

    private void OpenSettingsMenu()
    {
        _backgroundForClose.SetActive(true);
        SetSettingWindow();
    }

    private void DisableAllInMenu()
    {
        for (var i = 0; i < _shopButtons.Count; i++)
        {
            _buttonsImage[i].sprite = _deactiveButtonImage[i];
            _buttonsImage[i].SetNativeSize();
            _isActiveButtonInMenu[i] = false;
            _shopsGameObjects[i].SetActive(false);
        }
    }

    private void VibrationCheck()
    {
        _playerData.isVibrationOn = !_playerData.isVibrationOn;
        if (_playerData.isVibrationOn)
            Vibration.Vibrate(50);
        ChangeIconVibration();
    }

    private void ChangeIconVibration() => _vibration.GetComponent<Image>().color = _playerData.isVibrationOn ? Color.white : Color.grey;

    private void OpenMoneyWindow()
    {
        _minValue.text = $"${ _gameModeSettings.minMoneyGet}";
        _moneyScroll.minValue = _gameModeSettings.minMoneyGet;
        _maxValue.text = $"${_gameModeSettings.maxMoneyGet}";
        _moneyScroll.maxValue = _gameModeSettings.maxMoneyGet;
        _blindsCountText.text = $"${_gameModeSettings.smallBlind} / ${_gameModeSettings.bigBlind}";
        _countPeople.text = $"{_gameModeSettings.countPeople}";
        _buyInAllText.text = _playerData.money < _gameModeSettings.minMoneyGet ? "YOU DON`T HAVE ENOUGH MONEY" : "BUY-IN";
        switch (_gameModeSettings.gameMode)
        {
            case GameModes.Joker:
                _descriptionText.text = "JOKER POKER";
                _background.sprite = _backgroundsSprites[2];
                break;
            case GameModes.Royal:
                _descriptionText.text = "ROYAL POKER";
                _background.sprite = _backgroundsSprites[1];
                break;
            case GameModes.lowball:
                _descriptionText.text = "LOWBAL POKER";
                _background.sprite = _backgroundsSprites[0];
                break;
            case GameModes.Texas:
                _descriptionText.text = "TEXAS POKER";
                _background.sprite = _backgroundsSprites[0];
                break;
            case GameModes.Dash:
                _descriptionText.text = "DASH POKER";
                _background.sprite = _backgroundsSprites[0];
                break;
        }
        _windowWithMoney.SetActive(true);
    }

    private void OpenSitNGoMoneyWindow()
    {
        _buyinText.text = _playerData.money < _gameModeSettings.minMoneyGet ? "YOU DON`T HAVE ENOUGH MONEY" : "BUY-IN";
        _buyinMoneyText.text = $"${_gameModeSettings.minMoneyGet}";
        _blindingText.text = $"Blindings starting at {_gameModeSettings.smallBlind}/{_gameModeSettings.bigBlind}";
        _firstPlacePrizeText.text = $"${_gameModeSettings.maxPrizeSitNGo}";
        _secondPlacePrizeText.text = $"${_gameModeSettings.minPrizeSitNGo}";
        _startingChipsText.text += $"${_gameModeSettings.sitNGoMoneyGet}";
        _sitNGoWindow.SetActive(true);
    }

    private void DefaultShop()
    {
        for (int i = 0; i < _buttonsImage.Count; i++)
        {
            _buttonsImage[i].gameObject.SetActive(true);
            _buttonsImage[i].sprite = _deactiveButtonImage[i];
            _buttonsImage[i].SetNativeSize();
            _shopsGameObjects[i].SetActive(false);
            _isActiveButtonInMenu[i] = false;
        }
        for (int i = 0; i < _spinImage.Count; i++)
        {
            _spinImage[i].sprite = _spinDeactive[i];
            _spinImage[i].transform.GetChild(0).gameObject.SetActive(false);
            _spinImage[i].transform.GetChild(0).GetComponent<Text>().text = "";
            _spinGameObjects[i].transform.GetChild(1).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(2).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(3).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(3).GetComponent<Text>().text = "$8000";
            _spinGameObjects[i].transform.GetChild(4).gameObject.SetActive(true);
            _spinGameObjects[i].transform.GetChild(4).GetComponent<Text>().text = "WIN UP TO";
            _spinGameObjects[i].gameObject.SetActive(true);
            _spinGameObjects[i].transform.localPosition = _startPosSpins[i];
            _spinButtons[i].gameObject.SetActive(true);
            _spinButtons[i].transform.GetChild(0).GetComponent<Text>().text = "Let`s Roll";
            _spinGameObjects[i].transform.localScale = Vector3.one;
            _slotScroller.transform.localScale = Vector3.one;
        }
        _scroller.SetActive(false);
        _buttonsImage[0].sprite = _activeButtonImage[0];
        _buttonsImage[0].SetNativeSize();
        _shopsGameObjects[0].SetActive(true);
        _isActiveButtonInMenu[0] = true;
        _closeButton.gameObject.SetActive(true);
        _readyToRoll = false;
        _finishRoll = false;
    }
    private void ResetMoneyWindow()
    {
        _moneyScroll.minValue = 0;
        _moneyScroll.value = 0;
        _playerData.playerGetMoney = 0;
    }

    private void PlusButton() => _moneyScroll.value += 10;

    private void MinusButton() => _moneyScroll.value -= 10;

    private void OpenShop() => _shopGameObject.SetActive(true);

    private void CloseShop() => _shopGameObject.SetActive(false);

    private void OpenPlayerInfo() => _playerInfoPanel.SetActive(true);

    private void SetSettingWindow() => _settingWindow.SetActive(!_settingWindow.activeSelf);

    private IEnumerator SpinActive(int index)
    {
        yield return new WaitForSeconds(6.5f);
        _finishRoll = true;
        _spinButtons[index].gameObject.SetActive(true);
        _spinButtons[index].transform.GetChild(0).GetComponent<Text>().text = "Nice";
        _scroller.SetActive(false);
        _spinGameObjects[index].transform.GetChild(3).gameObject.SetActive(true);
        _spinGameObjects[index].transform.GetChild(4).gameObject.SetActive(true);
        _spinGameObjects[index].transform.GetChild(4).GetComponent<Text>().text = "You won ";
        _spinGameObjects[index].transform.GetChild(3).GetComponent<Text>().text = $"${_snappingDemo.GetScore()}";
        _spinImage[index].transform.GetChild(0).gameObject.SetActive(true);
        _spinImage[index].transform.GetChild(0).GetComponent<Text>().text = $"${_snappingDemo.GetScore()}";
        _playerData.money += _snappingDemo.GetScore();
        _playerMoneyText.text = $"${_playerData.money}";
        var counter = 0;
        while (counter < 3)
        {
            for (int i = 0; i < _effectsPos.Count; i++)
            {
                _effectsPos[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                yield return new WaitForSeconds(0.2f);
            }
            counter++;
            yield return new WaitForSeconds(1);
        }
    }
}