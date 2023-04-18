using System;
using System.Text;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int ID { get; set; }
    public int playerGetMoney { get; set; }
    public string nickname;
    public int money;
    public int bitcoins;
    public LevelCounter level;
    public string visitedTables;
    public MoneyBox moneyBox;
    //Settings
    public Language language;
    public bool isVibrationOn;
    public bool isHandsOn;
    public bool isSoundOn;
    public bool isAllSoundOn;
    public bool isHintsOn;
    public string countryPicturePath;
    public string firstLogin;

    //Achievements
    public string bestHand;
    public int combinationValue;
    public int theBiggestWin;
    public int sitNGoWins;
    public int gamesCount;
    public int winsCount;
    public int dailyReward;
    public int sitNGoPlace = 5;
    public static PlayerData Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            moneyBox = new MoneyBox();
            LoadData();
            DontDestroyOnLoad(gameObject);
        }
        if (money < 0) 
            money = 0;
    }

    public void LoadData()  
    {
        money = PlayerPrefs.GetInt("Chips", 25000000);
        bitcoins = PlayerPrefs.GetInt("Bitcoins", 100000);
        countryPicturePath = PlayerPrefs.GetString("CountryPicturePath", "");
        firstLogin = PlayerPrefs.GetString("FirstLogin", DateTime.Now.ToString("dd/MM/yyyy"));
        nickname = PlayerPrefs.GetString("Nickname", "Unknown");
        gamesCount = PlayerPrefs.GetInt("GamesCount", 0);
        winsCount = PlayerPrefs.GetInt("WinsCount", 0);
        sitNGoWins = PlayerPrefs.GetInt("SitNGoWins", 0);
        theBiggestWin = PlayerPrefs.GetInt("TheBiggestWin", 0);
        bestHand = PlayerPrefs.GetString("BestHand", "");
        combinationValue = 0;
        PlayerPrefs.SetInt("CombinationValue", combinationValue);
        isVibrationOn = PlayerPrefs.GetInt("IsVibrationOn", 1) == 1 ? true : false;
        isHandsOn = PlayerPrefs.GetInt("IsHandsOn", 1) == 1 ? true : false;
        isHintsOn = PlayerPrefs.GetInt("IsHintsOn", 1) == 1 ? true : false;
        isSoundOn = PlayerPrefs.GetInt("IsSoundOn", 1) == 1 ? true : false;
        isAllSoundOn = PlayerPrefs.GetInt("IsAllSoundOn", 1) == 1 ? true : false;
        visitedTables = PlayerPrefs.GetString("VisitedTables", "00000000000000000000000000");
        language = PlayerPrefs.HasKey("Language") 
            ? (Language)PlayerPrefs.GetInt("Language")
            : LanguageParcer(Application.systemLanguage);

        moneyBox.Chips = PlayerPrefs.GetInt("MoneyBoxChips", 0);
    }
    public void SaveMoneyBox() => PlayerPrefs.SetInt("MoneyBoxChips", moneyBox.Chips);

    public void SaveLanguage(Language language)
    {
        this.language = language;
        PlayerPrefs.SetInt("Language", Convert.ToInt32(language));
    }
    public void SaveMoney()
    {
        PlayerPrefs.SetInt("Chips", money);
        PlayerPrefs.SetInt("Bitcoins", bitcoins);
    }

    public void SaveTable(int index)
    {
        StringBuilder someString = new StringBuilder(visitedTables);
        someString[index] = '1';
        visitedTables = someString.ToString();
        PlayerPrefs.SetString("VisitedTables", visitedTables);
    }

    public bool IsVisitedTable(int index) => !visitedTables[index].Equals('0');

    public void SaveNickname(string newNickname)
    {
        nickname = newNickname;
        PlayerPrefs.SetString("Nickname", nickname);
    }
    public void SaveCountry(string newCountryPucturePath)
    {
        countryPicturePath = newCountryPucturePath;
        PlayerPrefs.SetString("CountryPicturePath", newCountryPucturePath);
    }

    public void SaveGamesCount() => PlayerPrefs.SetInt("GamesCount", gamesCount);   
    
    public void SaveWinsCount() => PlayerPrefs.SetInt("WinsCount", winsCount);

    public void SaveBiggestWin() => PlayerPrefs.SetInt("TheBiggestWin", theBiggestWin);
    
    public void SaveSitNGoWins() => PlayerPrefs.SetInt("SitNGoWins", sitNGoWins);

    public void SaveBestHand(int combinationValue, string bestHand)
    {
        this.combinationValue = combinationValue;
        this.bestHand = bestHand;
        PlayerPrefs.SetInt("CombinationValue", this.combinationValue);
        PlayerPrefs.SetString("BestHand", this.bestHand);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("IsVibrationOn", Convert.ToInt32(isVibrationOn));
        PlayerPrefs.SetInt("IsHandsOn", Convert.ToInt32(isHandsOn));
        PlayerPrefs.SetInt("IsHintsOn", Convert.ToInt32(isHintsOn));
        PlayerPrefs.SetInt("IsSoundOn", Convert.ToInt32(isSoundOn));
        PlayerPrefs.SetInt("IsAllSoundOn", Convert.ToInt32(isAllSoundOn));

    }

    private Language LanguageParcer(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.English: return Language.English;
            case SystemLanguage.Russian: return Language.Russian;
            case SystemLanguage.French: return Language.French;
            default: return Language.English;
        }
    }
}
