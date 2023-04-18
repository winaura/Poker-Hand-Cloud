using PokerHand.Common.Helpers.QuickChat;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatEmoji : FriendMessage
{
    [SerializeField] private Animator emojiAnimator;

    public void SetEmojiType(QuickMessage emojiType) => emojiAnimator.SetTrigger(emojiType.ToString());
}
