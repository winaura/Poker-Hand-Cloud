using System;
using System.Globalization;
using UnityEngine;

public class DailyReward 
{
    private const int startReward = 3000;
    private const int hoursToUpdateReward = 24;
    public int CurrentReward => startReward;

    public bool IsReadyToCollect()
    {
        if(PlayerPrefs.HasKey("CollectedRewardTime"))
        {
            DateTime currentTime = TimeManager.GetTime();
            DateTime previousTime = DateTime.ParseExact(PlayerPrefs.GetString("CollectedRewardTime"), GameConstants.dateTimeFormat, CultureInfo.InvariantCulture);
            if (currentTime.Subtract(previousTime).TotalHours >= hoursToUpdateReward)
            {
                SaveCollectedTime(currentTime);
                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

    private void SaveCollectedTime(DateTime currentTime)
    {
        PlayerPrefs.SetString("CollectedRewardTime", currentTime.ToString(GameConstants.dateTimeFormat));
        Client.HTTP_UpdateLastDailyRewardTimeRequest();
    }
}