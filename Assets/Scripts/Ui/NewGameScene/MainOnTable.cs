using UnityEngine;
using UnityEngine.SceneManagement;

public class MainOnTable : MonoBehaviour
{
    private TablePlayerPanelController _playerPanel;
    private TableHiddenMenuController _hiddenMenuController;
    private TableWindowMoneyController _moneyWindowController;
    private ShopController _shopController;
    private SpinController _spinController;
    private SettingsController _settingsController;
    private PlayersViewController _playersViewController;

    private void Awake()
    {
        _playerPanel = GetComponent<TablePlayerPanelController>();
        _shopController = GetComponent<ShopController>();
        _spinController = GetComponent<SpinController>();
        _hiddenMenuController = GetComponent<TableHiddenMenuController>();
        _settingsController = GetComponent<SettingsController>();
        _moneyWindowController = GetComponent<TableWindowMoneyController>();
        _playersViewController = GetComponent<PlayersViewController>();
    }

    private void OnEnable()
    {
        _spinController.OnPrepareToSpin += () => _shopController.ToSpin();
        _spinController.OnSpinFinally += () => _playerPanel.UpdateTexts();
        _playerPanel.OnShopButtonClick += () => _shopController.OpenWindow();
        _playerPanel.OnSettingButtonClick += () => _hiddenMenuController.OpenHiddenMenu();
        _hiddenMenuController.OnSettingsPressed += () => _settingsController.OpenWindow();
        _hiddenMenuController.OnAllWindowClose += () => _settingsController.CloseWindow();
        _hiddenMenuController.OnExitLobby += GoToLobby;
        _hiddenMenuController.OnLeaveTable += GoToMain;
        _moneyWindowController.OnCloseButton += GoToMain;
    }

    private void OnDisable()
    {
        _spinController.OnPrepareToSpin -= () => _shopController.ToSpin();
        _spinController.OnSpinFinally -= () => _playerPanel.UpdateTexts();
        _playerPanel.OnShopButtonClick -= () => _shopController.OpenWindow();
        _playerPanel.OnSettingButtonClick -= () => _hiddenMenuController.OpenHiddenMenu();
        _hiddenMenuController.OnSettingsPressed -= () => _settingsController.OpenWindow();
        _hiddenMenuController.OnAllWindowClose -= () => _settingsController.CloseWindow();
        _hiddenMenuController.OnExitLobby -= GoToLobby;
        _hiddenMenuController.OnLeaveTable -= GoToMain;
        _moneyWindowController.OnCloseButton -= GoToMain;
    }

    private void GoToLobby()
    {
        if (GameModeSettings.Instance != null)
            GameModeSettings.Instance.world = Worlds.None;
        GoToMain();
    }

    private void GoToMain() => SceneManager.LoadScene(0);
}
