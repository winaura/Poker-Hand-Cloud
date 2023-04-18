using AppleAuth.Interfaces;
using Facebook.Signing;
using Facebook;
using Google;
using Google.Signing;
using Newtonsoft.Json;
using PokerHand.Common.Dto;
using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;
using PokerHand.Token;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static Client;
using static Hub;
using Facebook.Unity;

public class NetworkManager : Singleton<NetworkManager>
{
    [SerializeField] PlayerProfileData profileData;
    [SerializeField] SigningView signingPrefab;
    [SerializeField] GoogleSigning googleSigning;
    [SerializeField] FacebookSigning facebookSigning;
    [SerializeField] AppleSigning appleSigning;
    public static event Action OnStartAuthentication;
    public static bool isSignitificated = false;
    public static event Action OnStartSilentAuthentication;
    public static event Action<bool, string> OnAuthenticationWithProviderComplete;
    public static string UserID { get; private set; } = null;
    public static string UserNameFromProviderProfile { get; private set; } = null;
    public static ExternalProviderName ChosenProvider { get; set; } = ExternalProviderName.None;
    SigningView signingObj = null;
    ConnectingWindow connectingWindow = null;

    private static DateTime date = new DateTime(2022, 5, 5, 0, 0, 0, DateTimeKind.Utc);
    private static string _token;
    public static string Token
    {
        get { return _token; }
        set { _token = GenerationToken.GetToken(date); }
    }

    protected override void Awake()
    {
        base.Awake();
        PlayerProfileData.InitializeWith(profileData);
        Token = "";
    }

    void Start()
    {
        switch(ConnectionState)
        {
            case BestHTTP.SignalRCore.ConnectionStates.Connected:
                ConnectingWindow.DestroyWindow();
                if (!isSignitificated)
                    goto case BestHTTP.SignalRCore.ConnectionStates.Initial;
                break;
            case BestHTTP.SignalRCore.ConnectionStates.Initial:
            case BestHTTP.SignalRCore.ConnectionStates.Closed:
                ConnectingWindow.DestroyWindow();
                signingObj = Instantiate(signingPrefab);
                OnConnectedToServer += OnConnect;
                StartCoroutine(Connect());
                break;
            default:
                OnReconected += Reconnected;
                ConnectingWindow.CreateWindow(ConnectingType.Reconnecting);
                break;
        }
    }

    private void Reconnected()
    {
        print($"{DateTime.UtcNow} Reconnected. Delete connectingWindow");
        OnReconected -= Reconnected;
        HTTP_UpdateTotalMoneyAndExperienceRequest();
        ConnectingWindow.DestroyWindow();
        SceneManager.LoadScene(GameConstants.SceneMainMenu);
    }

    IEnumerator Connect()
    {
        while(Application.internetReachability == NetworkReachability.NotReachable)
        {
            signingObj.EnternetProblem(true);
            yield return null;
        }
        signingObj.EnternetProblem(false);
        ConnectAsync();
    }

    void OnConnect()
    {
        OnReceiveConnectionId += StartAuthentication;
        HTTP_SendAllTablesInfoRequest();
        HTTP_SendPresentsInfoRequest();
    }

    void StartAuthentication()
    {
        OnReceiveConnectionId -= StartAuthentication;
        OnReceiveMyPlayerProfile += OnReceivePlayerProfile;
        if (PlayerProfileData.Instance.Id == Guid.Empty)
        {
            OnStartAuthentication?.Invoke();
        }
        else
        {
            OnStartSilentAuthentication?.Invoke();
            HTTP_SendAuthenticateRequest();
        }
    }

    void OnReceivePlayerProfile(PlayerProfileDto ppd)
    {
        OnReceiveMyPlayerProfile -= OnReceivePlayerProfile;
        if (signingObj != null)
            Destroy(signingObj.gameObject);
    }

    public void SignWithProvider(ExternalProviderName provider)
    {
        switch (provider)
        {
            case ExternalProviderName.Google:
                googleSigning.OnGoogleAuthenticationFinished += OnGoogleAuthenticated;
                googleSigning.SignIn();
                break;
            case ExternalProviderName.Facebook:
                facebookSigning.OnFacebookAuthenticationFinished += OnFacebookAuthenticated;
                facebookSigning.SignIn();
                break;
            case ExternalProviderName.Apple:
                appleSigning.OnAppleAuthenticationFinished += OnAppleAuthenticated;
                appleSigning.SignIn();
                break;
        }
    }

    public void SignOutWithProvider(ExternalProviderName provider)
    {
        switch (provider)
        {
            case ExternalProviderName.Google:
                googleSigning.OnSignOut();
                break;
            case ExternalProviderName.Facebook:
                facebookSigning.SignOut();
                break;
        }
        ChosenProvider = ExternalProviderName.None;
    }

    public void SignWithGoogle(Action<Task<GoogleSignInUser>> callback) => googleSigning.SignIn(callback);

    public void SignWithFacebook(Action<IResult> callback)
    {
        facebookSigning.OnFacebookAuthenticationFinished += callback;
        facebookSigning.SignIn();
    }

    public void RegisterWithProvider(string userName, Gender gender, HandsSpriteType handsSprite, ExternalProviderName provider, string userId)
    {
        profileData.SetLastLoginProvider(provider);
        HTTP_SendRegisterWithExternalProvider(userName, gender, handsSprite, provider, userId);
    }

    public void RegisterAsGuest(string userName, Gender gender, HandsSpriteType handsType)
    {
        profileData.SetLastLoginProvider(ExternalProviderName.None);
        HTTP_SendRegisterAsGuest(userName, gender, handsType);
    }

    void OnAppleAuthenticated(ICredential credential)
    {
        appleSigning.OnAppleAuthenticationFinished -= OnAppleAuthenticated;
        // Obtained credential, cast it to IAppleIDCredential
        var appleIdCredential = credential as IAppleIDCredential;
        if (appleIdCredential != null)
        {
            OnAuthenticationWithProviderComplete?.Invoke(true, string.Empty);
            HTTP_SendTryAuthenticateWithExternalProvider(appleIdCredential.User);
            UserID = appleIdCredential.User;
            UserNameFromProviderProfile = appleIdCredential.FullName.Nickname;
        }
    }

    void OnGoogleAuthenticated(Task<GoogleSignInUser> task)
    {
        googleSigning.OnGoogleAuthenticationFinished -= OnGoogleAuthenticated;
        if (task.IsFaulted)
        {
            using (var enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    OnAuthenticationWithProviderComplete?.Invoke(false, $"Sign in with Google error: {error.Status} {error.Message}");
                    Debug.LogError($"{DateTime.UtcNow} Sign in with Google error: {error.Status} {error.Message}");
                }
                else
                {
                    OnAuthenticationWithProviderComplete?.Invoke(false, $"Sign in with Google exception {task.Exception}");
                    Debug.LogError($"{DateTime.UtcNow} Sign in with Google exception {task.Exception}");
                }
            }
        }
        else if (task.IsCanceled)
        {
            OnAuthenticationWithProviderComplete?.Invoke(false, string.Empty);
        }
        else
        {
            OnAuthenticationWithProviderComplete?.Invoke(true, string.Empty);
            HTTP_SendTryAuthenticateWithExternalProvider(task.Result.UserId);
            UserID = task.Result.UserId;
            UserNameFromProviderProfile = task.Result.DisplayName;
            StartCoroutine(DownloadImage(task.Result.ImageUrl.AbsoluteUri));
        }
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        byte[] imageBytes;
        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError($"{DateTime.UtcNow} {request.error}");
                imageBytes = Resources.Load<Texture2D>("Images/Avatar_standart").EncodeToPNG();
                break;
            case UnityWebRequest.Result.Success:
                imageBytes = ((DownloadHandlerTexture)request.downloadHandler).texture.EncodeToPNG();
                break;
            default:
                imageBytes = new byte[1];
                break;
        }
        HTTP_SendUpdateProfileImage(imageBytes);
    }

    void OnFacebookAuthenticated(IResult result)
    {
        facebookSigning.OnFacebookAuthenticationFinished -= OnFacebookAuthenticated;
        if (result == null)
            OnAuthenticationWithProviderComplete?.Invoke(false, $"{result.Error}");
        else
        {
            OnAuthenticationWithProviderComplete?.Invoke(true, string.Empty);
            if (FB.Mobile.CurrentProfile() == null)
            {
                FB.Mobile.RefreshCurrentAccessToken(RefreshCallback);
                return;
            }
            SendTryToAuthWithProvider();
        }
        foreach (Action<bool, string> d in OnAuthenticationWithProviderComplete.GetInvocationList())
            OnAuthenticationWithProviderComplete -= d;
    }

    void RefreshCallback(IAccessTokenRefreshResult result)
    {
        if (FB.IsLoggedIn)
            SendTryToAuthWithProvider();
    }

    void SendTryToAuthWithProvider() => FB.API("me", HttpMethod.GET, FacebookProfileResponce);

    void FacebookProfileResponce(IGraphResult result)
    {
        IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
        string name = dict["name"].ToString();
        string userID = dict["id"].ToString();
        print($"{DateTime.UtcNow} Facebook data name:{name} id:{userID}");
        HTTP_SendTryAuthenticateWithExternalProvider(userID);
        UserID = userID;
        UserNameFromProviderProfile = name;
        OnReceiveMyPlayerProfile += LoadFacebookImage;
        void LoadFacebookImage(PlayerProfileDto playerProfile)
        {
            OnReceiveMyPlayerProfile -= LoadFacebookImage;
            print($"{DateTime.UtcNow} Loading facebook image");
            FB.API("me/picture?type=med", HttpMethod.GET, LoadImage);
            void LoadImage(IGraphResult result)
            {
                print($"{DateTime.UtcNow} Facebook image loaded");
                var bytes = result.Texture.EncodeToPNG();
                HTTP_SendUpdateProfileImage(bytes);
            }
        }
    }
    public void ResetProfileData()
    {
        PlayerProfileData.ResetData();
        PlayerProfileData.InitializeWith(profileData);
    }



    void OnDestroy() => OnConnectedToServer -= OnConnect;

    void OnApplicationQuit()
    {
        CloseAsync();
    }
}