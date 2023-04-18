using System.Collections.Generic;
using UnityEngine;
using static Client;
using PokerHand.Common.Helpers;

public enum ScreenId
{
    PlayerTurnScreen,
    PlayerWaitHisTurnScreen,
    AllElementScreen
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIScreen> _screens;
    [SerializeField] private AllElementScreen _allElementScreen;
    private UIScreen currentScreen;

    public UIScreen UpdateData() => _allElementScreen;

    public void StopCountdown() => _allElementScreen.StopAllCoroutines();

    public UIScreen OpenScreen(ScreenId screenId)
    {
       for (int i = 0; i < _screens.Count; i++)
            _screens[i].gameObject.SetActive(false);
        currentScreen = _screens[(int)screenId];
        currentScreen.gameObject.SetActive(true);
        return currentScreen;
    }

    public void DisableTimer() => _allElementScreen.DeactiveTimer();

    public void CloseAllScreen()
    {
        for (int i = 0; i < _screens.Count; i++)
            _screens[i].gameObject.SetActive(false);
    }
}