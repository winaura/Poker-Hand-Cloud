using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateTableEditorWindowController : MonoBehaviour
{
    [SerializeField] private PrivateTableEditorView _privateTableEditorView;
    public event Action OnChangePlayerMoney;
    public event Action OnEnterPressed;
    private PlayerData _playerData;

    private void Start()
    {
        _playerData = PlayerData.Instance;
        _privateTableEditorView.OnChangedScrollValue += OnChangedScrollValue;
        _privateTableEditorView.OnMinusButtonPressed += MinusButtonPressed;
        _privateTableEditorView.OnPlusButtonPressed += PlusButtonPressed;
        _privateTableEditorView.OnEnterButton += EnterButtonPressed;
        _privateTableEditorView.OnCloseButtonPressed += OnCloseButtonPressed;
    }

    private void OnDestroy()
    {
        _privateTableEditorView.OnChangedScrollValue -= OnChangedScrollValue;
        _privateTableEditorView.OnMinusButtonPressed -= MinusButtonPressed;
        _privateTableEditorView.OnPlusButtonPressed -= PlusButtonPressed;
        _privateTableEditorView.OnEnterButton -= EnterButtonPressed;
        _privateTableEditorView.OnCloseButtonPressed -= OnCloseButtonPressed;
    }

    public void ActivetePrivateTableEditorView() => _privateTableEditorView.gameObject.SetActive(true);

    private void OnCloseButtonPressed()
    {
        _privateTableEditorView.gameObject.SetActive(false);
        _playerData.playerGetMoney = 0;
    }

    private void EnterButtonPressed()
    {
        OnEnterPressed?.Invoke();
        OnChangePlayerMoney?.Invoke();
    }

    private void PlusButtonPressed()
    {
        int newValue = 10;
        if (newValue > _playerData.money) 
            newValue = _playerData.money;
        _privateTableEditorView.PlusButton(newValue);
    }

    private void MinusButtonPressed()
    {
        int newValue = 10;
        if (newValue < 0) newValue = 0;
        _privateTableEditorView.MinusButton(newValue);
    }

    private void OnChangedScrollValue(float value)
    {
        int newValue = Mathf.RoundToInt(value);
        UpdateScrollData(newValue);
    }

    private void UpdateScrollData(int newValue)
    {
        if (newValue > _playerData.money)
        {
            _privateTableEditorView.UpdateHowMuchMoneyText($"${_playerData.money}");
            _playerData.playerGetMoney = _playerData.money;
            _privateTableEditorView.UpdateMoneyScrollValue(_playerData.money);
            return;
        }
        _privateTableEditorView.UpdateHowMuchMoneyText($"${newValue}");
        _playerData.playerGetMoney = newValue;
    }
}