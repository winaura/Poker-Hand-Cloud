using AppleAuth;
using System;
using TMPro;
using UnityEngine;

public enum SigningViewState { None, Initial, Auth, AuthSilent, RegistrationForm }

[RequireComponent(typeof(Canvas))]
public class SigningView : MonoBehaviour
{
    [SerializeField] SigningViewState initialState = SigningViewState.Initial;
    [Header("Wait panel"), SerializeField] GameObject waitPanel;
    [SerializeField] TextMeshProUGUI waitText;
    [Header("Authentication"), SerializeField] GameObject authPanel;
    [SerializeField] TextMeshProUGUI authHeaderLabel;
    [SerializeField] TextMeshProUGUI guestButtonLabel;
    [SerializeField] TextMeshProUGUI googleButtonLabel;
    [SerializeField] TextMeshProUGUI facebookButtonLabel;
    [SerializeField] TextMeshProUGUI appleButtonLabel;
    [SerializeField] GameObject appleButton;
    public event Action OnGoogleSignClick;
    public event Action OnFacebookSignClick;
    public event Action OnAppleSignClick;
    public event Action OnGuestSignClick;
    public event Action OnSigningCloseClick;
    SigningViewState currentState = SigningViewState.None;

    private void Start()
    {
        appleButton.SetActive(AppleAuthManager.IsCurrentPlatformSupported);
        authHeaderLabel.text = SettingsManager.Instance.GetString("Signing.AuthWindow");
        guestButtonLabel.text = SettingsManager.Instance.GetString("Signing.Guest");
        googleButtonLabel.text = SettingsManager.Instance.GetString("Signing.Google");
        facebookButtonLabel.text = SettingsManager.Instance.GetString("Signing.Facebook");
        appleButtonLabel.text = SettingsManager.Instance.GetString("Signing.Apple");
        SetState(initialState);
    }

    public void SetState(SigningViewState newState)
    {
        switch (currentState)
        {
            case SigningViewState.Initial: SetWait(false); break;
            case SigningViewState.Auth: SetAuthState(false); break;
            case SigningViewState.AuthSilent: SetWait(false); break;
        }
        switch (newState)
        {
            case SigningViewState.Initial: SetWait(true, waitString: SettingsManager.Instance.GetString("Signing.Connecting")); break;
            case SigningViewState.Auth: SetAuthState(true); break;
            case SigningViewState.AuthSilent: SetWait(true, waitString: SettingsManager.Instance.GetString("Signing.Authentication")); break;
        }
        currentState = newState;
    }

    public void OnGoogleButtonClick() => OnGoogleSignClick?.Invoke();

    public void OnFacebookButtonClick() => OnFacebookSignClick?.Invoke();

    public void OnAppleButtonClick() => OnAppleSignClick?.Invoke();

    public void OnGuestButtonClick() => OnGuestSignClick?.Invoke();

    public void OnCloseButtonClick() => OnSigningCloseClick?.Invoke();

       
    void SetWait(bool isWaiting, string waitString = "")
    {
        if (isWaiting)
        {
            waitPanel.SetActive(true);
            waitText.text = waitString;
        }
        else
            waitPanel.SetActive(false);
    }

    public void EnternetProblem(bool hasProblem)
    {
        if (hasProblem)
            SetWait(true, waitString: SettingsManager.Instance.GetString("Signing.NoEnternet"));
        else
            SetWait(true, waitString: SettingsManager.Instance.GetString("Signing.Connecting"));
    }

    void SetAuthState(bool value) => authPanel.SetActive(value);
}