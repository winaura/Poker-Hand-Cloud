using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Create tasks pool", menuName = "Create Tasks Pool")]
public class TasksPool : ScriptableObject
{
    [SerializeField] private TaskPoolInterval[] taskPoolIntervals;
    [SerializeField] private List<CombinationTask> _combinationTaskPool;
    [SerializeField] private List<WinChipsAmountTask> _winChipsAmountTaskPool;
    [SerializeField] private List<WinHandsTask> _winHandsTaskPool;
    [SerializeField] private List<PlayHandsTask> _playHandsTaskPool;
    [SerializeField] private List<PlayOnOneTableTask> _playOnOneTableTaskPool;
    [SerializeField] private List<PlayOnDifferentTablesTask> _playOnDifferentTablesTaskPool;
    [SerializeField] private List<AllInAndWinTask> _allInAndWinTaskPool;
    [SerializeField] private List<FoldWithCombinationTask> _foldWithCombinationTaskPool;
    [SerializeField] private List<PlayOnCertainModeTask> _playOnCertainModeTaskPool;
    private List<DailyTask> _tasks = new List<DailyTask>();
    public int TaskPoolIntervalsCount => taskPoolIntervals.Length;
    public void InitializeList()
    {
        _tasks = new List<DailyTask>();
        _tasks.AddRange(_combinationTaskPool);
        _tasks.AddRange(_winChipsAmountTaskPool);
        _tasks.AddRange(_winHandsTaskPool);
        _tasks.AddRange(_playHandsTaskPool);
        _tasks.AddRange(_playOnOneTableTaskPool);
        _tasks.AddRange(_playOnDifferentTablesTaskPool);
        _tasks.AddRange(_allInAndWinTaskPool);
        _tasks.AddRange(_foldWithCombinationTaskPool);
        _tasks.AddRange(_playOnCertainModeTaskPool);
    }

    public DailyTask GetRandomTaskFromPool(int poolNumber)
    {
        int index = Random.Range(taskPoolIntervals[poolNumber].firstElenentIndex, taskPoolIntervals[poolNumber].lastElementIndex);
        return (DailyTask)_tasks[index].Clone();
    }

    public DailyTask GetUpdatedTask(DailyTaskServerData serverTask)
    {
        DailyTask task = (DailyTask)_tasks.First(t => t.TaskName == serverTask.TaskName).Clone();
        task.UpdateTask(serverTask);
        return task;
    }


    [System.Serializable]
    public struct TaskPoolInterval
    {
        public int firstElenentIndex;
        public int lastElementIndex;
    }
}