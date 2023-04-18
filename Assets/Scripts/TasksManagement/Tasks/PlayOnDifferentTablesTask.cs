using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayOnDifferentTablesTask : DailyTask
{
    [HideInInspector] private List<TablesInWorlds> visitedTables = new List<TablesInWorlds>();

    public void AddVisitedTable(TablesInWorlds currentTable)
    {
        if (visitedTables.Contains(currentTable))
            return;
        visitedTables.Add(currentTable);
        IncrementAmount();
    }
    public override object Clone()
    {
        return new PlayOnDifferentTablesTask()
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
