using PokerHand.Common.Helpers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TasksTracker : MonoBehaviour
{
    [SerializeField] TasksPool _tasksPool;
    [HideInInspector] public DailyTask[] tasks;
    private bool isWaitingToUpdate = false;
    public static TasksTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (Instance != null)
        {
            tasks = new DailyTask[_tasksPool.TaskPoolIntervalsCount];
            _tasksPool.InitializeList();
        }
    }

    public void LoadTasks(DailyTaskServerData[] serverTasks)
    {
        if (serverTasks.Length == 0)
        {
            for (int i = 0; i < _tasksPool.TaskPoolIntervalsCount; ++i)
                GenerateTask(i);
            UpdateTasksOnServer();
        }
        else
        {
            for (int i = 0; i < _tasksPool.TaskPoolIntervalsCount; ++i)
                tasks[i] = _tasksPool.GetUpdatedTask(serverTasks[i]);
        }
        DailyEventsWindowController.DailyEventUpdate();
        TrackTasksEnd();
    }

    private void GenerateTask(int poolIndex) => tasks[poolIndex] = _tasksPool.GetRandomTaskFromPool(poolIndex);

    public void UpdateTasksOnServer() => Client.HTTP_UpdateDailyTasksRequest(tasks);

    public bool IsAnyDone()
    {
        foreach (var el in tasks)
            if (el.IsDone && !el.IsCollectedReward)
                return true;
        return false;
    }

    public void TrackTasksEnd()
    {
        if (isWaitingToUpdate)
            return;
        isWaitingToUpdate = true;
        TimeSpan timeToUpdate = tasks.Min(t => t.TimeToNeedUpdate);
        Invoke("UpdateTasks", (float)timeToUpdate.TotalSeconds);
    }

    private void UpdateTasks()
    {
        isWaitingToUpdate = false;
        for (int i = 0; i < tasks.Length; ++i)
            GenerateTask(i);
        UpdateTasksOnServer();
        TrackTasksEnd();
    }

    #region FIRST POOL
    public void WinWithCombination(HandType combination)
    {
        if (!tasks[0].IsDone && tasks[0] is CombinationTask && (tasks[0] as CombinationTask).combinationType == combination)
            tasks[0].IncrementAmount();
    }
    #endregion

    #region SECOND POOL
    public void WinChips(long earnedChips)
    {
        if (!tasks[1].IsDone && tasks[1] is WinChipsAmountTask)
            (tasks[1] as WinChipsAmountTask).IncrementEarnedMoney(earnedChips);
    }
    public void WinHand()
    {
        if (!tasks[1].IsDone && tasks[1] is WinHandsTask)
            tasks[1].IncrementAmount();
    }
    public void PlayHand()
    {
        if (!tasks[1].IsDone && tasks[1] is PlayHandsTask)
            tasks[1].IncrementAmount();
    }
    public void PlayOnDifferentTables(TablesInWorlds currentTable)
    {
        if (!tasks[1].IsDone && tasks[1] is PlayOnDifferentTablesTask)
            (tasks[1] as PlayOnDifferentTablesTask).AddVisitedTable(currentTable);
    }
    public void PlayOnOneTable(TablesInWorlds currentTable)
    {
        if (!tasks[1].IsDone && tasks[1] is PlayOnOneTableTask)
            (tasks[1] as PlayOnOneTableTask).IncrementGamesAmount(currentTable);
    }
    #endregion

    #region THIRD POOL
    public void FoldWithCombination(HandType combination)
    {
        if (!tasks[2].IsDone && tasks[2] is FoldWithCombinationTask && (tasks[2] as FoldWithCombinationTask).combinationType == combination)
            tasks[2].IncrementAmount();
    }
    public void PlayCertainMode(GameModes gameMode)
    {
        if (!tasks[2].IsDone && tasks[2] is PlayOnCertainModeTask && (tasks[2] as PlayOnCertainModeTask)._requiredMode == gameMode)
            tasks[2].IncrementAmount();
    }
    public void WinAfterAllIn()
    {
        if (!tasks[2].IsDone && tasks[2] is AllInAndWinTask)
            tasks[2].IncrementAmount();
    }

    public void GivePresent()
    {
        if (!tasks[2].IsDone && tasks[2] is GivePresentTask)
            tasks[2].IncrementAmount();
    }
    #endregion
}
