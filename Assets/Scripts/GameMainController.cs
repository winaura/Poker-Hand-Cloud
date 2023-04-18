using UnityEngine;

public class GameMainController : MonoBehaviour
{
    private PlayerPanelController _playerPanelController;
    private PlayerProfileWindowController _playerProfileWindowController;
    private ShopController _shopController;
    private DailyEventsWindowController _dailyEventsWindowController;

    private void Awake()
    {
        _playerPanelController = GetComponent<PlayerPanelController>();
        _playerProfileWindowController = GetComponent<PlayerProfileWindowController>();
        _shopController = GetComponent<ShopController>();
        _dailyEventsWindowController = GetComponent<DailyEventsWindowController>();
    }

    private void OnEnable()
    {
        _playerPanelController.OnShopButtonClick += _shopController.OpenWindow;
        _playerPanelController.OnInfoButtonClick += () => _playerProfileWindowController.OpenWindow();
        _playerPanelController.OnDailyTasksButtonClick += _dailyEventsWindowController.OpenWindow;
    }

    private void OnDisable()
    {
        _playerPanelController.OnShopButtonClick -= _shopController.OpenWindow;
        _playerPanelController.OnInfoButtonClick -= () => _playerProfileWindowController.OpenWindow();
        _playerPanelController.OnDailyTasksButtonClick -= _dailyEventsWindowController.OpenWindow;
    }
}