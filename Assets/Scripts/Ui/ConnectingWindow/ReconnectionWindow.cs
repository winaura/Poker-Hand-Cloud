using System;
using System.Collections;
using UnityEngine;

public class ReconnectionWindow : MonoBehaviour
{
    private WaitForSeconds checkDelay = new WaitForSeconds(3);

    private void Awake()
    {
        StartCoroutine(CheckConnectionStatus());
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            StopAllCoroutines();
        else
            StartCoroutine(CheckConnectionStatus());
    }

    private IEnumerator CheckConnectionStatus()
    {
        while (true)
        {
            yield return checkDelay;
            switch(Hub.ConnectionState)
            {
                case BestHTTP.SignalRCore.ConnectionStates.Connected:
                case BestHTTP.SignalRCore.ConnectionStates.Closed:
                    Debug.Log($"{DateTime.UtcNow} Reconnecing by window, state:{Hub.ConnectionState}");
                    ConnectingWindow.DestroyWindow();
                    yield break;
            }
        }
    }
}
