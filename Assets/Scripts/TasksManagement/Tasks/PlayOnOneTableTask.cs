using UnityEngine;

[System.Serializable]
public class PlayOnOneTableTask : DailyTask
{
    [SerializeField] public int _requiredGamesAmount;
    [HideInInspector] public int _currentGamesAmount;
    [HideInInspector] public TablesInWorlds _lastTable;

    public void IncrementGamesAmount(TablesInWorlds currentTable)
    {
        if(_lastTable != currentTable)
        {
            _lastTable = currentTable;
            _currentGamesAmount = 1;
            return;
        }
        _currentGamesAmount++;
        if (_currentGamesAmount >= _requiredGamesAmount)
            IncrementAmount();
        TasksTracker.Instance.UpdateTasksOnServer();
    }

    public override object Clone()
    {
        return new PlayOnOneTableTask()
        {
            taskName = taskName,
            chipsRewardValue = chipsRewardValue,
            bigcoinsRewardValue = bigcoinsRewardValue,
            requiredAmount = requiredAmount,
            hoursToUpdateTask = hoursToUpdateTask,
            _requiredGamesAmount = _requiredGamesAmount,
            creationTime = TimeManager.GetTime()
        };
    }
}
