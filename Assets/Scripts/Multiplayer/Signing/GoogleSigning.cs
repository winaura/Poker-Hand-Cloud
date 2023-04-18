namespace Google.Signing
{
    using Google;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;

    public class GoogleSigning : MonoBehaviour
    {
        public string webClientId = "<web client id here>";
        public event Action<Task<GoogleSignInUser>> OnGoogleAuthenticationFinished;
        GoogleSignInConfiguration configuration;

        void Awake()
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                UseGameSignIn = Application.platform == RuntimePlatform.Android
            };
        }

        public void SignIn()
        {
            GoogleSignIn.Configuration = configuration;
            if (Application.platform == RuntimePlatform.Android)
            {
                GoogleSignIn.Configuration.UseGameSignIn = true;
                GoogleSignIn.Configuration.RequestIdToken = false;
            }
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        }

        public void SignIn(Action<Task<GoogleSignInUser>> callback)
        {
            GoogleSignIn.Configuration = configuration;
            if (Application.platform == RuntimePlatform.Android)
            {
                GoogleSignIn.Configuration.UseGameSignIn = true;
                GoogleSignIn.Configuration.RequestIdToken = false;
            }
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(callback);
        }

        public void OnSignOut()
        {
            print($"{DateTime.UtcNow} [Google] Calling SignOut");
            GoogleSignIn.DefaultInstance.SignOut();
        }

        public void OnDisconnect()
        {
            print($"{DateTime.UtcNow} [Google] Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            print($"{DateTime.UtcNow} [Google] Authentication Finished");
            OnGoogleAuthenticationFinished?.Invoke(task);
        }

        public void OnSignInSilently()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
        }

        public void OnGamesSignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = true;
            GoogleSignIn.Configuration.RequestIdToken = false;
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        }
    }
}