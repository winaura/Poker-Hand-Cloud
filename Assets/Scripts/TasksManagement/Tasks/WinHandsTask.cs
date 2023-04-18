using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WinHandsTask : DailyTask
{
    public override object Clone()
    {
        return new WinHandsTask()
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
