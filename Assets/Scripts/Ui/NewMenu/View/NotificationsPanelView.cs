using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsPanelView : MonoBehaviour
{
    [SerializeField] GameObject messagePanel;
    [SerializeField] Text notificationText;

    public void SetNotificationText(string text) => notificationText.text = text;

    public void SetNotificationWithDuration(string text, float seconds) => StartCoroutine(NotificationDurationRoutine(text, seconds));

    IEnumerator NotificationDurationRoutine(string text, float seconds)
    {
        notificationText.text = text;
        messagePanel.SetActive(true);
        yield return new WaitForSeconds(seconds);
        messagePanel.SetActive(false);
        notificationText.text = string.Empty;
    }
}