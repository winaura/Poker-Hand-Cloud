using System;
using UnityEngine;
using UnityEngine.UI;

public class SitNGoWindowView : MonoBehaviour
{
    [SerializeField] private Button _goToLevelButton;
    [SerializeField] private Button _closeWindow;
    [SerializeField] private Button _tableInfoButton;
    [SerializeField] private Text _letsGoText;
    [SerializeField] private Text _windowNameText;
    [SerializeField] private Text _buyinText;
    [SerializeField] private Text _buyinMoneyText;
    [SerializeField] private Text _firstPlacePrizeText;
    [SerializeField] private Text _secondPlacePrizeText;
    [SerializeField] private Text _blindingText;
    [SerializeField] private Text _startingChipsText;
    [SerializeField] private Text _blindLevelsText;
    [SerializeField] private Text _1stPlaceText;
    [SerializeField] private Text _2ndPlaceText;
    public event Action OnEnterButton;
    public event Action OnCloseButtonPressed;
    public event Action OnTableInfoPressed;

    private void Awake()
    {
        _goToLevelButton.onClick.AddListener(()=> OnEnterButton?.Invoke());
        _closeWindow.onClick.AddListener(() => OnCloseButtonPressed?.Invoke());
        _tableInfoButton.onClick.AddListener(() => OnTableInfoPressed?.Invoke());
    }

    public void UpdateAllText(PlayerData playerData, GameModeSettings gameModeSettings)
    {
        _windowNameText.text = SettingsManager.Instance.GetString("SitNGoWindowWithMoney.Description");
        _1stPlaceText.text = SettingsManager.Instance.GetString("SitNGoWindowWithMoney.1stPlace");
        _2ndPlaceText.text = SettingsManager.Instance.GetString("SitNGoWindowWithMoney.2ndPlace");
        _blindLevelsText.text = SettingsManager.Instance.GetString("SitNGoWindowWithMoney.BlindsLevels");
        _letsGoText.text = SettingsManager.Instance.GetString("SitNGoWindowWithMoney.LetsGo");
        _buyinText.text = playerData.money < gameModeSettings.minMoneyGet ? SettingsManager.Instance.GetString("SitNGoWindowWithMoney.NotEnoughMoney") : SettingsManager.Instance.GetString("SitNGoWindowWithMoney.BuyIn");
        _buyinText.gameObject.SetActive(true);
        _buyinMoneyText.text = $"${gameModeSettings.minMoneyGet.IntoCluttered()}";
        _blindingText.text = $"{SettingsManager.Instance.GetString("SitNGoWindowWithMoney.BlindsStarts")} {gameModeSettings.smallBlind.IntoCluttered()}/{gameModeSettings.bigBlind.IntoCluttered()}";
        _firstPlacePrizeText.text = $"${gameModeSettings.maxPrizeSitNGo.IntoCluttered()}";
        _secondPlacePrizeText.text = $"${gameModeSettings.minPrizeSitNGo.IntoCluttered()}";
        _startingChipsText.text = $"{SettingsManager.Instance.GetString("SitNGoWindowWithMoney.SatrtingChips")} = ${gameModeSettings.sitNGoMoneyGet.IntoCluttered()}";
    }

    public void SetCongratulationsWindow()
    {
        switch (PlayerProfileData.Instance.SitNGoPlace)
        {
            case 1:
                _buyinMoneyText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.1stPlace");
                break;
            case 2:
                _buyinMoneyText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.2ndPlace");
                break;
            case 3:
                _buyinMoneyText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.3rdPlace");
                break;
            case 4:
                _buyinMoneyText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.4thPlace");
                break;
            case 5:
                _buyinMoneyText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.5thPlace");
                break;
        }
        _letsGoText.text = SettingsManager.Instance.GetString("SitNGoWindowWithMoney.TryAgain");
        _buyinText.gameObject.SetActive(false);
    }
}