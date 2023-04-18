using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DailyEventsWindowView : MonoBehaviour
{
    [SerializeField] private Button _closeWindowButton;
    [SerializeField] private Button _receiveAllButton;
    [SerializeField] private List<DailyEventElement> _eventElementsList;
    public event Action OnCloseButtonPressed;
    public event Action OnReceiveAllButtonPressed;
    [SerializeField] private Text _dailyGoalsText;
    [SerializeField] private Text _receiveAllText;

    private void Awake()
    {
        _receiveAllButton.onClick.AddListener(() => OnReceiveAllButtonPressed.Invoke());
        _closeWindowButton.onClick.AddListener(() => OnCloseButtonPressed.Invoke());
        DailyEventsWindowController.OnDailyEventComplete += UpdateReceiveAllButton;
    }

    private void OnDestroy() => DailyEventsWindowController.OnDailyEventComplete -= UpdateReceiveAllButton;

    public void SetTasks()
    {
        for (int i = 0; i < TasksTracker.Instance.tasks.Length; ++i)
            _eventElementsList[i]._currentTask = TasksTracker.Instance.tasks[i];
    }
    public void UpdateWindow()
    {
        _dailyGoalsText.text = SettingsManager.Instance.GetString("DailyGoals.DailyGoals");
        _receiveAllText.text = SettingsManager.Instance.GetString("DailyGoals.ReceiveAll");
        foreach (var element in _eventElementsList)
            element.UpdateElementView();
    }
    public void ReceiveAllRewards()
    {
        foreach(var element in _eventElementsList)
            element.OnCollectReward();
    }

    public void UpdateReceiveAllButton() 
        => _receiveAllButton.interactable = _eventElementsList.Any(t => t._currentTask.IsDone == true && t._currentTask.IsCollectedReward == false);
}