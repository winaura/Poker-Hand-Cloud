using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GivePresentTask : DailyTask
{
    public override object Clone()
    {
        return new GivePresentTask()
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
