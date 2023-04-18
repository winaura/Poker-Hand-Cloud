using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;
using UnityEngine;
using static Client;
using static NetworkManager;
using static Hub;
using PokerHand.Common.Dto;

public class SigningController : MonoBehaviour
{
    [SerializeField] SigningView view;
    [SerializeField] GameObject registrationFormPrefab;
    GameObject registrationForm = null;
    RegistrationController registrationController;

    void Start()
    {
        OnReceiveContinueRegistration += ContinueRegistration;
        OnStartSilentAuthentication += OnSilentAuthenticationStart;
        OnStartAuthentication += OnAuthenticationStart;
        view.OnGoogleSignClick += OnGoogleSigninButtonClick;
        view.OnFacebookSignClick += OnFacebookSigninButtonClick;
        view.OnGuestSignClick += OnGuestSigninButtonClick;
        view.OnSigningCloseClick += OnSigningCloseButtonClick;
        view.OnAppleSignClick += OnAppleSigninButtonClick;
        OnReceiveMyPlayerProfile += SigningComplete;
    }

    void OnDestroy()
    {
        OnReceiveMyPlayerProfile -= SigningComplete;
        OnReceiveContinueRegistration -= ContinueRegistration;
        OnStartSilentAuthentication -= OnSilentAuthenticationStart;
        OnStartAuthentication -= OnAuthenticationStart;
        view.OnGoogleSignClick -= OnGoogleSigninButtonClick;
        view.OnFacebookSignClick -= OnFacebookSigninButtonClick;
        view.OnGuestSignClick -= OnGuestSigninButtonClick;
        view.OnSigningCloseClick -= OnSigningCloseButtonClick;
        view.OnAppleSignClick -= OnAppleSigninButtonClick;
        if (registrationController != null)
            registrationController.OnRegistrationComplete -= OnRegistrationFormComplete;
    }

    void SigningComplete(PlayerProfileDto playerProfileDto) => isSignitificated = true;

    void OnAuthenticationStart() => view.SetState(SigningViewState.Auth);

    void OnSilentAuthenticationStart() => view.SetState(SigningViewState.AuthSilent);


    void ContinueRegistration()
    {
        view.SetState(SigningViewState.RegistrationForm); // just disable previous state because registration form is a separate pair of controller-view
        if (registrationForm == null)
        {
            registrationForm = Instantiate(registrationFormPrefab, view.transform);
            registrationController = registrationForm.GetComponent<RegistrationController>();
            if (ChosenProvider != ExternalProviderName.None)
                registrationController.SetDefaultName(UserNameFromProviderProfile);
            registrationController.OnRegistrationComplete += OnRegistrationFormComplete;
            registrationController.OnRegistrationFormClose += OnRegistrationFromCloseHandler;
        }
        else
            registrationForm.SetActive(true);
    }

    void OnRegistrationFormComplete(string userName, Gender gender, HandsSpriteType handsType)
    {
        if (ChosenProvider == ExternalProviderName.None)
            NetworkManager.Instance.RegisterAsGuest(userName, gender, handsType);
        else
            NetworkManager.Instance.RegisterWithProvider(
                userName,
                gender,
                handsType,
                ChosenProvider,
                UserID
                );
        registrationController.OnRegistrationComplete -= OnRegistrationFormComplete;
    }

    void OnRegistrationFromCloseHandler()
    {
        registrationForm.SetActive(false);
        view.SetState(SigningViewState.Auth);
    }

    void OnGoogleSigninButtonClick()
    {
        ChosenProvider = ExternalProviderName.Google;
        NetworkManager.Instance.SignWithProvider(ExternalProviderName.Google);
    }    
    
    void OnAppleSigninButtonClick()
    {
        ChosenProvider = ExternalProviderName.Apple;
        NetworkManager.Instance.SignWithProvider(ExternalProviderName.Apple);
    }

    void OnFacebookSigninButtonClick()
    {
        ChosenProvider = ExternalProviderName.Facebook;
        NetworkManager.Instance.SignWithProvider(ExternalProviderName.Facebook);
    }

    void OnGuestSigninButtonClick()
    {
        ChosenProvider = ExternalProviderName.None;
        ContinueRegistration();
    }

    void OnSigningCloseButtonClick()
    {
        Application.Quit();
    }
}