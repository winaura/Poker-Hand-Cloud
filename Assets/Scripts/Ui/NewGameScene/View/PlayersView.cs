using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersView : MonoBehaviour
{
    [Header("5 Players"), SerializeField] private GameObject _fivePlayerObject;
    [SerializeField] private Text _waitTextFivePeople;
    [SerializeField] private Text _comboTextFivePeople;
    [SerializeField] private List<GameObject> _fiveTimerImagePlayer;
    [SerializeField] private List<Text> _fiveNamePlayer;
    [SerializeField] private List<Text> _fiveMoneyPlayer;
    [SerializeField] private List<Text> _fivePlayersBet;
    [SerializeField] private List<Text> _fivePlayersBlindText;
    [SerializeField] private List<GameObject> _fivePlayersChip;
    [Header("8 Players"), SerializeField] private GameObject _eightPlayerObject;
    [SerializeField] private Text _waitTextEightPeople;
    [SerializeField] private Text _comboTextEightPeople;
    [SerializeField] private List<GameObject> _eightTimerImagePlayer;
    [SerializeField] private List<Text> _eightNamePlayer;
    [SerializeField] private List<Text> _eightMoneyPlayer;
    [SerializeField] private List<Text> _eightPlayersBet;
    [SerializeField] private List<Text> _eightPlayersBlindText;
    [SerializeField] private List<GameObject> _eightPlayersChip;
    private int _playersCount;
    private GameObject _currentGameObjectImage;
    private Image _currentImage;
    private int _countDownTimer;
    private int _currentSliderMoney;

    public void SetBlindPrice((int, int) obj)
    {
        if (_playersCount == 8)
        {
            _eightPlayersBlindText[obj.Item2].gameObject.SetActive(true);
            _eightPlayersBlindText[obj.Item2].text = $"{obj.Item1}";
        }
        else
        {
            _fivePlayersBlindText[obj.Item2].gameObject.SetActive(true);
            _fivePlayersBlindText[obj.Item2].text = $"{obj.Item1}";
        }
    }

    public void ResetBlindsPrice()
    {
        if (_playersCount == 8)
            for (int i = 0; i < _eightPlayersBlindText.Count; i++)
                _eightPlayersBlindText[i].gameObject.SetActive(false);
        else
            for (int i = 0; i < _fivePlayersBlindText.Count; i++)
                _fivePlayersBlindText[i].gameObject.SetActive(false);
    }

    public void ResetPlayerBetPrice()
    {
        if (_playersCount == 8)
            for (int i = 0; i < _eightPlayersBet.Count; i++)
                _eightPlayersBet[i].text = "";
        else
            for (int i = 0; i < _fivePlayersBet.Count; i++)
                _fivePlayersBet[i].text = "";
    }    

    private void ChangeTimerImage(int index)
    {
        if (_playersCount == 8)
        {
            for (int i = 0; i < _eightTimerImagePlayer.Count; i++)
                _eightTimerImagePlayer[i].gameObject.SetActive(false);
            _currentGameObjectImage = _eightTimerImagePlayer[index];
        }
        else
        {
            for (int i = 0; i < _fiveTimerImagePlayer.Count; i++)
                _fiveTimerImagePlayer[i].gameObject.SetActive(false);
            _currentGameObjectImage = _fiveTimerImagePlayer[index];
        }
        _currentGameObjectImage.gameObject.SetActive(true);
        _currentImage = _currentGameObjectImage.GetComponent<Image>();
    }

    public void UpdatePlayersName(int index, string name)
    {
        if (_playersCount == 8)
            _eightNamePlayer[index].text = name;
        else
            _fiveNamePlayer[index].text = name;
    }

    public void UpdatePlayersMoney(int index, int count)
    {
        if (_playersCount == 8)
            _eightMoneyPlayer[index].text = $"${count}";
        else
            _fiveMoneyPlayer[index].text = $"${count}";
    }

    private void SetWaitText(bool state)
    {
        if (_playersCount == 8)
            _waitTextEightPeople.gameObject.SetActive(state);
        else
            _waitTextFivePeople.gameObject.SetActive(state);
    }

    private void SetPlayerCombinationText(string Value)
    {
        if (_playersCount == 8)
            _comboTextEightPeople.text = Value;
        else
            _comboTextFivePeople.text = Value;
    }

    public void SetPlayerBet((int, int) obj)
    {
        if (_playersCount == 8)
        {
            if (obj.Item1 > 0)
            {
                _eightPlayersBet[obj.Item2].text = $"{obj.Item1}";
                _eightPlayersBet[obj.Item2].transform.parent.gameObject.SetActive(true);
                _eightPlayersChip[obj.Item2].gameObject.SetActive(true);
            }
        }
        else
        {
            if (obj.Item1 > 0)
            {
                _fivePlayersBet[obj.Item2].text = $"{obj.Item1}";
                _fivePlayersBet[obj.Item2].transform.parent.gameObject.SetActive(true);
                _fivePlayersChip[obj.Item2].gameObject.SetActive(true);
            }
        }
    }

    public void SetPlayers(int value)
    {
        _playersCount = value;
        _fivePlayerObject.SetActive(_playersCount == 5);
        _eightPlayerObject.SetActive(_playersCount == 8);
    }
}
