using UnityEngine;

public class BlindsController : MonoBehaviour
{
    [SerializeField] private BlindsView _view;

    public void ResetBlinds() => _view.ResetBlinds();

    public void SetDealer(int currentCountActivePlayers, int allCountPlayers) => _view.SetDealer(currentCountActivePlayers, allCountPlayers);

    public void SetBlinds(int currentCountActivePlayers, int allCountPlayers) => _view.SetBlinds(currentCountActivePlayers, allCountPlayers);
}
