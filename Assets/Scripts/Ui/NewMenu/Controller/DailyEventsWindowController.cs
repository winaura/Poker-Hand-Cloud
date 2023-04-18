using System;
using UnityEngine;

public class DailyEventsWindowController : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    [SerializeField] GameObject dailyEventsWindowViewPrefab;
    GameObject dailyEventsWindowViewObj = null;
    DailyEventsWindowView dailyEventsWindowView = null;
    public static event Action OnDailyEventComplete;
    public static event Action OnDailyEventUpdate;

    public static void DailyEventUpdate() => OnDailyEventUpdate?.Invoke();

    public static void DailyEventComplete() => OnDailyEventComplete?.Invoke();

    private void OnCloseButtonPressed() => dailyEventsWindowView.gameObject.SetActive(false);

    private void OnReceiveAllButtonPressed() => dailyEventsWindowView.ReceiveAllRewards();

    public void OpenWindow()
    {
        if (dailyEventsWindowViewObj == null)
        {
            dailyEventsWindowViewObj = Instantiate(dailyEventsWindowViewPrefab, parentTransform);
            dailyEventsWindowView = dailyEventsWindowViewObj.GetComponent<DailyEventsWindowView>();
            StartListenEvents();
        }
        dailyEventsWindowView.SetTasks();
        dailyEventsWindowView.UpdateWindow();
        dailyEventsWindowView.UpdateReceiveAllButton();
        dailyEventsWindowView.gameObject.SetActive(true);
    }

    private void StartListenEvents()
    {
        dailyEventsWindowView.OnCloseButtonPressed += OnCloseButtonPressed;
        dailyEventsWindowView.OnReceiveAllButtonPressed += OnReceiveAllButtonPressed;
    }
}