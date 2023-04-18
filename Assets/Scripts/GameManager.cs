using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _tableBackground;
    [SerializeField] private TableData _tableData;
    private CardData _cardData;
    private GameModeSettings _gameModeSettings;
    private TableManager _tableManager;

    private void Start()
    {
        _cardData = GetComponent<CardData>();
        _tableManager = GetComponent<TableManager>();
        _gameModeSettings = GameModeSettings.Instance;
        MP_InitGame();
        AudioManager.Instance.PlayMusic(Clips.ChilloutMusic);
    }

    private void MP_InitGame()
    {
        if (_gameModeSettings != null)
        {
            _tableManager.CreateSession(_gameModeSettings.countPeople);
            switch (_gameModeSettings.gameMode)
            {
                case GameModes.Royal:
                    _tableManager.SetCardCollection(_cardData.CardDeckRoyal);
                    break;
                case GameModes.Joker:
                    _tableManager.SetCardCollection(_cardData.CardDeckJoker);
                    break;
                default:
                    _tableManager.SetCardCollection(_cardData.CardDeckTexas);
                    break;
            }
            _tableManager.MP_UpdatePlayersState();
            _tableManager.MP_UpdatePlayersInfo();
            if (_tableData != null || _tableBackground != null)
            {
                var tableIndex = (int)_gameModeSettings.TablesInWorlds;
                if (!PlayerData.Instance.IsVisitedTable(tableIndex))
                {
                    PlayerData.Instance.SaveTable(tableIndex);
                }
                _tableBackground.sprite = _tableData.GetTableSprite(tableIndex);
            }
        }
        else
        {
            _tableManager.CreateSession(5);
            _tableManager.SetCardCollection(_cardData.CardDeckTexas);
            _tableManager.MP_UpdatePlayersState();
            _tableManager.MP_UpdatePlayersInfo();
        }
        _tableManager.MP_InitGame();
    }
}