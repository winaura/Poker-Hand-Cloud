using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayOnCertainModeTask : DailyTask
{
    [SerializeField] public GameModes _requiredMode;

    public override object Clone()
    {
        return new PlayOnCertainModeTask
        {
            taskName = taskName,
            chipsRewardValue = chipsRewardValue,
            bigcoinsRewardValue = bigcoinsRewardValue,
            requiredAmount = requiredAmount,
            hoursToUpdateTask = hoursToUpdateTask,
            _requiredMode = _requiredMode,
            creationTime = TimeManager.GetTime()
        };
    }
}
