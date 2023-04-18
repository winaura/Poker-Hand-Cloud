using System;
using System.Collections;
using UnityEngine;

public class WindowWithMoneyController : MonoBehaviour
{
    [SerializeField] private WindowWithMoneyView _windowWithMoneyView;
    [SerializeField] private TableManager _tableManager;
    private GameModeSettings _gameModeSettings;
    public enum WindowWithMoneyType
    {
        GoToTable,
        ContinueInTable
    }
    public event Action OnEnterPressed;

    private void Start()
    {
        StaticRuntimeSets.ItemsTypes.Add(typeof(WindowWithMoneyController), this);
        _gameModeSettings = GameModeSettings.Instance;
        _windowWithMoneyView.OnChangedScrollValue += OnChangedScrollValue;
        _windowWithMoneyView.OnMinusButtonPressed += MinusButtonPressed;
        _windowWithMoneyView.OnPlusButtonPressed += PlusButtonPressed;
        _windowWithMoneyView.OnAutoTopPressed += OnAutoTopButtonPressed;
        _windowWithMoneyView.OnAutoTopInfoPressed += OnAutoTopInfoPressed;
        _windowWithMoneyView.OnTableInfoPressed += OnTableInfoPressed;
        SettingsManager.Instance.UpdateTextsEvent += UpdateTexts;
    }

    private void OnDestroy()
    {
        StaticRuntimeSets.ItemsTypes.Remove(typeof(WindowWithMoneyController));
        _windowWithMoneyView.OnChangedScrollValue -= OnChangedScrollValue;
        _windowWithMoneyView.OnMinusButtonPressed -= MinusButtonPressed;
        _windowWithMoneyView.OnPlusButtonPressed -= PlusButtonPressed;
        _windowWithMoneyView.OnAutoTopPressed -= OnAutoTopButtonPressed;
        SettingsManager.Instance.UpdateTextsEvent -= UpdateTexts;
        _windowWithMoneyView.OnAutoTopInfoPressed -= OnAutoTopInfoPressed;
        _windowWithMoneyView.OnTableInfoPressed -= OnTableInfoPressed;
    }

    private void OnAutoTopInfoPressed() => NotificationController.ShowNotification(NotificationController.NotificationType.AutoTopInfo);

    private void OnTableInfoPressed() => NotificationController.ShowNotification(GameModeSettings.Instance.gameMode);

    private void OnAutoTopButtonPressed()
    {
        _gameModeSettings.autoTop = !_gameModeSettings.autoTop;
        _windowWithMoneyView.SetAutoTop();
    }
    private void OnCloseButtonPressedGoToTable()
    {
        _windowWithMoneyView.OnEnterButton -= EnterButtonPressedGoToTable;
        _windowWithMoneyView.OnCloseButtonPressed -= OnCloseButtonPressedGoToTable;
        _windowWithMoneyView.gameObject.SetActive(false);
    }

    private void EnterButtonPressedGoToTable()
    {
        _windowWithMoneyView.OnEnterButton -= EnterButtonPressedGoToTable;
        _windowWithMoneyView.OnCloseButtonPressed -= OnCloseButtonPressedGoToTable;
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        if (PlayerProfileData.Instance.MinMoneyNeed > PlayerProfileData.Instance.TotalMoney + playerStackMoney)
        {
            ShopController _shopController = FindObjectOfType<ShopController>();
            _shopController.OpenWindow();
            _windowWithMoneyView.gameObject.SetActive(false);
            return;
        }
        ConnectingWindow.CreateWindow(ConnectingType.ConnectingToTable);
        OnEnterPressed?.Invoke();
    }

    private void OnCloseButtonPressedContinueInTable()
    {
        _windowWithMoneyView.OnEnterButton -= EnterButtonPressedContinueInTable;
        _windowWithMoneyView.OnCloseButtonPressed -= OnCloseButtonPressedContinueInTable;
        _tableManager.LeaveTable();
        _windowWithMoneyView.gameObject.SetActive(false);
    }

    private void EnterButtonPressedContinueInTable()
    {
        StopAllCoroutines();
        _windowWithMoneyView.OnEnterButton -= EnterButtonPressedContinueInTable;
        _windowWithMoneyView.OnCloseButtonPressed -= OnCloseButtonPressedContinueInTable;
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        if (PlayerProfileData.Instance.MinMoneyNeed > PlayerProfileData.Instance.TotalMoney + playerStackMoney)
        {
            _tableManager.LeaveTable();
            return;
        }
        Client.SendAddStackMoney(_windowWithMoneyView.GetSelectedMoney());
        _windowWithMoneyView.gameObject.SetActive(false);
    }

    private void PlusButtonPressed()
    {
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        long newValue = GameModeSettings.Instance.smallBlind;
        if (newValue > PlayerProfileData.Instance.TotalMoney + playerStackMoney)
            newValue = PlayerProfileData.Instance.TotalMoney + playerStackMoney;
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
            newValue = 1;
        _windowWithMoneyView.PlusButton(newValue);
    }

    private void MinusButtonPressed()
    {
        int newValue = GameModeSettings.Instance.smallBlind;
        if (newValue < 0)
            newValue = 0;
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
            newValue = 1;
        _windowWithMoneyView.MinusButton(newValue);
    }

    private void OnChangedScrollValue(float value)
    {
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        var newValue = Mathf.RoundToInt(value);
        if (GameModeSettings.Instance.gameMode == GameModes.Dash || GameModeSettings.Instance.gameMode == GameModes.lowball)
        {
            _windowWithMoneyView.UpdateBlinds();
            return;
        }
        if (newValue > PlayerProfileData.Instance.TotalMoney + playerStackMoney)
        {
            SetUIDataToTotalMoney();
            return;
        }
        _windowWithMoneyView.UpdateHowMuchMoneyText($"${newValue.IntoCluttered()}");
        PlayerProfileData.Instance.playerGetMoney = newValue;
    }

    private void SetUIDataToTotalMoney()
    {
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        long totalMoney = PlayerProfileData.Instance.TotalMoney + playerStackMoney;
        _windowWithMoneyView.UpdateHowMuchMoneyText($"${totalMoney.IntoCluttered()}");
        PlayerProfileData.Instance.playerGetMoney = totalMoney;
        _windowWithMoneyView.UpdateMoneyScrollValue(totalMoney);
    }

    public void UpdateTexts() => _windowWithMoneyView.UpdateAllText(PlayerProfileData.Instance, _gameModeSettings);

    public void OpenWindow(WindowWithMoneyType windowWithMoneyType)
    {
        switch(windowWithMoneyType)
        {
            case WindowWithMoneyType.ContinueInTable:
                _windowWithMoneyView.OnEnterButton += EnterButtonPressedContinueInTable;
                _windowWithMoneyView.OnCloseButtonPressed += OnCloseButtonPressedContinueInTable;
                _windowWithMoneyView.isContinue = true;
                StartCoroutine(ContinueRoutine());
                break;
            case WindowWithMoneyType.GoToTable:
                _windowWithMoneyView.OnEnterButton += EnterButtonPressedGoToTable;
                _windowWithMoneyView.OnCloseButtonPressed += OnCloseButtonPressedGoToTable;
                _windowWithMoneyView.isContinue = false;
                break;
        }
        PlayerProfileData.Instance.playerGetMoney = _gameModeSettings.minMoneyGet;
        _windowWithMoneyView.UpdateAllText(PlayerProfileData.Instance, _gameModeSettings);
        _windowWithMoneyView.gameObject.SetActive(true);
    }

    IEnumerator ContinueRoutine()
    {
        yield return GameConstants.WaitSeconds_WindowWithMoneyTime;
        _tableManager.LeaveTable();
    }

    public void SetSliderValue(long value) => _windowWithMoneyView.SetSliderValue(value);
}