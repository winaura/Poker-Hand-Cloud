using UnityEngine;

public class NotificationsPanelController : MonoBehaviour
{
    [SerializeField] NotificationsPanelView view;

    void Start()
    {
        var tryMessageType = MessagingSystem.GetFirstMessage();
        if (tryMessageType != null)
        {
            var messageType = tryMessageType ?? default;
            string text;
            switch(messageType)
            {
                case MessageType.KickedAFK:
                    text = SettingsManager.Instance.GetString(GameConstants.KickedByAFKNotification);
                    break;
                default: text = string.Empty; break;
            }
            view.SetNotificationWithDuration(text, GameConstants.MainMenuNotificationDuration);
        }
    }
}