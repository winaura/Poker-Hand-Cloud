using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayHandsTask : DailyTask
{
    public override object Clone()
    {
        return new PlayHandsTask()
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
