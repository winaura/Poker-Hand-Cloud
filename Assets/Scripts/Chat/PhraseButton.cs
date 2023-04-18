using PokerHand.Common.Helpers.QuickChat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhraseButton : MonoBehaviour
{
    [SerializeField] QuickMessage phrase;
    [SerializeField] Button button;
    [SerializeField] Text label;

    public QuickMessage Phrase => phrase;

    public void SetClickAction(UnityAction<QuickMessage> onClickAction) => button.onClick.AddListener(() => onClickAction(phrase));

    public void SetText(string text) => label.text = text;
}