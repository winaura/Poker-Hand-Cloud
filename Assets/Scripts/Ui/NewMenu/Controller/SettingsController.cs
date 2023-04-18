using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private SettingsView _settingsView;

    private void OnEnable()
    {
        _settingsView.OnHandsButton += SetHands;
        _settingsView.OnHintsButton += SetHints;
        _settingsView.OnSoundButton += SetSound;
        _settingsView.OnVibrationButton += SetVibration;
        _settingsView.OnApplyButton += CloseWindow;
        _settingsView.OnAllSoundButton += SetAllSound;
    }

    private void OnDisable()
    {
        _settingsView.OnHandsButton -= SetHands;
        _settingsView.OnHintsButton -= SetHints;
        _settingsView.OnSoundButton -= SetSound;
        _settingsView.OnVibrationButton -= SetVibration;
        _settingsView.OnApplyButton -= CloseWindow;
        _settingsView.OnAllSoundButton -= SetAllSound;
    }

    private void SetVibration()
    {
        if (PlayerData.Instance == null) 
            return;
        PlayerData.Instance.isVibrationOn = !PlayerData.Instance.isVibrationOn;
        if (PlayerData.Instance.isVibrationOn)        
            Vibration.Vibrate(50);        
    }

    private void SetSound()
    {
        if (PlayerData.Instance == null) 
            return;
        PlayerData.Instance.isSoundOn = !PlayerData.Instance.isSoundOn;
    }

    private void SetAllSound()
    {
        if (PlayerData.Instance == null)
            return;
        PlayerData.Instance.isAllSoundOn = !PlayerData.Instance.isAllSoundOn;
    }

    private void SetHints()
    {
        if (PlayerData.Instance == null) 
            return;
        PlayerData.Instance.isHintsOn = !PlayerData.Instance.isHintsOn;
    }

    private void SetHands()
    {
        if (PlayerData.Instance == null) 
            return;
        PlayerData.Instance.isHandsOn = !PlayerData.Instance.isHandsOn;
    }

    public void CloseWindow() => _settingsView.gameObject.SetActive(false);

    public void OpenWindow()
    {
        _settingsView.SetWindowTexts();
        _settingsView.gameObject.SetActive(true);
    }
}