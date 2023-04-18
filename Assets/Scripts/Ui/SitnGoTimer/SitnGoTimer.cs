using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SitnGoTimer : MonoBehaviour
{
    [SerializeField] private Text textField;
    private static DateTime timerEnd;
    private static Action timerEndCallback;
    private WaitForSeconds oneSecond = new WaitForSeconds(1);
    private string waitTextString;

    public static void StartTimer(DateTime timerEnd, Action timerEndCallback)
    {
        SitnGoTimer.timerEnd = timerEnd;
        SitnGoTimer.timerEndCallback = timerEndCallback;
    }

    private void OnEnable()
    {
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo)
        {
            waitTextString = SettingsManager.Instance.GetString(GameConstants.WaitingForPlayersMessage);
            StartCoroutine(TimerTick());
        }
    }

    private void OnDisable()
    {
        if (GameModeSettings.Instance.gameMode == GameModes.SitNGo)
            StopAllCoroutines();
    }

    private IEnumerator TimerTick()
    {
        while(true)
        {
            TimeSpan timerLeft = timerEnd.Subtract(DateTime.UtcNow);
            if ((int)timerLeft.TotalSeconds <= 0)
            {
                timerEndCallback?.Invoke();
                yield break;
            }
            textField.text = waitTextString + " " + timerLeft.ToString(@"mm\:ss");
            yield return oneSecond;
        }
    }
}
