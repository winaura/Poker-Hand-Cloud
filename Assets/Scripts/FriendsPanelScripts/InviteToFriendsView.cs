using UnityEngine;
using UnityEngine.UI;

public class InviteToFriendsView : MonoBehaviour
{
    [Header("InviteToFriendsWindowTexts")]
    [SerializeField] private Text _addToFriendText;
    [SerializeField] private Text _sendReference;
    [SerializeField] private Text _sendReferenceToClipboard;
    [SerializeField] private Text _ready;
    [SerializeField] private Text _share;
    [SerializeField] private Text _copy;

    void Awake() => SetTexts();

    private void SetTexts()
    {
        _addToFriendText.text = SettingsManager.Instance.GetString("FriendsPanel.AddToFriendText");
        _sendReference.text = SettingsManager.Instance.GetString("FriendsPanel.SendReference");
        _sendReferenceToClipboard.text = SettingsManager.Instance.GetString("FriendsPanel.SendReferenceToClipboard");
        _ready.text = SettingsManager.Instance.GetString("FriendsPanel.Ready");
        _share.text = SettingsManager.Instance.GetString("FriendsPanel.Share");
        _copy.text = SettingsManager.Instance.GetString("FriendsPanel.Copy");
    }
}
