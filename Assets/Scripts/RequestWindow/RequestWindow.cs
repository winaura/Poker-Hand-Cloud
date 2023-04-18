using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class RequestWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text requestText;
    public void SetNotificationText(FriendDto friendDto)
    {
        requestText.text = SettingsManager.Instance.GetString("FriendsPanel.FriendNotification");
    }

    private void CloseWindow()
    {
        Destroy(gameObject);
    }
}
