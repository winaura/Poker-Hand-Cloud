using UnityEngine;
using UnityEngine.UI;

public class DailyEventElement : MonoBehaviour
{
    [SerializeField] private Text _taskText;
    [SerializeField] private Text _progressText;
    [SerializeField] private Text _chipsReward;
    [SerializeField] private Text _bigcoinsReward;
    [SerializeField] private Image _isDoneImage;
    [SerializeField] private Image _isCollectedImage;
    [SerializeField] private Image _goldBackgroundImage;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Button _collectRewardButton;
    [SerializeField] private Color _taskIsDoneTextColor;
    [SerializeField] private Color _taskTextColor;
    [HideInInspector] public DailyTask _currentTask;
    [SerializeField] private Text _getText;
    private bool isThisMethodReceived = false;

    private void Awake() => _collectRewardButton.onClick.AddListener(OnCollectReward);

    public void OnCollectReward()
    {
        if (!_currentTask.IsDone)
            return;
        SetIsCollectedView();
        _currentTask.CollectReward();
    }

    public void UpdateElementView()
    {
        _progressSlider.maxValue = _currentTask.RequiredAmount;
        _progressSlider.value = _currentTask.CurrentAmount;
        _taskText.text = SettingsManager.Instance.GetString(_currentTask.TaskName);
        _progressText.text = ((int)_progressSlider.value).IntoCluttered() + "/" + ((int)_progressSlider.maxValue).IntoCluttered();
        _taskText.color = _taskTextColor;
        _collectRewardButton.gameObject.SetActive(false);
        _getText.text = SettingsManager.Instance.GetString("DailyGoals.Get");
        SetReward();
        if (_currentTask.IsCollectedReward)
        {
            SetIsCollectedView();
            return;
        }
        if (_currentTask.IsDone)
        {
            SetIsDoneView();
            return;
        }
    }

    private void SetReward()
    {
        if (_currentTask.ChipsRewardValue != 0)
            _chipsReward.text = _currentTask.ChipsRewardValue.ToString();
        else
            _bigcoinsReward.text = _currentTask.BigcoinsRewardValue.ToString();
    }

    private void SetIsDoneView()
    {
        gameObject.transform.SetSiblingIndex(0);
        _collectRewardButton.gameObject.SetActive(true);
        _goldBackgroundImage.gameObject.SetActive(true);
        _taskText.color = _taskIsDoneTextColor;
        _progressText.color = _taskIsDoneTextColor;
    }

    private void SetIsCollectedView()
    {
        gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
        _isCollectedImage.gameObject.SetActive(true);
        _isCollectedImage.gameObject.SetActive(true);
        _isDoneImage.gameObject.SetActive(true);
        _collectRewardButton.gameObject.SetActive(false);
        _goldBackgroundImage.gameObject.SetActive(false);
        _taskText.color = _taskIsDoneTextColor;
        _progressText.color = _taskIsDoneTextColor;
    }
}

struct TotalMoneySerializer
{
    public string Message;
    public long Value;
}