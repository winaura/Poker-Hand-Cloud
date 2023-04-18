using System;
using UnityEngine;

public class DailyTask: ICloneable
{
    [SerializeField] protected string taskName;
    [SerializeField] protected int chipsRewardValue;
    [SerializeField] protected int bigcoinsRewardValue;
    [SerializeField] protected int requiredAmount = 1;
    [SerializeField] protected int hoursToUpdateTask = 24;
    protected bool isDone = false;
    protected bool isCollectedReward = false;
    protected long currentAmount = 0;
    protected DateTime collectedRewardTime;
    protected DateTime creationTime;

    public string TaskName => taskName;
    public bool IsDone => isDone;
    public bool IsCollectedReward => isCollectedReward;
    public long CurrentAmount => currentAmount;
    public DateTime CollectedRewardTime => collectedRewardTime;

    public void UpdateTask(DailyTaskServerData task)
    {
        isDone = task.IsDone;
        isCollectedReward = task.IsCollectedReward;
        currentAmount = task.CurrentAmount;
        collectedRewardTime = task.CollectedRewardTime;
        creationTime = task.CreationTime;
    }

    public TimeSpan TimeToNeedUpdate => creationTime.AddHours(hoursToUpdateTask) - TimeManager.GetTime();

    public bool NeedUpdate => IsCollectedReward && (TimeManager.GetTime() - collectedRewardTime).TotalHours >= hoursToUpdateTask;

    public int ChipsRewardValue => chipsRewardValue;
    public int BigcoinsRewardValue => bigcoinsRewardValue;
    public int RequiredAmount => requiredAmount;

    public DateTime CreationTime { get => creationTime; }

    public void IncrementAmount()
    {
        currentAmount++;
        if (currentAmount >= requiredAmount)
        {
            isDone = true;
            DailyEventsWindowController.DailyEventUpdate();
        }
        TasksTracker.Instance.UpdateTasksOnServer();
    }

    public void CollectReward()
    {
        if (!isDone || isCollectedReward || Application.internetReachability == NetworkReachability.NotReachable)
            return;
        isCollectedReward = true;
        collectedRewardTime = TimeManager.GetTime();
        Client.AddChips(chipsRewardValue);
        TasksTracker.Instance.UpdateTasksOnServer();
        TasksTracker.Instance.TrackTasksEnd();
        DailyEventsWindowController.DailyEventComplete();
        DailyEventsWindowController.DailyEventUpdate();
    }

    public virtual object Clone()
    {
        return new DailyTask()
        {
            taskName = taskName,
            chipsRewardValue = chipsRewardValue,
            bigcoinsRewardValue = bigcoinsRewardValue,
            requiredAmount = requiredAmount,
            hoursToUpdateTask = hoursToUpdateTask,
            creationTime = TimeManager.GetTime()
        };
    }
}
