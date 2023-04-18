using UnityEngine;

public class PlayersViewController : MonoBehaviour
{
    [SerializeField] private PlayersView _view;

    private void Start()
    {
        int count = GameModeSettings.Instance ? GameModeSettings.Instance.countPeople : 5;
        _view.SetPlayers(count);
    }

    public void SetBlindPrice((int, int) data) => _view.SetBlindPrice(data);

    public void ResetBlindsPrice() => _view.ResetBlindsPrice();

    public void ResetPlayerBetPrice() => _view.ResetPlayerBetPrice();
}
