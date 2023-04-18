using System;
using UnityEngine;

public class LuckySpinController : MonoBehaviour
{
    [SerializeField] private LuckySpinWindowView _luckySpinWindowView;
    public event Action OnCloseButtonLuckySpinWindowClick;
    public event Action OnReRollButtonClick;
    public event Action OnSpinButtonClick;
    public event Action OnOpenSpinWindowButtonClick;

    private void OnEnable()
    {
        _luckySpinWindowView.OnCloseButtonLuckySpinWindowClick += OnCloseButtonLuckySpinWindow;
        _luckySpinWindowView.OnOpenSpinWindowButtonClick += OnOpenSpinWindowButton;
        _luckySpinWindowView.OnSpinButtonClick += OnSpinButton;
        SettingsManager.Instance.UpdateTextsEvent += _luckySpinWindowView.SetWindowTexts;
    }

    private void OnDisable()
    {
        _luckySpinWindowView.OnCloseButtonLuckySpinWindowClick -= OnCloseButtonLuckySpinWindow;
        _luckySpinWindowView.OnOpenSpinWindowButtonClick -= OnOpenSpinWindowButton;
        _luckySpinWindowView.OnSpinButtonClick -= OnSpinButton;
        SettingsManager.Instance.UpdateTextsEvent -= _luckySpinWindowView.SetWindowTexts;
    }
    private void OnOpenSpinWindowButton() => _luckySpinWindowView.OpenSpinWindow();

    private void OnSpinButton() => _luckySpinWindowView.RollSpin();

    private void OnCloseButtonLuckySpinWindow()
    {
        _luckySpinWindowView.CloseWindow();
        TryGetComponent(out PlayerPanelController playerPanelController);
        playerPanelController?.UpdateData();
    }
}
