using System.Collections.Generic;

public enum MessageType { KickedAFK }

public static class MessagingSystem
{
    static List<MessageType> messages = new List<MessageType>(); // use stack as an alternative

    static public void AddMessage(MessageType message) => messages.Add(message);

    static public MessageType? GetFirstMessage()
    {
        if (messages == null || messages.Count == 0)
            return null;
        var message = messages[0];
        messages.RemoveAt(0);
        return message;
    }
}