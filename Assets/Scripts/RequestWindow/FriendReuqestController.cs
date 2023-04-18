using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendReuqestController : MonoBehaviour
{
    [SerializeField] private RequestWindow friendNotificationWindowPrefab;

    private static FriendReuqestController Instance;
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Client.OnReceiveFriendRequest += ShowNotification;
    }

    private void ShowNotification(FriendDto friendDto)
    {
        RequestWindow requestWindow = Instantiate(friendNotificationWindowPrefab);
        requestWindow.SetNotificationText(friendDto);
    }
}
