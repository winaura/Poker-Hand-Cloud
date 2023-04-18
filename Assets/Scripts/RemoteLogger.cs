using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class RemoteLogger : MonoBehaviour
{
    [SerializeField] private string serverURL;
    [SerializeField] private string[] loggingIDs;

    private static RemoteLogger Instance;
    private static string playerID = "";

    private void Awake()
    {
        playerID = PlayerPrefs.GetString("PlayerLoginId", "");
        if (Instance == null && !string.IsNullOrEmpty(serverURL) && CheckPlayerForLogging())
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private bool CheckPlayerForLogging() => loggingIDs.Contains(PlayerPrefs.GetString("PlayerLoginId", ""));

    private void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

    //Called when there is an exception
    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        LogData log = new LogData(condition, stackTrace, type);
        string json_log = JsonUtility.ToJson(log);
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Content-Type"] = "application/json";
        WWW www = new WWW(serverURL, System.Text.ASCIIEncoding.UTF8.GetBytes(json_log), headers);
    }

    [System.Serializable]
    public class LogData
    {
        public string device;
        public string condition;
        public string stackTrace;
        public LogType type;

        public LogData(string condition, string stackTrace, LogType type)
        {
            device = playerID;
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }
}