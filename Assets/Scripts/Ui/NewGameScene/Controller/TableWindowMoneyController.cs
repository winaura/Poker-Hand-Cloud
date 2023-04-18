using System;
using UnityEngine;

public class TableWindowMoneyController : MonoBehaviour
{
    [SerializeField] private TableWindowMoneyView _view;
    private GameModeSettings _gameModeSettings;
    private PlayerData _playerData;
    public event Action OnCloseButton;

    private void Awake()
    {
        _gameModeSettings = GameModeSettings.Instance;
        _playerData = PlayerData.Instance;
    }

    private void OnEnable()
    {
        _view.OnMinusButtonPressed += MinusButtonPressed;
        _view.OnPlusButtonPressed += PlusButtonPressed;
        _view.OnEnterButton += EnterButtonPressed;
        _view.OnCloseButtonPressed += OnCloseButtonPressed;
    }

    private void OnDisable()
    {
        _view.OnMinusButtonPressed -= MinusButtonPressed;
        _view.OnPlusButtonPressed -= PlusButtonPressed;
        _view.OnEnterButton -= EnterButtonPressed;
        _view.OnCloseButtonPressed -= OnCloseButtonPressed;
    }

    private void OnCloseButtonPressed()
    {
        _view.gameObject.SetActive(false);
        OnCloseButton?.Invoke();
    }

    private void EnterButtonPressed() => _view.gameObject.SetActive(false);

    private void PlusButtonPressed() => _view.PlusButton(5);

    private void MinusButtonPressed() => _view.MinusButton(5);

    private void UpdateDiscriptionText()
    {
        if (_gameModeSettings.bigBlind > _playerData.money)        
            _view.UpdateDiscriptionText($"You must get more then Big Blind ${_gameModeSettings.bigBlind}");        
    }

    public void OpenWindow()
    {
        if (_gameModeSettings.bigBlind > _playerData.money)
        {
            _view.gameObject.SetActive(true);
            _view.UpdateMoneyScrollValue(10, 50);
            _view.MinMoneyTextUpdate(_gameModeSettings == null ? "0" : $"${_gameModeSettings.bigBlind}");
            _view.MaxMoneyTextUpdate(_playerData == null ? "0" : $"${_playerData.money}");
        }
        else
            OnCloseButtonPressed();
    }
}