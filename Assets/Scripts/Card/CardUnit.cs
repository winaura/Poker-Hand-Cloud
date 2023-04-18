using UnityEngine;

public class CardUnit : MonoBehaviour
{
    public bool isUse;
    public SpriteRenderer front;
    public SpriteRenderer back;
    public SpriteRenderer border;
    public Card card;
    public AudioSource audioSource;

    public void SetData(Card card)
    {
        this.card = card;
        front.sprite = card.Front;
        back.sprite = card.Back;
        isUse = true;
    }

    public void SetData(Card card, Vector3 point, Quaternion direction)
    {
        this.card = card;
        front.sprite = card.Front;
        back.sprite = card.Back;
        transform.position = point;
        transform.rotation = direction;
        isUse = true;
    }

    public void SetColor(Color frontColor) => front.color = frontColor;

    public void Clear()
    {
        card = null;
        isUse = false;
        back.enabled = true;
        transform.position = GameConstants.DefaultOutOfViewPosition;
        front.color = GameConstants.WhiteNotTransparentColor;
        front.sortingOrder = 1;
        border.sortingOrder = 0;
        back.sortingOrder = 6;
    }

    public void IsShowCardBack(bool value) => back.enabled = !value;
}