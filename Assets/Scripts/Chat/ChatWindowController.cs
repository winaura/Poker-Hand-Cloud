using PokerHand.Common.Helpers.QuickChat;
using UnityEngine;

public class ChatWindowController : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    [SerializeField] GameObject viewObjPrefab;
    GameObject viewObject = null;
    ChatWindowView view;

    public void Open()
    {
        if (viewObject == null)
        {
            viewObject = Instantiate(viewObjPrefab, parentTransform);
            view = viewObject.GetComponent<ChatWindowView>();
            view.OnCloseButtonClick += Close;
            view.OnPhraseButtonClick += ReleasePhrase;
        }
        viewObject.SetActive(true);
    }

    void ReleasePhrase(QuickMessage phrase)
    {
        Client.SendQuickMessage(phrase);
        Close();
    }

    void Close() => viewObject.SetActive(false);
}