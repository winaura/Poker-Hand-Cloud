using System.Collections;
using UnityEngine;

public class MoneyBoxWindowController : Singleton<MoneyBoxWindowController>
{
    [SerializeField] private MoneyBoxWindowView _moneyBoxWindowView;

    private void OnEnable() => _moneyBoxWindowView.OnCloseButtonClick += CloseWindow;

    private void OnDisable() => _moneyBoxWindowView.OnCloseButtonClick -= CloseWindow;

    public void OnBreakButtonClick()
    {
        _moneyBoxWindowView.BreakMoneyBox();
        StartCoroutine(Delay(6f));
    }

    public void CloseWindow() => _moneyBoxWindowView.gameObject.SetActive(false);

    public void OpenWindow() => _moneyBoxWindowView.gameObject.SetActive(true);

    public IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _moneyBoxWindowView.gameObject.SetActive(false);
        TryGetComponent(out PlayerPanelController playerPanelController);
        TryGetComponent(out MainLobbyController mainLobbyController);
        playerPanelController.UpdateData();
        mainLobbyController.UpdateLobbyTexts();
    }
}
