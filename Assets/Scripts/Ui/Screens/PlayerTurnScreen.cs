using UnityEngine;
using UnityEngine.UI;
using static Client;
using System.Linq;

public class PlayerTurnScreen : UIScreen
{
    [SerializeField] private TableManager _tableManager;
    [SerializeField] private GameObject missClickArea;
    [Header("Buttons"), SerializeField] private Button _checkButton;
    [SerializeField] private Button _foldButton;
    [SerializeField] private Button _raiseButton;
    [Header("Raise button"), SerializeField] private Button _blind2Button;
    [SerializeField] private Button _blind6Button;
    [SerializeField] private Button _bankButton;
    [SerializeField] private Button _allInButton;
    [Header("Raise Slider"), SerializeField] private GameObject _sliderGameObject;
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _sliderPlayerMoneyCountText;
    [Header("Texts"), SerializeField] private Text _foldText;
    [SerializeField] private Text _raiseText;
    [SerializeField] private Text _allInText;
    [SerializeField] private Text _bankText;
    [SerializeField] private Text _blindX6Text;
    [SerializeField] private Text _blindX2Text;
    private Text _textCheckButton;
    private bool _isOpenBetScreen;
    private bool _isSetData;
    private long _bankCount;
    private float _lastAmount;
    private int _bet;

    private void OnEnable()
    {
        _isOpenBetScreen = false;
        _slider.value = 0;
        _sliderGameObject.SetActive(false);
        MP_UpdateButtonsStates();
    }

    private void OnDisable() => missClickArea.SetActive(false);

    private void Awake()
    {
        _textCheckButton = _checkButton.GetComponentInChildren<Text>();
        _checkButton.onClick.AddListener(MP_CheckOrCall);
        _foldButton.onClick.AddListener(MP_Fold);
        _raiseButton.onClick.AddListener(MP_RaiseOrBet);
        _allInButton.onClick.AddListener(() => MP_SetRaiseChoise(MyPlayer.StackMoney));
        _bankButton.onClick.AddListener(() => MP_SetRaiseChoise(Client.TableData.Pot.TotalAmount));
        _blind2Button.onClick.AddListener(() => MP_SetRaiseChoise(Client.TableData.Pot.TotalAmount / 2));
        _blind6Button.onClick.AddListener(() => MP_SetRaiseChoise((int)(Client.TableData.Pot.TotalAmount * 0.75f)));
        _slider.onValueChanged.AddListener(ChangeBetAmount);
    }

    private void Start()
    {
        SettingsManager.Instance.UpdateTextsEvent += UpdateWindowTexts;
        OnActFoldByInactivePlayer += LockButtons;
        OnReceiveCurrentPlayerId += MP_SetCallCheckButtonText;
        UpdateWindowTexts();
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= UpdateWindowTexts;
        OnActFoldByInactivePlayer -= LockButtons;
        OnReceiveCurrentPlayerId -= MP_SetCallCheckButtonText;
    }

    private void MP_UpdateButtonsStates()
    {
        var betDelta = Client.TableData.CurrentMaxBet - MyPlayer.CurrentBet;
        var playerStack = MyPlayer.StackMoney;
        // Check button state (always available, if bet bigger then playerStack it will be sent as all-in)
        _checkButton.interactable = true;
        // Raise button state
        _raiseButton.interactable =
            playerStack > 0 &&
            betDelta < playerStack;
        // Slider state
        var sliderMaxValue = playerStack;
        var sliderMinValue =
            betDelta > Client.TableData.BigBlind
            ? betDelta
            : Client.TableData.BigBlind;
        _slider.maxValue = sliderMaxValue;
        _slider.minValue = sliderMinValue;
        _slider.value = sliderMinValue;
        _slider.interactable = playerStack > 0 && playerStack > sliderMinValue;
        // Bank 1/2 button state
        long amount_1_2 = IsMyFirstTurnInGame
            ? Client.TableData.BigBlind
            : Client.TableData.Pot.TotalAmount / 2;
        _blind2Button.interactable =
            playerStack >= amount_1_2 &&
            amount_1_2 >= betDelta &&
            amount_1_2 >= Client.TableData.BigBlind;
        // Bank 3/4 button state
        var amount_3_4 = (int)(Client.TableData.Pot.TotalAmount * 0.75f);
        _blind6Button.interactable =
            playerStack >= amount_3_4 &&
            amount_3_4 >= betDelta &&
            amount_3_4 >= Client.TableData.BigBlind;
        // Bank button state
        var pot = Client.TableData.Pot.TotalAmount;
        _bankButton.interactable =
            playerStack > 0 &&
            playerStack >= pot &&
            pot > 0 &&
            pot >= betDelta;
        // All-in button state
        _allInButton.interactable = playerStack > 0;
        // Fold button state (always available)
        _foldButton.interactable = true;
    }

    private void MP_RaiseOrBet()
    {
        if (!_isOpenBetScreen)
        {
            SetRaiseUIState(true);
            _sliderPlayerMoneyCountText.text = $"${((int)_slider.minValue).IntoCluttered()}";
        }
        else
        {
            SetRaiseUIState(false);
            if (!_slider.interactable)
                return;
            _tableManager.MP_MakeABetOrRaise(Mathf.RoundToInt(_slider.value));
            _lastAmount = _slider.value;
            LockButtons();
        }
    }

    public void SetRaiseUIState(bool newState) // Using also in miss click area gameobject to close the raise panel without any action
    {
        _isOpenBetScreen = newState;
        _sliderGameObject.SetActive(newState);
        missClickArea.SetActive(newState);
    }

    private void MP_CheckOrCall()
    {
        var bet = Client.TableData.CurrentMaxBet - MyPlayer.CurrentBet;
        if (bet == 0)
            _tableManager.MP_Check();
        else
            _tableManager.MP_Call(bet);
        LockButtons();
    }

    private void MP_Fold()
    {
        MyPlayer.PocketCards = null;
        switch(GameModeSettings.Instance.TablesInWorlds)
        {
            case TablesInWorlds.Dash:
            case TablesInWorlds.Dash300K:
            case TablesInWorlds.Dash1M:
            case TablesInWorlds.Dash10M:
            //case TablesInWorlds.Dash100M:
                PressedDashTable();
                return;
        }
        _tableManager.MP_Fold();
        LockButtons();
    }

    private void MP_SetRaiseChoise(long money)
    {
        _tableManager.MP_MakeABetOrRaise(money);
        SetRaiseUIState(false);
    }

    protected override void UpdateScreen(ScreenData dat) 
    {
        SetBankCountMoney(dat.Prize);
        UpdateWindowTexts();
        if (!_isSetData)
        {
            _isSetData = true;
            dat.OnPrizeChanged += SetBankCountMoney;
        }
    }    

    private void UpdateWindowTexts()
    {
        _foldText.text = SettingsManager.Instance.GetString("ActionOnTable.Fold");
        _raiseText.text = SettingsManager.Instance.GetString("ActionOnTable.Raise");
        _allInText.text = SettingsManager.Instance.GetString("ActionOnTable.AllIn");
        _bankText.text = SettingsManager.Instance.GetString("ActionOnTable.Bank");
        _blindX2Text.text = "1/2";
        _blindX6Text.text = "3/4";
        MP_SetCallCheckButtonText();
    }
    private void ChangeBetAmount(float amount) => _sliderPlayerMoneyCountText.text = $"{((int)amount).IntoCluttered()}";

    private void SetBankCountMoney(long Value) => _bankCount = Value;

    private void ChangeCheckButton(int amount)
    {
        _textCheckButton.text = amount > 0 ? amount == _lastAmount ? SettingsManager.Instance.GetString("ActionOnTable.Check") 
            : $"{SettingsManager.Instance.GetString("ActionOnTable.Call")}"// {amount}$" 
            : SettingsManager.Instance.GetString("ActionOnTable.Check");
        _bet = amount;
    }

    private void MP_SetCallCheckButtonText()
    {
        // [Optimization] ToDo: at now call on ever player turn changed, not only in my turn
        var amount = Client.TableData.CurrentMaxBet - MyPlayer.CurrentBet;
        if (amount <= 0)
            _textCheckButton.text = SettingsManager.Instance.GetString("ActionOnTable.Check");
        else if (amount >= MyPlayer.StackMoney)
            _textCheckButton.text = SettingsManager.Instance.GetString("ActionOnTable.AllIn");
        else
            _textCheckButton.text = $"{SettingsManager.Instance.GetString("ActionOnTable.Call")} ${amount.IntoCluttered()}";
        _raiseText.text = Client.TableData.CurrentMaxBet == 0
            ? SettingsManager.Instance.GetString("ActionOnTable.Bet")
            : SettingsManager.Instance.GetString("ActionOnTable.Raise");
    }

    private void LockButtons()
    {
        gameObject.SetActive(false);
        SetRaiseUIState(false);
        _checkButton.interactable = false;
        _foldButton.interactable = false;
        _raiseButton.interactable = false;
    }
}