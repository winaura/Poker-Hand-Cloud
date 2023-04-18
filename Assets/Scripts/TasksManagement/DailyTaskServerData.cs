using System;
[Serializable]
public class DailyTaskServerData
{
    public string TaskName { get; set; }
    public bool IsDone { get; set; }
    public bool IsCollectedReward { get; set; }
    public long CurrentAmount { get; set; }
    public DateTime CollectedRewardTime { get; set; }
    public DateTime CreationTime { get; set; }
}
