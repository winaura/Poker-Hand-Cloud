using PokerHand.Common.Helpers.QuickChat;
using System.Collections;
using UnityEngine;

public class EmojiImageObj : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void RunWith(QuickMessage emoji)
    {
        switch(emoji)
        {
            case QuickMessage.Emoji_17:
                gameObject.transform.localScale = new Vector3(1.2f, 1, 1);
                break;
            case QuickMessage.Emoji_18:
                gameObject.transform.localScale = new Vector3(1.2f, 1, 1);
                break;
            case QuickMessage.Emoji_19:
                gameObject.transform.localScale = new Vector3(1.3f, 1.2f, 1);
                break;   
            case QuickMessage.Emoji_20:
                gameObject.transform.localScale = new Vector3(1.3f, 1, 1);
                break;
            default:
                gameObject.transform.localScale = Vector3.one;
                break;
        }
        gameObject.SetActive(true);
        StartCoroutine(ShowEmojiOnAvatarRoutine());
        IEnumerator ShowEmojiOnAvatarRoutine()
        {
            animator.SetTrigger(emoji.ToString());
            yield return GameConstants.WaitSeconds_3;
            gameObject.SetActive(false);
            StopCoroutine(ShowEmojiOnAvatarRoutine());
        }
    }
}