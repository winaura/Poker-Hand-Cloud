using System;
using UnityEngine;
using UnityEngine.UI;

public class WonSitNGoWindowView : MonoBehaviour
{
    [SerializeField] private Button _tryAgainButton;
    [SerializeField] private Button _closeWindow;
    [SerializeField] private Text _placeText;
    [SerializeField] private Text _rewardChipsText;
    [SerializeField] private Text _windowNameText;
    [SerializeField] private Text _congratulationsText;
    [SerializeField] private Text _youWonText;
    [SerializeField] private Text _tryAgainText;
    [SerializeField] private Image _coinImage;
    public event Action OnEnterButton;
    public event Action OnCloseButtonPressed;

    private void Awake()
    {
        _tryAgainButton.onClick.AddListener(() => OnEnterButton?.Invoke());
        _closeWindow.onClick.AddListener(() => OnCloseButtonPressed?.Invoke());
    }

    public void UpdateWindow()
    {
        _windowNameText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.Name");
        _tryAgainText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.TryAgain");
        switch (PlayerProfileData.Instance.SitNGoPlace)
        {
            case 1:
                _placeText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.1stPlace");
                _congratulationsText.gameObject.SetActive(true);
                _congratulationsText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.Congratulations");
                _youWonText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.YouWon");
                _youWonText.gameObject.SetActive(true);
                _rewardChipsText.text = GameModeSettings.Instance.maxPrizeSitNGo.IntoCluttered();
                _rewardChipsText.gameObject.SetActive(true);
                _coinImage.gameObject.SetActive(true);
                break;
            case 2:
                _placeText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.2ndPlace");
                _congratulationsText.gameObject.SetActive(true);
                _congratulationsText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.Congratulations");
                _youWonText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.YouWon");
                _youWonText.gameObject.SetActive(true);
                _rewardChipsText.text = GameModeSettings.Instance.minPrizeSitNGo.IntoCluttered();
                _rewardChipsText.gameObject.SetActive(true);
                _coinImage.gameObject.SetActive(true);
                break;
            case 3:
                _placeText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.3rdPlace");
                _congratulationsText.gameObject.SetActive(true);
                _congratulationsText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.Lose");
                _youWonText.gameObject.SetActive(false);
                _rewardChipsText.gameObject.SetActive(false);
                _coinImage.gameObject.SetActive(false);
                break;
            case 4:
                _placeText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.4thPlace");
                _congratulationsText.gameObject.SetActive(false);
                _youWonText.gameObject.SetActive(false);
                _rewardChipsText.gameObject.SetActive(false);
                _coinImage.gameObject.SetActive(false);
                break;
            case 5:
                _placeText.text = SettingsManager.Instance.GetString("SitNGoFinishWindow.5thPlace");
                _congratulationsText.gameObject.SetActive(false);
                _youWonText.gameObject.SetActive(false);
                _rewardChipsText.gameObject.SetActive(false);
                _coinImage.gameObject.SetActive(false);
                break;
        }
    }
}