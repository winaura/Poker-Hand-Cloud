using System;
using System.Net.Http;
using UnityEngine;

public class TimeManager
{
    public static DateTime GetTime()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return DateTime.UtcNow;
        using (var client = new HttpClient())
        {
            var response = System.Threading.Tasks.Task.Run(() => client.GetAsync(Hub.uriString));

            var responseResult = response.GetAwaiter().GetResult();

            var result = responseResult.Headers.Date.GetValueOrDefault();

            return result.DateTime;
        }
    }
}
 