using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _background;

    public void SetBackground(Sprite newBackground) => _background.sprite = newBackground;
}
