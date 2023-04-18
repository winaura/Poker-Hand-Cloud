using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _allSoundButton;
    [SerializeField] private Button _vibrationButton;
    [SerializeField] private Button _handsButton;
    [SerializeField] private Button _hintsButton;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button[] _languages;
    [SerializeField] private Text _settingsText;
    [SerializeField] private Text _soundText;
    [SerializeField] private Text _allSoundText;
    [SerializeField] private Text _vibrationText;
    [SerializeField] private Text _handsText;
    [SerializeField] private Text _hintsText;
    [SerializeField] private Text _applyText;
    public event Action OnSoundButton;
    public event Action OnHandsButton;
    public event Action OnHintsButton;
    public event Action OnVibrationButton;
    public event Action OnApplyButton;
    public event Action OnAllSoundButton;
    private Language language;

    private void Awake()
    {
        _soundButton.onClick.AddListener(SetSound);
        _allSoundButton.onClick.AddListener(SetAllSound);
        _vibrationButton.onClick.AddListener(SetVibration);
        _handsButton.onClick.AddListener(SetHands);
        _hintsButton.onClick.AddListener(SetHints);
        _applyButton.onClick.AddListener(ApplyButton);
        ChangeIconVibration();
        ChangeIconSound();
        ChangeIconAllSound();
        ChangeIconHints();
        ChangeIconHands();
    }

    private void OnEnable()
    {
        language = PlayerData.Instance.language;
        SetLanguage((int)language);
    }

    private void SetSound()
    {
        OnSoundButton?.Invoke();
        ChangeIconSound();
    }    
    
    private void SetAllSound()
    {
        OnAllSoundButton?.Invoke();
        ChangeIconAllSound();
    }

    private void SetHands()
    {
        OnHandsButton?.Invoke();
        ChangeIconHands();
    }

    private void SetHints()
    {
        OnHintsButton?.Invoke();
        ChangeIconHints();
    }

    private void SetVibration()
    {
        OnVibrationButton?.Invoke();
        ChangeIconVibration();
    }

    private void ApplyButton()
    {
        OnApplyButton?.Invoke();
        PlayerData.Instance.SaveSettings();
        AudioManager.Instance.SetVolume();
        PlayerData.Instance.SaveLanguage(language);
        SettingsManager.Instance.ChangeLanguage(language);
    }

    private void ChangeIconVibration()
    {
        if (PlayerData.Instance == null) return;
        _vibrationButton.GetComponent<Image>().color =
            PlayerData.Instance.isVibrationOn ? Color.white : Color.grey;
    }

    private void ChangeIconSound()
    {
        if (PlayerData.Instance == null) 
            return;
        _soundButton.GetComponent<Image>().color =
            PlayerData.Instance.isSoundOn ? Color.white : Color.grey;
    }

    private void ChangeIconAllSound()
    {
        if (PlayerData.Instance == null) return;
        _allSoundButton.GetComponent<Image>().color =
            PlayerData.Instance.isAllSoundOn ? Color.white : Color.grey;
    }

    private void ChangeIconHints()
    {
        if (PlayerData.Instance == null) return;
        _hintsButton.GetComponent<Image>().color =
            PlayerData.Instance.isHintsOn ? Color.white : Color.grey;
    }

    private void ChangeIconHands()
    {
        if (PlayerData.Instance == null) return;
        _handsButton.GetComponent<Image>().color =
            PlayerData.Instance.isHandsOn ? Color.white : Color.grey;
    }

    public void SetLanguage(int index)
    {
        foreach (var language in _languages)
            language.transform.GetChild(0).GetComponentInChildren<Image>().gameObject.SetActive(false);
        _languages[index].transform.GetChild(0).GetComponentInChildren<Image>().gameObject.SetActive(true);
        language = (Language)index;
    }

    public void SetWindowTexts()
    {
        _settingsText.text = SettingsManager.Instance.GetString("Settings.Settings");
        _soundText.text = SettingsManager.Instance.GetString("Settings.Music");
        _allSoundText.text = SettingsManager.Instance.GetString("Settings.Sound");
        _vibrationText.text = SettingsManager.Instance.GetString("Settings.Vibration");
        _handsText.text = SettingsManager.Instance.GetString("Settings.Hands");
        _hintsText.text = SettingsManager.Instance.GetString("Settings.Hints");
        _applyText.text = SettingsManager.Instance.GetString("Settings.Apply");
    }
}