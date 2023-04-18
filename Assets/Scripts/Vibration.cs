using UnityEngine;

public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"); // ###
#elif UNITY_EDITOR
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    public static void Vibrate()
    {
#if UNITY_ANDROID
        vibrator.Call("vibrate");
#elif UNITY_IOS
            Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID
        vibrator.Call("vibrate", milliseconds);
#elif UNITY_IOS
            Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
#if UNITY_ANDROID
        vibrator.Call("vibrate", pattern, repeat);
#elif UNITY_IOS
            Handheld.Vibrate();
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID
        vibrator.Call("cancel");
#endif
    }
}