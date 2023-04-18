using PokerHand.Common.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoldWithCombinationTask : DailyTask
{
    [SerializeField] public HandType combinationType;

    public override object Clone()
    {
        return new FoldWithCombinationTask()
        {
            taskName = taskName,
            chipsRewardValue = chipsRewardValue,
            bigcoinsRewardValue = bigcoinsRewardValue,
            requiredAmount = requiredAmount,
            hoursToUpdateTask = hoursToUpdateTask,
            combinationType = combinationType,
            creationTime = TimeManager.GetTime()
        };
    }
}
