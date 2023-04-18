using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindsView : MonoBehaviour
{
    [SerializeField] private GameObject _dealer, _smallBlind, _bigBlind;
    [SerializeField] private List<Transform> _fivePlayerChipPosition;
    [SerializeField] private List<Transform> _eightPlayerChipPosition;
    private int _index = 1;
    private int _blindIndex;

    public void SetDealer(int playersInGames, int tablesPlayerCount)
    {
        if (playersInGames > 2)
        {
            _dealer.transform.position = tablesPlayerCount == 8
                ? _eightPlayerChipPosition[_blindIndex].transform.position
                : _fivePlayerChipPosition[_blindIndex].transform.position;
        }
        else
            _dealer.transform.position = Vector3.right * 100;
        IncreaseBlindIndex(tablesPlayerCount);
    }

    public void SetBlinds(int playersInGame, int tablesPlayerCount)
    {
        if (playersInGame < 2) 
            return;
        if (tablesPlayerCount == 8)
        {
            _bigBlind.transform.position = _eightPlayerChipPosition[_index].transform.position;
            IncreaseIndex(tablesPlayerCount);
            _smallBlind.transform.position = _eightPlayerChipPosition[_index].transform.position;
            IncreaseIndex(tablesPlayerCount);
        }
        else
        {
            _bigBlind.transform.position = _fivePlayerChipPosition[_index].transform.position;
            IncreaseIndex(tablesPlayerCount);
            _smallBlind.transform.position = _fivePlayerChipPosition[_index].transform.position;
            IncreaseIndex(tablesPlayerCount);
        }
    }

    public void ResetBlinds()
    {
        _dealer.transform.position = Vector3.right * 100;
        _smallBlind.transform.position = Vector3.right * 100;
        _bigBlind.transform.position = Vector3.right * 100;
    }

    private void IncreaseBlindIndex(int peopleCount)
    {
        for (int i = 0; i < peopleCount; i++)
        {
            _blindIndex++;
            if (_blindIndex > peopleCount)
                _blindIndex = 0;
            if (peopleCount == 8)
            {
                if (_eightPlayerChipPosition[_blindIndex].parent.gameObject.activeSelf)
                    break;
            }
            else
            {
                if (_fivePlayerChipPosition[_blindIndex].parent.gameObject.activeSelf)
                    break;
            }
        }
    }

    private void IncreaseIndex(int peopleCount)
    {
        _index = _blindIndex;
        for (int i = 0; i < peopleCount; i++)
        {
            _index++;
            if (_index > peopleCount)
                _index = 0;
            if (peopleCount == 8)
            {
                if (_eightPlayerChipPosition[_index].parent.gameObject.activeSelf)
                    break;
            }
            else
            {
                if (_fivePlayerChipPosition[_index].parent.gameObject.activeSelf)
                    break;
            }
        }
    }
}
