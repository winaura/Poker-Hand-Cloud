using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class ConnectingWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectingText;
    private ConnectingType connectingType;
    private static ConnectingWindow windowObject = null;

    public static void CreateWindow(ConnectingType connectingType)
    {
        if (windowObject != null)
            DestroyWindow();
        windowObject = Instantiate(Resources.Load<ConnectingWindow>("CanvasConnecting"));
        windowObject.connectingType = connectingType;
        switch (connectingType)
        {
            case ConnectingType.ConnectingToTable:
                windowObject.connectingText.text = SettingsManager.Instance.GetString("MainMenu.ConnectingToTable");
                break;
            case ConnectingType.DisconnectingFromTable:
                windowObject.connectingText.text = SettingsManager.Instance.GetString("MainMenu.DisconnectingFromTable");
                break;
            case ConnectingType.Reconnecting:
                windowObject.connectingText.text = SettingsManager.Instance.GetString("Signing.Reconnecting");
                windowObject.gameObject.AddComponent<ReconnectionWindow>();
                break;
        }
    }

    public static void DestroyWindow()
    {
        if (windowObject == null)
            return;
        switch (windowObject.connectingType)
        {
            case ConnectingType.ConnectingToTable:
                Destroy(windowObject.gameObject);
                break;
            case ConnectingType.Reconnecting:
                windowObject.StopAllCoroutines();
                Destroy(windowObject.gameObject);
                Hub.SendAsync(ServerMethods.SendReconnectionToServer, JsonConvert.SerializeObject(PlayerProfileData.Instance.Id));
                break;
        }
    }

    private void OnDestroy() => windowObject = null;
}