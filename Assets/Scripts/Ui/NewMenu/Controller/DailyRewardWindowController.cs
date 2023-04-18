using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardWindowController : MonoBehaviour
{
    private DailyReward _dailyReward = new DailyReward();

    public void OpenWindow()
    {
        if (_dailyReward.IsReadyToCollect())
        {
            Client.AddChips(_dailyReward.CurrentReward);
            DailyRewardWindowView.CreateWindow
            (
                SettingsManager.Instance.GetString("DailyReward.ComeBack"),
                SettingsManager.Instance.GetString("DailyReward.Dailyreward"),
                SettingsManager.Instance.GetString("DailyReward.Claim"),
                _dailyReward.CurrentReward.IntoCluttered()
            );
        }
    }
}
