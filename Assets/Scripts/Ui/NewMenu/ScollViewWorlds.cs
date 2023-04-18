using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScollViewWorlds : MonoBehaviour
{
    private float _scrollPos;
    private float[] _pos;
    [SerializeField] private List<Image> _islandsImage;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private Scrollbar _scrollBar;
    [Header("WorldButtonsTexts"), SerializeField] private Text _chilloutText;
    [SerializeField] private Text _insomniaText;
    [SerializeField] private Text _undergroundText;
    [SerializeField] private Text _businessText;
    public static int _curIslandNumber = 0;
    int curIslandNumber
    {
        get => _curIslandNumber;
        set
        {
            if (value < 0)
                _curIslandNumber = 0;
            else if (value >= _islandsImage.Count)
                _curIslandNumber = _islandsImage.Count - 1;
            else
                _curIslandNumber = value;
        }
    }
    float distance;
    float startPos;
    float endPos;

    private void Start()
    {
        _leftArrow.onClick.AddListener(ClickLeft);
        _rightArrow.onClick.AddListener(ClickRight);
        SettingsManager.Instance.UpdateTextsEvent += SetWindowTexts;
        SetWindowTexts();
        _pos = new float[_islandsImage.Count];
        distance = 1f / (_pos.Length - 1);
        for (int i = 0; i < _pos.Length; i++)
            _pos[i] = distance * i;
    }
    private void OnDestroy() => SettingsManager.Instance.UpdateTextsEvent -= SetWindowTexts;

    private void Update()
    {
        WaitMouseButton(distance);
        ChangeWorlds(distance);
    }

    private void ChangeWorlds(float distance)
    {
        for (int i = 0; i < _pos.Length; i++)
            if (_scrollPos < _pos[i] + (distance / 2) && _scrollPos > _pos[i] - (distance / 2))
            {
                _islandsImage[i].gameObject.transform.parent.transform.localScale =
                    Vector2.Lerp(_islandsImage[i].gameObject.transform.parent.transform.localScale, Vector2.one, 0.1f);
                _islandsImage[i].color = Color.Lerp(_islandsImage[i].color, GameConstants.WhiteNotTransparentColor, 0.1f);
                for (int j = 0; j < _pos.Length; j++)
                    if (j != i)
                    {
                        CheckStateArrows();
                        _islandsImage[j].gameObject.transform.localScale
                            = Vector2.Lerp(_islandsImage[j].gameObject.transform.parent.transform.localScale, GameConstants.WorldsLerpVector, 0.1f);
                        _islandsImage[j].color = Color.Lerp(_islandsImage[j].color, GameConstants.WhiteTransparentColor, 0.1f);
                    }
            }
    }

    private void WaitMouseButton(float distance)
    {
        _scrollPos = _scrollBar.value;
        if (Input.GetMouseButtonDown(0))
            startPos = _scrollBar.value;
        else if (Input.GetMouseButtonUp(0))
        {
            endPos = _scrollBar.value;
            float delta = endPos - startPos;
            if (Mathf.Abs(delta) > distance / 10)
            {
                float step = 1f / _islandsImage.Count;
                curIslandNumber = (int)(_scrollBar.value / step);

                if (delta > 0)
                    curIslandNumber++;
                else
                    curIslandNumber--;
            }
        }
        else if (!Input.GetMouseButton(0))
            _scrollBar.value = Mathf.Lerp(_scrollBar.value, _pos[curIslandNumber], 0.1f);
    }

    private void CheckStateArrows()
    {
        _leftArrow.gameObject.SetActive(curIslandNumber > 0);
        _rightArrow.gameObject.SetActive(curIslandNumber < _islandsImage.Count - 1);
    }

    private void ClickLeft()
    {
        if (curIslandNumber > 0)
        {
            curIslandNumber--;
            _scrollPos = _pos[curIslandNumber];
        }
        else
            _leftArrow.gameObject.SetActive(false);
    }

    private void ClickRight()
    {
        if (curIslandNumber < _islandsImage.Count - 1)
        {
            curIslandNumber++;
            _scrollPos = _pos[curIslandNumber];
        }
        else
            _rightArrow.gameObject.SetActive(false);      
    }
    public void SetWindowTexts()
    {
        _chilloutText.text = SettingsManager.Instance.GetString("Worlds.Chillout");
        _insomniaText.text = SettingsManager.Instance.GetString("Worlds.Insomnia");
        _undergroundText.text = SettingsManager.Instance.GetString("Worlds.Underground");
        _businessText.text = SettingsManager.Instance.GetString("Worlds.Business");
    }
}