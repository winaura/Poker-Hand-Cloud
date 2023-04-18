using System;
using UnityEngine;

public class WonSitNGoWindowController : MonoBehaviour
{
    [SerializeField] private WonSitNGoWindowView _wonSitNGoWindowView;
    public event Action OnEnterButton;

    private void Awake()
    {
        _wonSitNGoWindowView.OnEnterButton += EnterButtonClick;
        _wonSitNGoWindowView.OnCloseButtonPressed += Close;
    }

    private void OnDestroy()
    {
        _wonSitNGoWindowView.OnEnterButton -= EnterButtonClick;
        _wonSitNGoWindowView.OnCloseButtonPressed -= Close;
    }
    private void OnEnable()
    {
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo && PlayerProfileData.Instance != null && PlayerProfileData.Instance.SitNGoPlace != -1)
            OpenStartScreen();
    }

    public void Close() => _wonSitNGoWindowView.gameObject.SetActive(false);

    private void EnterButtonClick() => OnEnterButton?.Invoke();

    private void OpenStartScreen()
    {
        _wonSitNGoWindowView.UpdateWindow();
        _wonSitNGoWindowView.gameObject.SetActive(true);
    }
}