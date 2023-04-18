using System;
using UnityEngine;
using UnityEngine.UI;

public class TablePlayerPanelView : MonoBehaviour
{
    [SerializeField] private Text _playerMoneyText;
    [SerializeField] private Text _bigCoinText;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _shopButton;
    public event Action OnSettingButtonClick;
    public event Action OnShopButtonClick;

    private void Awake()
    {
        _settingsButton.onClick.AddListener(() => OnSettingButtonClick?.Invoke());
        _shopButton.onClick.AddListener(() => OnShopButtonClick?.Invoke());
    }

    public void SetMoneyText(string text) => _playerMoneyText.text = text;
    public void SetBigCoinText(string text) => _bigCoinText.text = text;

}
