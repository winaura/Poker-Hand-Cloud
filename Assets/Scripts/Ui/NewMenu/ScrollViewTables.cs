using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
struct LevelInfo
{
    public string levelName;
    public string levelDescription;
}

public class ScrollViewTables : MonoBehaviour
{
    private float _scrollPos;
    private float[] _pos = null;
    [SerializeField, Range(0, 1)] private float speed;
    [Header("Levels info"), SerializeField] LevelInfo[] levelsInfo;
    [Header("Links"), SerializeField] private Text _levelNameText;
    [SerializeField] private Text _levelDescText;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private Scrollbar _scrollBar;
    [SerializeField] private RectTransform[] contentObjects;
    [SerializeField] private GameObject mainLobby;
    int _curWorldNumber = 0;
    int curWorldNumber
    {
        get => _curWorldNumber;
        set
        {
            if (value < 0)
                _curWorldNumber = 0;
            else if (value >= levelsInfo.Length)
                _curWorldNumber = levelsInfo.Length - 1;
            else
                _curWorldNumber = value;
            GameModeSettings.Instance.world = (Worlds)_curWorldNumber;
            ScollViewWorlds._curIslandNumber = _curWorldNumber;
        }
    }
    float startPos;
    float endPos;
    float distance;
    float step;

    private void OnEnable() => mainLobby.SetActive(false);

    private void OnDisable() => mainLobby.SetActive(true);

    private void Start()
    {
        var pageWidth = GetComponent<RectTransform>().rect.width;
        foreach (var r in contentObjects)
            r.sizeDelta = new Vector2(pageWidth, r.rect.y);
        _leftArrow.onClick.AddListener(ClickLeft);
        _rightArrow.onClick.AddListener(ClickRight);
        SettingsManager.Instance.UpdateTextsEvent += SetLevelInfoText;
        if (_pos == null) // already calculated
            CalculatePositions();
        curWorldNumber = (int)GameModeSettings.Instance.world;
        SetWorld(curWorldNumber);
    }

    private void OnDestroy() => SettingsManager.Instance.UpdateTextsEvent -= SetLevelInfoText;

    public void SetWorld(int worldNumber)
    {
        curWorldNumber = worldNumber;
        if (_pos == null)
            CalculatePositions();
        _scrollBar.value = _pos[curWorldNumber];
        SetLevelInfoText();
    }

    private void Update()
    {
        WaitMouseButton();
        ChangeWorlds();
    }

    private void CalculatePositions()
    {
        step = 1f / levelsInfo.Length;
        _pos = new float[levelsInfo.Length];
        distance = 1f / (_pos.Length - 1);
        for (int i = 0; i < _pos.Length; i++)
            _pos[i] = distance * i;
    }

    private void ChangeWorlds()
    {
        for (int i = 0; i < _pos.Length; i++)
            if (_scrollPos < _pos[i] + (distance / 2) && _scrollPos > _pos[i] - (distance / 2))
                for (int j = 0; j < _pos.Length; j++)
                {
                    if (j != i)
                        CheckStateArrows();
                    else if (curWorldNumber != j)
                        SetLevelInfoText();
                }
    }

    private void WaitMouseButton()
    {
        _scrollPos = _scrollBar.value;
        if (Input.GetMouseButtonDown(0))
            startPos = _scrollBar.value;
        else if (Input.GetMouseButtonUp(0))
        {
            endPos = _scrollBar.value;
            var delta = endPos - startPos;
            if (Mathf.Abs(delta) > distance / 10)
            {
                curWorldNumber = (int)(_scrollBar.value / step);
                if (delta > 0)
                    curWorldNumber++;
                else
                    curWorldNumber--;
            }
        }
        else if (!Input.GetMouseButton(0))
            _scrollBar.value = Mathf.Lerp(_scrollBar.value, _pos[curWorldNumber], speed);
    }

    private void CheckStateArrows()
    {
        _leftArrow.gameObject.SetActive(curWorldNumber > 0);
        _rightArrow.gameObject.SetActive(curWorldNumber < levelsInfo.Length - 1);
    }

    private void ClickLeft()
    {
        if (curWorldNumber > 0)
            curWorldNumber--;
        else
            _leftArrow.gameObject.SetActive(false);
    }

    private void ClickRight()
    {
        if (curWorldNumber < levelsInfo.Length - 1)
            curWorldNumber++;
        else
            _rightArrow.gameObject.SetActive(false);
    }

    public void SetLevelInfoText()
    {
        _levelNameText.text = SettingsManager.Instance.GetString(levelsInfo[curWorldNumber].levelName);
        _levelDescText.text = SettingsManager.Instance.GetString(levelsInfo[curWorldNumber].levelDescription);
    }
}