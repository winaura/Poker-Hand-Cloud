using PokerHand.Common.Helpers.QuickChat;
using UnityEngine;
using UnityEngine.UI;

public class QuickMessageObj : MonoBehaviour
{
    [SerializeField] Text label;

    void Start() => Destroy(gameObject, GameConstants.QuickMessageLifetime);

    public void SetText(QuickMessage message) => label.text = SettingsManager.Instance.GetString($"Phrase.{message}");
}