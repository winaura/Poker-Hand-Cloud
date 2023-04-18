using System.Collections;
using UnityEngine;

public class Share : MonoBehaviour
{
    [SerializeField] private string playmarketLink = "https://play.google.com/apps/internaltest/4700096720606702447";
    [SerializeField] private string appstoreLink = "https://apps.apple.com/us/app/com-mygame-pokerhand/id1585304705";
    public void ShareClick()
    {
        NativeShare nativeShare = new NativeShare();
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                nativeShare.SetText(string.Format(SettingsManager.Instance.GetString("Share.Message"), playmarketLink, ReceiveFriendsDataFromServer.PersonalCode, ':', '\n'));
                break;
            case RuntimePlatform.IPhonePlayer:
                nativeShare.SetText(string.Format(SettingsManager.Instance.GetString("Share.Message"), appstoreLink, ReceiveFriendsDataFromServer.PersonalCode, ':', '\n'));
                break;
            default:
                nativeShare.SetText(string.Format(SettingsManager.Instance.GetString("Share.Message"), playmarketLink, ReceiveFriendsDataFromServer.PersonalCode, ':', '\n'));
                break;
        }
        nativeShare.Share();
    }

    public void CopyToClipBoardClick()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                GUIUtility.systemCopyBuffer = string.Format(SettingsManager.Instance.GetString("Share.Message"), playmarketLink, ReceiveFriendsDataFromServer.PersonalCode, ':', '\n');
                break;
            case RuntimePlatform.IPhonePlayer:
                GUIUtility.systemCopyBuffer = string.Format(SettingsManager.Instance.GetString("Share.Message"), appstoreLink, ReceiveFriendsDataFromServer.PersonalCode, ':', '\n');
                break;
            default:
                GUIUtility.systemCopyBuffer = string.Format(SettingsManager.Instance.GetString("Share.Message"), playmarketLink, ReceiveFriendsDataFromServer.PersonalCode, ':', '\n');
                break;
        }
    }
}
