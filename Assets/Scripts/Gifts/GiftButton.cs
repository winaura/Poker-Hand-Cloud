using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GiftButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI costLabel;

    public void Init(int cost, Sprite sprite)
    {
        image.sprite = sprite;
        costLabel.text = cost.IntoCluttered();
    }
}