using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using PokerHand.Common.Helpers.QuickChat;
using System.Linq;

public class FriendsChatView : MonoBehaviour
{
    [SerializeField] private Button _quitButton;
    [SerializeField] private ChatMessage textObject;
    [SerializeField] private ChatEmoji emojiObject;
    [SerializeField] private RectTransform contentWindow;
    [SerializeField] private Button _sendButton;
    [SerializeField] private InputField _messageInputField;
    [SerializeField] private Text _personalChat;
    [SerializeField] private Text _sendMessage;
    [SerializeField] private Text _enterMessage;
    [HideInInspector] public Guid recipientId;
    [SerializeField] private GameObject emojiPanel;
    private int _spacing = 257;
    public static event Action OnMessageRead;
    [Serializable] private struct PrivateMessage
    {
        public QuickMessage messageType;
        public string messageText;
    }

    #region Initialize
    private void CreateTextMessage(Guid id, string messageString, bool isMine)
    {
        ChatMessage message = Instantiate(textObject, contentWindow);
        message.ID = id;
        message.messageText.text = messageString;
        if (isMine)
            message.verticalLayout.childAlignment = TextAnchor.MiddleRight;
        else
            message.verticalLayout.childAlignment = TextAnchor.MiddleLeft;
    }

    private void ShowChat(List<PrivateMessageDto> messageList)
    {
        foreach (PrivateMessageDto message in messageList)
        {
            PrivateMessage privateMessage = JsonConvert.DeserializeObject<PrivateMessage>(message.Text);
            bool isMine = message.SenderId != recipientId;
            switch (privateMessage.messageType)
            {
                case QuickMessage.Text:
                    CreateTextMessage(message.Id, privateMessage.messageText, isMine);
                    break;
                default:
                    CreateEmojiMessage(privateMessage.messageType, message.Id, isMine);
                    break;
            }
        }
    }

    private void Awake()
    {
        SetTexts();
        _sendButton.onClick.AddListener(SendTextMessage);
        _quitButton.onClick.AddListener(delegate { Destroy(gameObject); });
        FriendsPanelController.OnUpdateChatView += ReceiveMessage;
        for (int i = 0; i < emojiPanel.transform.childCount; ++i)
            emojiPanel.transform.GetChild(i).GetComponent<EmojiButton>().SetClickAction(EmojiButtonClick);
    }

    private void Start() => ShowChat(ReceiveFriendsDataFromServer.allChatsDictionary[recipientId]);

    private void OnDestroy() => FriendsPanelController.OnUpdateChatView -= ReceiveMessage;

    private void SetTexts()
    {
        _personalChat.text = SettingsManager.Instance.GetString("FriendsPanel.PersonalChat");
        _sendMessage.text = SettingsManager.Instance.GetString("FriendsPanel.SendTextMessage");
        _enterMessage.text = SettingsManager.Instance.GetString("FriendsPanel.EnterMessage");
    }

    public void SendEmojiButtonClick() => emojiPanel.SetActive(!emojiPanel.activeInHierarchy);

    private void CreateEmojiMessage(QuickMessage emojiType, Guid id, bool isMine)
    {
        ChatEmoji emoji = Instantiate(emojiObject, contentWindow);
        emoji.SetEmojiType(emojiType);
        emoji.ID = id;
        if (isMine)
            emoji.verticalLayout.childAlignment = TextAnchor.MiddleRight;
        else
            emoji.verticalLayout.childAlignment = TextAnchor.MiddleLeft;
    }
    #endregion

    private void EmojiButtonClick(QuickMessage emojiType)
    {
        emojiPanel.SetActive(false);
        SendEmojiMessage(emojiType);
    }

    private void SendTextMessage()
    {
        if (string.IsNullOrEmpty(_messageInputField.text))
            return;
        var messageList = ReceiveFriendsDataFromServer.allChatsDictionary[recipientId];
        string messageString = JsonConvert.SerializeObject(new PrivateMessage() { messageType = QuickMessage.Text, messageText = _messageInputField.text });
        PrivateMessageDto message = new PrivateMessageDto
        {
            SenderId = PlayerProfileData.Instance.Id,
            SendTime = DateTime.Now,
            Text = messageString,
            Id = Guid.NewGuid()
        };
        CreateTextMessage(message.Id, _messageInputField.text, true);
        messageList.Add(message);
        SendPrivateMessage(PlayerProfileData.Instance.Id, recipientId, messageString, message.Id);
        _messageInputField.text = string.Empty;
    }

    private void SendEmojiMessage(QuickMessage emojiType)
    {
        var messageList = ReceiveFriendsDataFromServer.allChatsDictionary[recipientId];
        string messageString = JsonConvert.SerializeObject(new PrivateMessage() { messageType = emojiType });
        PrivateMessageDto message = new PrivateMessageDto
        {
            SenderId = PlayerProfileData.Instance.Id,
            SendTime = DateTime.Now,
            Text = messageString,
            Id = Guid.NewGuid()
        };
        CreateEmojiMessage(emojiType, message.Id, true);
        messageList.Add(message);
        SendPrivateMessage(PlayerProfileData.Instance.Id, recipientId, messageString, message.Id);
    }

    private void ReceiveMessage(PrivateMessageDto privateMessageDto)
    {
        PrivateMessage privateMessage = JsonConvert.DeserializeObject<PrivateMessage>(privateMessageDto.Text);
        bool isMine = privateMessageDto.SenderId != recipientId;
        switch (privateMessage.messageType)
        {
            case QuickMessage.Text:
                CreateTextMessage(privateMessageDto.Id, privateMessage.messageText, isMine);
                break;
            default:
                CreateEmojiMessage(privateMessage.messageType, privateMessageDto.Id, isMine);
                break;
        }
        ReceiveFriendsDataFromServer.allChatsDictionary[recipientId].Add(privateMessageDto);
        if (--ReceiveFriendsDataFromServer.allUnreadMessagesDictionary[privateMessageDto.SenderId] == 0)
            ReceiveFriendsDataFromServer.allUnreadMessagesDictionary.Remove(privateMessageDto.SenderId);
        OnMessageRead?.Invoke();
    }

    public static void SendPrivateMessage(Guid senderJson, Guid recipientJson, string messageText, Guid messageID)
    {
        var senderIdJson = JsonConvert.SerializeObject(senderJson);
        var recipientIdJson = JsonConvert.SerializeObject(recipientJson);
        var messageIDJson = JsonConvert.SerializeObject(messageID);
        Hub.SendAsync(ServerMethods.SendPrivateMessage, senderIdJson, recipientIdJson, messageText, messageIDJson);
        print($"{DateTime.UtcNow} SendTextMessage from {senderJson} to {recipientJson}");
    }
}