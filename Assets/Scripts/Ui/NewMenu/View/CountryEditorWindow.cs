using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryEditorWindow : MonoBehaviour
{
    [SerializeField] private GameObject prefabs;
    [SerializeField] private Transform Content;
    [SerializeField] List<Image> _scrollViewElements;
    [SerializeField] Image _countryButtonImage;
    [SerializeField] GameObject _scrollView;
    [SerializeField] CountryData _countryData;

    private void Start()
    {
        InitScrollView();
        _countryButtonImage.sprite = _scrollViewElements[0].sprite;
    }
    private void InitScrollView()
    {
        var name = "";
        if (_scrollViewElements.Count != 0) _scrollViewElements.Clear();
        foreach (var sprite in _countryData.CountryDatas)
        {
            name += $"case '{sprite.name}': return return CountryCode.{sprite.name.ToUpper()};" + "\n";
            InitElement(sprite);
        }
        Debug.Log(name);
    }
    public void GetCountryPicture(Image image) => _countryButtonImage.sprite = image.sprite;

    private void InitElement(Sprite sprite)
    {
        var obj = Instantiate(prefabs);
        obj.transform.SetParent(Content, false);
        var flag = obj.GetComponent<FlagsElement>();
        flag.FirstInit(sprite, this);
        _scrollViewElements.Add(obj.GetComponent<Image>());
        //button.onClick.AddListener(GetCountryPicture(image));
    }

    private void OpenScrollView() => _scrollView.SetActive(true);

    private void CloseScrollView() => _scrollView.SetActive(false);
}
