using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class OpenCardsChoiseWindowView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerLabel;
    [SerializeField] TextMeshProUGUI headerLabel;
    [SerializeField] TextMeshProUGUI yesButtonLabel;
    [SerializeField] TextMeshProUGUI noButtonLabel;
    public event Action OnYesClick;
    public event Action OnNoClick;
    public event Action OnTimerEnds;

    public void OnYesClickHandler() => OnYesClick?.Invoke();

    public void OnNoClickhandler() => OnNoClick?.Invoke();

    public void SetTexts()
    {
        headerLabel.text = SettingsManager.Instance.GetString("OpenCardsChoise.Question");
        yesButtonLabel.text = SettingsManager.Instance.GetString("OpenCardsChoise.Yes");
        noButtonLabel.text = SettingsManager.Instance.GetString("OpenCardsChoise.No");
    }

    public void StartTimer(float seconds)
    {
        SetTexts();
        StartCoroutine(TimerRoutine());
        IEnumerator TimerRoutine()
        {
            for (int i = 0; i < seconds; i++)
            {
                timerLabel.text = (seconds - i).ToString();
                yield return GameConstants.WaitSeconds_1;
            }
            OnTimerEnds?.Invoke();
        }
    }
}