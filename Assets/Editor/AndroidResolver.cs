using System;
using UnityEditor;

[InitializeOnLoad]
public class AndroidResolver
{
    static AndroidResolver() 
    {
        string newJDKPath = EditorApplication.applicationPath.Replace("Unity.exe", "Data/PlaybackEngines/AndroidPlayer/OpenJDK");

        if (Environment.GetEnvironmentVariable("JAVA_HOME") != newJDKPath)
        {
            Environment.SetEnvironmentVariable("JAVA_HOME", newJDKPath);
        }
    }
}
