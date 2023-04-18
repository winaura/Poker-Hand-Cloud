[System.Serializable]
public class WinChipsAmountTask : DailyTask
{
    public void IncrementEarnedMoney(long _earnedChips)
    {
        currentAmount += _earnedChips;
        if(currentAmount >= requiredAmount)
        {
            currentAmount = requiredAmount;
            isDone = true;
            DailyEventsWindowController.DailyEventUpdate();
        }
        TasksTracker.Instance.UpdateTasksOnServer();
    }

    public override object Clone()
    {
        return new WinChipsAmountTask()
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
