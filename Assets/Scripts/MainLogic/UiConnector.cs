using UnityEngine;

public class UiConnector : MonoBehaviour
{
    public static UiConnector instance;
    public ScreenData ScreenData { get; set; } = new ScreenData();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
