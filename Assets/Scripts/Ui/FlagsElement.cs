using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagsElement : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    CountryEditorWindow window;

    public void FirstInit(Sprite sprite, CountryEditorWindow country)
    {
        window = country;
        _image.sprite = sprite;
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        window.GetCountryPicture(_image);
    }
}
