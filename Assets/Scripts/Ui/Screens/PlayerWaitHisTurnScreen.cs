using PokerHand.Common.Helpers;
using UnityEngine;
using UnityEngine.UI;
using static Client;

public class PlayerWaitHisTurnScreen : UIScreen
{
    [Header("Buttons"), SerializeField] private Button _callButton;
    [SerializeField] private Button _foldButton;
    [SerializeField] private Button _callAnyButton;
    [Header("Images for Buttons"), SerializeField] private Sprite _deactiveSprite;
    [SerializeField] private Sprite _activeFoldSprite;
    [SerializeField] private Sprite _activeCallSprite;
    [SerializeField] private Sprite _activeCallAnySprite;
    private ScreenData _screenData;
    private Image _foldButtonImage;
    private Image _callButtonImage;
    private Image _callAnyButtonImage;
    [SerializeField] private Text _checkButtonText;
    [SerializeField] private Text _foldButtonText;
    [SerializeField] private Text _callAnyButtonText;
    private bool _isSetData;
    private bool _isFold;
    private bool _isCall;
    private bool _isCallAny;
    private bool _myPlayerCheck;

    private void OnEnable() => ResetButtons();

    private void Awake()
    {
        _callButton.onClick.AddListener(Call);
        _foldButton.onClick.AddListener(Fold);
        _callAnyButton.onClick.AddListener(CallAny);
        _foldButtonImage = _foldButton.GetComponent<Image>();
        _callButtonImage = _callButton.GetComponent<Image>();
        _callAnyButtonImage = _callAnyButton.GetComponent<Image>();
        _checkButtonText = _callButton.GetComponentInChildren<Text>();
        SettingsManager.Instance.UpdateTextsEvent += UpdateWindowTexts;
    }

    private void Start()
    {
        OnReceiveCurrentPlayerId += MP_UpdateScreen;
        OnActFoldByInactivePlayer += CloseWindow;
        MP_UpdateScreen();
        UpdateWindowTexts();
    }

    private void OnDestroy()
    {
        OnReceiveCurrentPlayerId -= MP_UpdateScreen;
        OnActFoldByInactivePlayer -= CloseWindow;
        SettingsManager.Instance.UpdateTextsEvent -= UpdateWindowTexts;
    }

    private void FullResetUIPlayerWait() => ResetButtons();

    private void ResetButtons()
    {
        _isFold = false;
        _isCall = false;
        _isCallAny = false;
        _foldButtonImage.sprite = _deactiveSprite;
        _callButtonImage.sprite = _deactiveSprite;
        _callAnyButtonImage.sprite = _deactiveSprite;
    }

    private void Fold()
    {
        _isFold = !_isFold;
        if (_isFold)
        {
            _isCall = false;
            _isCallAny = false;
            _callButtonImage.sprite = _deactiveSprite;
            _callAnyButtonImage.sprite = _deactiveSprite;
        }
        _foldButtonImage.sprite = _isFold ? _activeFoldSprite : _deactiveSprite;
        _screenData.PlayerSetPreAction = _isFold ? PlayerPreActionType.CheckFold : PlayerPreActionType.None;
    }

    private void Call()
    {
        _isCall = !_isCall;
        if (_isCall)
        {
            _isFold = false;
            _isCallAny = false;
            _foldButtonImage.sprite = _deactiveSprite;
            _callAnyButtonImage.sprite = _deactiveSprite;
        }
        _callButtonImage.sprite = _isCall ? _activeCallSprite : _deactiveSprite;
        if (!_isCall)
            _screenData.PlayerSetPreAction = PlayerPreActionType.None;
        else
        {
            var amount = Client.TableData.CurrentMaxBet - Client.MyPlayer.CurrentBet;
            if (amount == 0)
                _screenData.PlayerSetPreAction = PlayerPreActionType.Check;
            else
                _screenData.PlayerSetPreAction = PlayerPreActionType.CallCurrent;
        }
    }

    private void CallAny()
    {
        _isCallAny = !_isCallAny;
        if (_isCallAny)
        {
            _isFold = false;
            _isCall = false;
            _callButtonImage.sprite = _deactiveSprite;
            _foldButtonImage.sprite = _deactiveSprite;
        }

        _callAnyButtonImage.sprite = _isCallAny ? _activeCallAnySprite : _deactiveSprite;
        _screenData.PlayerSetPreAction = _isCallAny ? PlayerPreActionType.CallAny : PlayerPreActionType.None;
    }

    protected override void UpdateScreen(ScreenData dat)
    {
        if (!_isSetData)
        {
            _screenData = dat;
            _screenData.OnMyPlayerBetChangedEqual += MyPlayerCheck;
            _screenData.OnRestartGameUIAction += FullResetUIPlayerWait;
            _isSetData = true;
        }
        MyPlayerCheck(dat.MyPlayerBetEqual);
    }

    private void MyPlayerCheck(bool obj) => _myPlayerCheck = obj;

    private void UpdateWindowTexts()
    {
        var foldText = SettingsManager.Instance.GetString("ActionOnTable.Fold");
        var checkText = SettingsManager.Instance.GetString("ActionOnTable.Check");
        _foldButtonText.text = $"{checkText}/{foldText}";
        _checkButtonText.text = $"{SettingsManager.Instance.GetString("ActionOnTable.CallWaiting")}";
        _callAnyButtonText.text = SettingsManager.Instance.GetString("ActionOnTable.CallAny");
    }

    private void MP_UpdateScreen()
    {
        bool preactionsEnable = MyPlayer.PocketCards != null;
        if (Client.TableData.CurrentStage == RoundStageType.NotStarted ||
            MyPlayer.PocketCards == null ||
            MyPlayer.PocketCards.Count == 0)
        {
            CloseWindow();
            return;
        }
        var betDelta = Client.TableData.CurrentMaxBet - MyPlayer.CurrentBet;
        var playerStack = MyPlayer.StackMoney;
        var isCallAvailable = playerStack >= betDelta;
        if (!isCallAvailable &&
            _screenData != null &&
            _screenData.PlayerSetPreAction == PlayerPreActionType.CallCurrent)
            _screenData.PlayerSetPreAction = PlayerPreActionType.None;
        _callButton.interactable = isCallAvailable && preactionsEnable;
        // Call Any button
        _callAnyButton.interactable = (playerStack >= betDelta) && preactionsEnable;// && playerStack > 0);
        // Keeps pre actions for the next step also
        if (_screenData != null)
        {
            if (_screenData.PlayerSetPreAction == PlayerPreActionType.CheckFold)
            {
                _isFold = true;
                _foldButtonImage.sprite = _activeFoldSprite;
            }
            else if ((_screenData.PlayerSetPreAction == PlayerPreActionType.CallCurrent ||
                _screenData.PlayerSetPreAction == PlayerPreActionType.Check) &&
                _callButton.interactable)
            {
                _isCall = true;
                _callButtonImage.sprite = _activeCallSprite;
            }
            else if (_screenData.PlayerSetPreAction == PlayerPreActionType.CallAny &&
                _callAnyButton.interactable)
            {
                _isCallAny = true;
                _callAnyButtonImage.sprite = _activeCallAnySprite;
            }
        }
        // Check/Fold button
        _foldButton.interactable = preactionsEnable;
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}