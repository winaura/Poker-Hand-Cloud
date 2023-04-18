using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerThinkTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerLabel;

    private void OnEnable() => StartCoroutine(TimerRoutine(GameConstants.OpenCardsChoiseLifetime));

    private void OnDisable() => StopAllCoroutines();

    IEnumerator TimerRoutine(float seconds)
    {
        for (int i=0; i < seconds; i++)
        {
            timerLabel.text = (seconds - i).ToString();
            yield return GameConstants.WaitSeconds_1;
        }
        gameObject.SetActive(false);
    }
}
