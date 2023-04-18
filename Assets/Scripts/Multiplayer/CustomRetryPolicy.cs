using BestHTTP.SignalRCore;
using System;
public class CustomRetryPolicy : IRetryPolicy
{
    public TimeSpan? GetNextRetryDelay(RetryContext context) => new TimeSpan(0, 0, 1);
}
