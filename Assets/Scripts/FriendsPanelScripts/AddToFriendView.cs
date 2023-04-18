using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AddToFriendView : MonoBehaviour
{
    [SerializeField] private InputField _inputField;
    [SerializeField] private GameObject _wrongCodeText;
    [SerializeField] private Button _sendCodeButton;
    [Header("AddToFriendsWindowTexts"), SerializeField] private Text _addToFriendText;
    [SerializeField] private Text _enterCodeHere;
    [SerializeField] private Text _enterCode;
    [SerializeField] private Text _sendCode;
    [SerializeField] private Text _wrongCode;

    private void Awake()
    {
        SetTexts();
        _sendCodeButton.onClick.AddListener(SendCode);
        Client.OnSuccessOnAddFriendByPersonalCode += ShowSuccessPanel;
        Client.OnErrorOnAddFriendByPersonalCode += ShowErrorPanel;
    }

    private void OnEnable()
    {
        _wrongCodeText.SetActive(false);
        _inputField.text = string.Empty;
    }

    private void OnDestroy()
    {
        Client.OnSuccessOnAddFriendByPersonalCode -= ShowSuccessPanel;
        Client.OnErrorOnAddFriendByPersonalCode -= ShowErrorPanel;
    }
    private void SetTexts()
    {
        _addToFriendText.text = SettingsManager.Instance.GetString("FriendsPanel.AddToFriendText");
        _enterCodeHere.text = SettingsManager.Instance.GetString("FriendsPanel.EnterCodeHere");
        _enterCode.text = SettingsManager.Instance.GetString("FriendsPanel.EnterCode");
        _sendCode.text = SettingsManager.Instance.GetString("FriendsPanel.SendCode");
        _wrongCode.text = SettingsManager.Instance.GetString("FriendsPanel.WrongCode");
    }

    private void SendCode() => SendRequestByPersonalCode(PlayerProfileData.Instance.Id, _inputField.text);

    public void SendRequestByPersonalCode(Guid senderJson, string personalCode)
    {
        var requestSenderIdJson = JsonConvert.SerializeObject(senderJson);
        Hub.SendAsync(ServerMethods.AddFriendByPersonalCode, requestSenderIdJson, personalCode);
    }

    private void ShowSuccessPanel()
    {
        gameObject.SetActive(false);
        _inputField.text = string.Empty;
        NotificationController.ShowNotification(NotificationController.NotificationType.RequestToFriendSuccessful);
    }

    private void ShowErrorPanel(string message)
    {
        switch(message)
        {
            case "Wrong personal code":
                _wrongCodeText.SetActive(true);
                break;
        }
    }
}
