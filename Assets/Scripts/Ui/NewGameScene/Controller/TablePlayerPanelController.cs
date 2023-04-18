using System;
using UnityEngine;

public class TablePlayerPanelController : MonoBehaviour
{
    [SerializeField] private TablePlayerPanelView _playerPanelView;
    public event Action OnSettingButtonClick;
    public event Action OnShopButtonClick;

    private void OnEnable()
    {
        UpdateTexts();
        _playerPanelView.OnSettingButtonClick += SettingButtonClick;
        _playerPanelView.OnShopButtonClick += ShopButtonClick;
    }

    private void OnDisable()
    {
        _playerPanelView.OnSettingButtonClick -= SettingButtonClick;
        _playerPanelView.OnShopButtonClick -= ShopButtonClick;
    }

    private void ShopButtonClick() => OnShopButtonClick?.Invoke();

    private void SettingButtonClick() => OnSettingButtonClick?.Invoke();

    public void UpdateTexts()
    {
        _playerPanelView.SetMoneyText($"${PlayerData.Instance?.money}");
        _playerPanelView.SetBigCoinText($"{PlayerData.Instance?.bitcoins}");
    }
}
