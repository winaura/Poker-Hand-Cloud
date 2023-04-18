using System;
using UnityEngine;
using UnityEngine.UI;

public class MainLobbyView : MonoBehaviour
{
    [SerializeField] private Text _dashPokerText;
    [SerializeField] private Text _lowballPokerText;
    [SerializeField] private Text _privateRoomText;
    [SerializeField] private Text _tournamentText;
    [SerializeField] private Text _inventaryText;
    [Header("MoneyBoxButton"), SerializeField] private Button _moneyBoxButton;
    [SerializeField] private Slider _moneyBoxProgressSlider;
    [SerializeField] private Text _moneyBoxProgressText;
    [SerializeField] private Text _moneyBoxOpenText;
    public Action OnMoneyBoxButtonClick;

    public void Awake()
    {
        _moneyBoxButton.onClick.AddListener(() => OnMoneyBoxButtonClick.Invoke());
        Client.OnReceiveCurrentMoneyBoxAmount += UpdateMoneyButtonProgress;
        Client.OnReceiveCurrentMoneyBoxAmount += UpdateMoneyBoxReward;
    }

    private void UpdateMoneyButtonProgress(int sum)
    {
        _moneyBoxProgressSlider.maxValue = PlayerProfileData.Instance.moneyBox.MaxChips;
        _moneyBoxProgressSlider.value = sum;
        _moneyBoxProgressText.text = $"${sum.IntoCluttered()}/${PlayerProfileData.Instance.moneyBox.MaxChips.IntoCluttered()}";
    }

    private void UpdateMoneyBoxReward(int sum) => _moneyBoxOpenText.text = "$" + sum.IntoCluttered();

    public void SetWindowTexts()
    {
        _dashPokerText.text = SettingsManager.Instance.GetString("MainMenu.DashPoker");
        _lowballPokerText.text = SettingsManager.Instance.GetString("MainMenu.LowballPoker");
        _privateRoomText.text = SettingsManager.Instance.GetString("MainMenu.PrivateRoom");
        _tournamentText.text = SettingsManager.Instance.GetString("MainMenu.Tournament");
        _inventaryText.text = SettingsManager.Instance.GetString("MainMenu.Inventary");
    }

    private void OnDestroy()
    {
        Client.OnReceiveCurrentMoneyBoxAmount -= UpdateMoneyButtonProgress;
        Client.OnReceiveCurrentMoneyBoxAmount -= UpdateMoneyBoxReward;
    }
}