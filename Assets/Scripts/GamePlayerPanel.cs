using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPanel : MonoBehaviour
{
    [SerializeField] private DailyEventsWindowController _dailyEventsWindowController;
    private Button _dailyTasksButton;

    private void Awake()
    {
        _dailyTasksButton = GetComponent<Button>();
        _dailyTasksButton.onClick.AddListener(() => _dailyEventsWindowController.OpenWindow());
    }
}
