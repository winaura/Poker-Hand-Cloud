using UnityEngine;

public class GameModeSettings : MonoBehaviour
{
    public int smallBlind { get; set; }
    public int bigBlind { get; set; }
    public int minMoneyGet { get; set; }
    public int maxMoneyGet { get; set; }
    public int maxPrizeSitNGo { get; set; }
    public int minPrizeSitNGo { get; set; }
    public int countPeople { get; set; }
    public int sitNGoMoneyGet { get; set; }
    public bool autoTop { get; set; } = true;
    public Worlds world { get; set; }
    public GameModes gameMode { get; set; }
    public float tableExperience { get; set; }
    public TablesInWorlds TablesInWorlds { get; set; }

    public static GameModeSettings Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            world = Worlds.None;
            DontDestroyOnLoad(gameObject);
        }
    }
}