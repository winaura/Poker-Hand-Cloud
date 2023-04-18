using PokerHand.Common.Helpers.QuickChat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EmojiButton : MonoBehaviour
{
    [SerializeField] QuickMessage emoji;
    [SerializeField] Button button;

    void OnEnable() => GetComponent<Animator>().SetTrigger(emoji.ToString());

    public QuickMessage Emoji => emoji;

    public void SetClickAction(UnityAction<QuickMessage> onClickAction) => button.onClick.AddListener(() => onClickAction(emoji));
}