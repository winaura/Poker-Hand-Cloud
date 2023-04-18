using PokerHand.Common.Helpers.QuickChat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ChatTab { Phrases, Emojis }

public class ChatWindowView : MonoBehaviour
{
    [SerializeField] ChatTab defaultTab;
    [SerializeField] Color normalTabColor;
    [SerializeField] Color selectedTabColor;
    [Header("Phrase links")]
    [SerializeField] GameObject phrasesPanel;
    [SerializeField] Image phraseTabImage;
    [SerializeField] PhraseButton[] phraseButtons;
    [Header("Emojis links")]
    [SerializeField] GameObject emojisPanel;
    [SerializeField] Image emojisTabImage;
    [SerializeField] EmojiButton[] emojiButtons;
    public event Action OnCloseButtonClick;
    public event UnityAction<QuickMessage> OnPhraseButtonClick;
    Dictionary<ChatTab, Image> tabsImageDict = new Dictionary<ChatTab, Image>();

    private void Awake()
    {
        tabsImageDict.Add(ChatTab.Phrases, phraseTabImage);
        tabsImageDict.Add(ChatTab.Emojis, emojisTabImage);
        foreach (var pb in phraseButtons)
            pb.SetClickAction(OnPhraseButtonClickHandler);
        foreach (var eb in emojiButtons)
            eb.SetClickAction(OnPhraseButtonClickHandler);
        SetTab(defaultTab);
    }

    private void OnEnable()
    {
        foreach (var pb in phraseButtons)
        {
            var phraseText = SettingsManager.Instance.GetString($"Phrase.{pb.Phrase}");
            pb.SetText(phraseText);
        }
    }

    public void SetTab(ChatTab tab)
    {
        switch(tab)
        {
            case ChatTab.Phrases:
                phrasesPanel.SetActive(true);
                emojisPanel.SetActive(false);
                break;
            case ChatTab.Emojis:
                phrasesPanel.SetActive(false);
                emojisPanel.SetActive(true);
                break;
        }
        foreach (var ti in tabsImageDict)
            ti.Value.color = ti.Key == tab ? selectedTabColor : normalTabColor;
    }

    public void OnPhraseTabClick() => SetTab(ChatTab.Phrases);
    
    public void OnEmojisTabClick() => SetTab(ChatTab.Emojis);

    public void OnCloseButtonClickHandler() => OnCloseButtonClick?.Invoke();

    void OnPhraseButtonClickHandler(QuickMessage phrase) => OnPhraseButtonClick?.Invoke(phrase);
}