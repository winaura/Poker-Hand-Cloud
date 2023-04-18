using UnityEngine;

public class MainLobbyController : MonoBehaviour
{
    [SerializeField] private MainLobbyView _mainLobbyView;
    private void Start()
    {
        UpdateLobbyTexts();
        SettingsManager.Instance.UpdateTextsEvent += UpdateLobbyTexts;
        _mainLobbyView.OnMoneyBoxButtonClick += OpenMoneyBoxWindow;
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= UpdateLobbyTexts;
        _mainLobbyView.OnMoneyBoxButtonClick -= OpenMoneyBoxWindow;
    }

    public void UpdateLobbyTexts()
    {
        if (PlayerProfileData.Instance.Id != System.Guid.Empty) 
            Client.ReceiveMoneyBoxAmount(PlayerProfileData.Instance.Id);
        _mainLobbyView.SetWindowTexts();
    }

    private void OpenMoneyBoxWindow()
    {
        if (PlayerProfileData.Instance.moneyBox.Chips == 0)
        {
            NotificationController.ShowNotification(NotificationController.NotificationType.MoneyBoxNothingToOpen, "");
            return;
        }
        TryGetComponent(out MoneyBoxWindowController moneyBoxWindowController);
        moneyBoxWindowController.OpenWindow();
    }
}