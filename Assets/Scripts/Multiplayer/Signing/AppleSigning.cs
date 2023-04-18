using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleSigning : MonoBehaviour
{
    private IAppleAuthManager appleAuthManager;
    public event Action<ICredential> OnAppleAuthenticationFinished;

    void Start()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserializer = new PayloadDeserializer();
            appleAuthManager = new AppleAuthManager(deserializer);
        }
    }

    public void SignIn()
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            OnAuthenticationFinished,
            error => Debug.LogError(error.GetAuthorizationErrorCode()));
    }

    void OnAuthenticationFinished(ICredential credential)
    {
        print($"{DateTime.UtcNow} Apple Authentication Finished");
        OnAppleAuthenticationFinished?.Invoke(credential);
    }

    private void Update()
    {
        if (appleAuthManager != null)
            appleAuthManager.Update();
    }
}
