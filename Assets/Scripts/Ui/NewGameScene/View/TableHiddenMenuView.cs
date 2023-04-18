using System;
using UnityEngine;
using UnityEngine.UI;

public class TableHiddenMenuView : MonoBehaviour
{
    [SerializeField] private Button _closeBackground;
    [SerializeField] private Button _exitToLobby;
    [SerializeField] private Button _leaveTable;
    [SerializeField] private Button _switchTable;
    [SerializeField] private Button _settingsWindow;
    [SerializeField] private Button _inventory;
    public event Action OnExitLobby;
    public event Action OnLeaveTable;
    public event Action OnSwitchTable;
    public event Action OnSettingsTable;
    public event Action OnAllWindowClose;
    public event Action OnInventoryPressed;

    private void Awake()
    {
        _exitToLobby.onClick.AddListener(() => OnExitLobby?.Invoke());
        _leaveTable.onClick.AddListener(() => OnLeaveTable?.Invoke());
        _switchTable.onClick.AddListener(() => OnSwitchTable?.Invoke());
        _settingsWindow.onClick.AddListener(() => OnSettingsTable?.Invoke());
        _inventory.onClick.AddListener(() => OnInventoryPressed?.Invoke());
        _closeBackground.onClick.AddListener(CloseWindows);
    }

    private void CloseWindows()
    {
        gameObject.SetActive(false);
        _closeBackground.gameObject.SetActive(false);
        OnAllWindowClose?.Invoke();
    }

    public void OpenHiddenMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        _closeBackground.gameObject.SetActive(!_closeBackground.gameObject.activeSelf);
    }   
}
