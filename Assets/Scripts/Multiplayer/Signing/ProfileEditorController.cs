using Facebook.Unity;
using Google;
using PokerHand.Common.Dto;
using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static Client;

public class ProfileEditorController : MonoBehaviour
{
    [SerializeField] ProfileEditorView view;
    public event Action<string, Gender, HandsSpriteType> OnProfileEditingComplete;
    public event Action OnProfileEditingClose;

    private void Start()
    {
        view.OnContinueButtonClick += OnProfileEditingCompleteHandler;
        view.OnProviderButtonClick += OnProviderButtonClickHandler;
        view.OnCloseButtonClick += OnProfileEditingCloseHandler;
    }

    private void OnDestroy()
    {
        view.OnContinueButtonClick -= OnProfileEditingCompleteHandler;
        view.OnProviderButtonClick -= OnProviderButtonClickHandler;
        view.OnCloseButtonClick -= OnProfileEditingCloseHandler;
    }

    public void SetInitialData(string nickname, Gender gender, HandsSpriteType handsType, ExternalProviderName externalProviderName) =>
        view.SetData(nickname, gender, handsType, externalProviderName);

    public void OnProfileEditingCompleteHandler(string name, Gender gender, HandsSpriteType handsType) =>
        OnProfileEditingComplete?.Invoke(name, gender, handsType);

    public void OnProviderButtonClickHandler(ExternalProviderName providerName)
    {
        NetworkManager.ChosenProvider = providerName;
        switch (providerName)
        {
            case ExternalProviderName.Google:
                NetworkManager.Instance.SignWithGoogle(OnGoogleSignInComplete);
                break;
            case ExternalProviderName.Facebook:
                NetworkManager.Instance.SignWithFacebook(OnFacebookSignInComplete);
                break;
        }
    }

    public void OnProfileEditingCloseHandler() => OnProfileEditingClose?.Invoke();

    void OnGoogleSignInComplete(Task<GoogleSignInUser> task)
    {
        if (task.IsCompleted)
            HTTP_AddExternalProviderRequest(ExternalProviderName.Google, task.Result.UserId);
    }

    void OnFacebookSignInComplete(IResult result)
    {
        if (result != null)
        {
            if (FB.Mobile.CurrentProfile() == null)
            {
                FB.Mobile.RefreshCurrentAccessToken(RefreshCallback);
                return;
            }
            FB.API("me", HttpMethod.GET, (result) =>
            {
                IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
                string userID = dict["id"].ToString();
                HTTP_AddExternalProviderRequest(ExternalProviderName.Facebook, userID);
            });
        }
    }

    void RefreshCallback(IAccessTokenRefreshResult result)
    {
        if (FB.IsLoggedIn)
        {
            FB.API("me", HttpMethod.GET, (result) =>
            {
                IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
                string userID = dict["id"].ToString();
                HTTP_AddExternalProviderRequest(ExternalProviderName.Facebook, userID);
            });
        }
    }
}