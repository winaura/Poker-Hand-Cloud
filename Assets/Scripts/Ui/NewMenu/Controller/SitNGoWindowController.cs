using System;
using UnityEngine;

public class SitNGoWindowController : MonoBehaviour
{
    public event Action OnEnterButton;
    [SerializeField] private SitNGoWindowView _sitNGoWindowView;

    private void Awake()
    {
        _sitNGoWindowView.OnEnterButton += EnterButtonClick;
        _sitNGoWindowView.OnCloseButtonPressed += CloseButtonClick;
        _sitNGoWindowView.OnTableInfoPressed += OnTableInfoPressed;
        StaticRuntimeSets.ItemsTypes.Add(typeof(SitNGoWindowController), this);
    }

    private void OnEnable()
    {
        if (GameModeSettings.Instance?.gameMode == GameModes.SitNGo)
            _sitNGoWindowView.SetCongratulationsWindow();
    }

    private void OnDestroy()
    {
        StaticRuntimeSets.ItemsTypes.Remove(typeof(SitNGoWindowController));
        _sitNGoWindowView.OnEnterButton -= EnterButtonClick;
        _sitNGoWindowView.OnCloseButtonPressed -= CloseButtonClick;
        _sitNGoWindowView.OnTableInfoPressed -= OnTableInfoPressed;
    }

    private void OnTableInfoPressed() => NotificationController.ShowNotification(GameModes.SitNGo);

    private void CloseButtonClick() => _sitNGoWindowView.gameObject.SetActive(false);

    private void EnterButtonClick()
    {
        long playerStackMoney = Client.MyPlayer == null ? 0 : Client.MyPlayer.StackMoney;
        if (PlayerProfileData.Instance.TotalMoney + playerStackMoney >= GameModeSettings.Instance.minMoneyGet)
        {
            PlayerProfileData.Instance.playerGetMoney = GameModeSettings.Instance.minMoneyGet;
            ConnectingWindow.CreateWindow(ConnectingType.ConnectingToTable);
            OnEnterButton?.Invoke();
        }
        else
        {
            TryGetComponent(out ShopController _shopController);
            _shopController.OpenWindow();
        }
    }

    public void OpenWindow()
    {
        _sitNGoWindowView.UpdateAllText(PlayerData.Instance, GameModeSettings.Instance);
        _sitNGoWindowView.gameObject.SetActive(true);
    }
}