using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Signing
{
    public class FacebookSigning : MonoBehaviour
    {
        public event Action<IResult> OnFacebookAuthenticationFinished;
        List<string> permissions = new List<string>()
        {
            "gaming_profile",
            "email",
            "gaming_user_picture"
        };

        public void SignIn()
        {
            if (!FB.IsInitialized)
                FB.Init(OnInitComplete, OnHideUnity);
            else
                FB.ActivateApp();
        }

        public void SignOut()
        {
            if (FB.IsInitialized && FB.IsLoggedIn)
                FB.LogOut();
        }

        public void SignInSilent() => FB.Android.RetrieveLoginStatus(LoginStatusCallback);

        private void OnHideUnity(bool isGameShown) { }

        private void OnInitComplete()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                FB.LogInWithReadPermissions(permissions, OnAuthComplete);
            }
        }


        private void LoginStatusCallback(ILoginStatusResult result)
        {
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log("Error: " + result.Error);
                FB.LogInWithReadPermissions(permissions, OnAuthComplete);
            }
            else if (result.Failed)
            {
                Debug.Log("Failure: Access Token could not be retrieved");
                FB.LogInWithReadPermissions(permissions, OnAuthComplete);
            }
            else
            {
                Debug.Log("Success: " + result.AccessToken.UserId);
                OnFacebookAuthenticationFinished?.Invoke(result);
            }
        }

        protected void OnAuthComplete(IResult result)
        {
            print($"{DateTime.UtcNow} Auth complete {result}");
            if (FB.IsLoggedIn)
                OnFacebookAuthenticationFinished?.Invoke(result);
        }
    }
}