using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableHiddenMenuController : MonoBehaviour
{
    [SerializeField] private TableHiddenMenuView _hiddenMenuView;
    public event Action OnSettingsPressed;
    public event Action OnAllWindowClose;
    public event Action OnExitLobby;
    public event Action OnLeaveTable;

    private void OnEnable()
    {
        _hiddenMenuView.OnSwitchTable += ChangeTable;
        _hiddenMenuView.OnExitLobby += () => OnExitLobby?.Invoke();
        _hiddenMenuView.OnLeaveTable += () => OnLeaveTable?.Invoke();
        _hiddenMenuView.OnSettingsTable += () => OnSettingsPressed?.Invoke();
        _hiddenMenuView.OnAllWindowClose += () => OnAllWindowClose?.Invoke();
    }

    private void OnDisable()
    {
        _hiddenMenuView.OnSwitchTable -= ChangeTable;
        _hiddenMenuView.OnExitLobby -= () => OnExitLobby?.Invoke();
        _hiddenMenuView.OnLeaveTable -= () => OnLeaveTable?.Invoke();
        _hiddenMenuView.OnSettingsTable -= () => OnSettingsPressed?.Invoke();
        _hiddenMenuView.OnAllWindowClose -= () => OnAllWindowClose?.Invoke();
    }

    private void ChangeTable() => SceneManager.LoadScene(1);

    public void OpenHiddenMenu() => _hiddenMenuView.OpenHiddenMenu();
}