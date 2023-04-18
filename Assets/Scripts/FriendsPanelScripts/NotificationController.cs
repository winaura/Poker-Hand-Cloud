using UnityEngine;
using UnityEngine.SceneManagement;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private NotificationWindow notificationWindowPref;
    private static NotificationController notificationController;

    public enum NotificationType
    {
        RequestToFriendSuccessful,
        RemoveFriend,
        ErrorOnJoinFriend,
        ErrorOnInviteFriend,
        ErrorOnRequestFriend,
        ErrorOnConnectToTable,
        AutoTopInfo,
        NoAdsAvaible,
        MoneyBoxNothingToOpen,
        PlayerHasAlreadyAuthorized,
        ServerNotWorking,
        ServerVersionDismatch
    }

    void Start()
    {
        if (notificationController != null)
        {
            Destroy(gameObject);
            return;
        }
        Client.OnErrorOnInviteFriendToTable += (message) =>
        {
            NotificationWindow notificationWindow = Instantiate(notificationWindowPref);
            notificationWindow.SetNotification(NotificationType.ErrorOnInviteFriend, message);
        };
        Client.OnErrorOnJoinFriendsTable += (message) =>
        {
            NotificationWindow notificationWindow = Instantiate(notificationWindowPref);
            notificationWindow.SetNotification(NotificationType.ErrorOnJoinFriend, message);
        };
        Client.OnErrorOnAddFriendByPersonalCode += (message) =>
        {
            NotificationWindow notificationWindow = Instantiate(notificationWindowPref);
            notificationWindow.SetNotification(NotificationType.ErrorOnRequestFriend, message);
        };
        Client.OnErrorOnConnectionToTableById += (message) =>
        {
            ConnectingWindow.DestroyWindow();
            NotificationWindow notificationWindow = Instantiate(notificationWindowPref);
            notificationWindow.SetNotification(NotificationType.ErrorOnConnectToTable, message);
        };
        notificationController = GetComponent<NotificationController>();
        DontDestroyOnLoad(notificationController);
    }

    public static void ShowNotification(NotificationType notificationType, string message = "")
    {
        NotificationWindow notificationWindow = Instantiate(notificationController.notificationWindowPref);
        notificationWindow.SetNotification(notificationType, message);
    }
    public static void ShowNotification(GameModes gameMode)
    {
        NotificationWindow notificationWindow = Instantiate(notificationController.notificationWindowPref);
        notificationWindow.SetNotification(gameMode);
    }
}
