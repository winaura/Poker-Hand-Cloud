using PokerHand.Common.Dto;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using static Client;

// MP analogue of user data instead of single player class PlayerData
[CreateAssetMenu(fileName = "Assets/Resources/PlayerProfileData", menuName = "Multiplayer/PlayerProfileData")]
public class PlayerProfileData : ScriptableObject
{
    public Action OnProfileUpdated;
    public Action OnMyTotalMoneyUpdated;
    public Action OnExperienceUpdated;
    public static PlayerProfileData Instance { get; private set; }
    // PlayerProfileDto fields
    public Guid Id { get; set; } = Guid.Empty;
    public bool IsOnline { get; set; }
    public string Nickname { get; set; }
    public Gender Gender { get; set; }
    public CountryCode Country { get; set; }
    public HandsSpriteType HandsSpriteType { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int Experience { get; set; }
    public long TotalMoney { get; set; }
    public bool SitNGoStarted { get; set; }
    public int GamesPlayed { get; set; }
    public HandType BestHandType { get; set; }
    public int GamesWon { get; set; }
    public long BiggestWin { get; set; }
    public int SitAndGoWins { get; set; }
    public string PersonalCode { get; set; }
    public string BestHandName { get; private set; }
    public ExternalProviderName ExternalLogin { get; private set; }
    public ExternalProviderName LastLoginProvider { get; private set; } = ExternalProviderName.None;
    // Local data
    public long playerGetMoney { get; set; }
    public int MinMoneyNeed { get; set; }
    public string visitedTables;
    public MoneyBox moneyBox = new MoneyBox();
    // Settings
    public Language language;
    public bool isVibrationOn;
    public bool isHandsOn;
    public bool isSoundOn;
    public bool isAllSoundOn;
    public bool isHintsOn;
    public string picturePath;
    public string countryPicturePath;
    public string firstLogin;
    // Achievements
    public int combinationValue;
    public int dailyReward;
    public int SitNGoPlace { get; set; } = -1;
    static bool isFirstProfileUpdateReceived = false;

    public static void ResetData()
    {
        Instance = null;
    }

    public static void InitializeWith(PlayerProfileData data)
    {
        if (Instance == null)
        {
            Instance = data;
            Instance.LoadLocalData();
            OnReceiveMyPlayerProfile += Instance.UpdateProfile;
            OnReceiveMyPlayerTotalMoney += Instance.UpdateTotalMoney;
            OnReceiveCurrentMoneyBoxAmount += (chips) => Instance.moneyBox.Chips = chips;
        }
    }

    void LoadLocalData()
    {
        if (PlayerPrefs.HasKey("PlayerLoginId"))
            Id = Guid.Parse(PlayerPrefs.GetString("PlayerLoginId"));
        else
            Id = Guid.Empty;
        LastLoginProvider = (ExternalProviderName)PlayerPrefs.GetInt("LastLoginProvider", 0);
        picturePath = PlayerPrefs.GetString("PicturePath", string.Empty);
        countryPicturePath = PlayerPrefs.GetString("CountryPicturePath", string.Empty);
        firstLogin = PlayerPrefs.GetString("FirstLogin", DateTime.Now.ToString("dd.MM.yyyy"));
        combinationValue = 0;
        isVibrationOn = PlayerPrefs.GetInt("IsVibrationOn", 1) == 1 ? true : false;
        isHandsOn = PlayerPrefs.GetInt("IsHandsOn", 1) == 1 ? true : false;
        isHintsOn = PlayerPrefs.GetInt("IsHintsOn", 1) == 1 ? true : false;
        isSoundOn = PlayerPrefs.GetInt("IsSoundOn", 1) == 1 ? true : false;
        isAllSoundOn = PlayerPrefs.GetInt("IsAllSoundOn", 1) == 1 ? true : false;
        visitedTables = PlayerPrefs.GetString("VisitedTables", "00000000000000000000000000");
        language = PlayerPrefs.HasKey("Language")
            ? (Language)PlayerPrefs.GetInt("Language")
            : LanguageParser(Application.systemLanguage);
    }

    void UpdateProfile(PlayerProfileDto profileData)
    {
        if (profileData.Id == Guid.Empty)
            return;
        Id = profileData.Id;
        PlayerPrefs.SetString("PlayerLoginId", Id.ToString());
        Gender = profileData.Gender;
        Country = profileData.Country;
        HandsSpriteType = profileData.HandsSprite;
        Nickname = profileData.UserName;
        PlayerPrefs.SetString("Nickname", Nickname);
        RegistrationDate = profileData.RegistrationDate;
        PlayerPrefs.SetString("FirstLogin", RegistrationDate.ToString("dd.MM.yyyy"));
        Experience = profileData.Experience;
        TotalMoney = profileData.TotalMoney;
        GamesPlayed = profileData.GamesPlayed;
        BestHandType = profileData.BestHandType;
        GamesWon = profileData.GamesWon;
        BiggestWin = profileData.BiggestWin;
        SitAndGoWins = profileData.SitAndGoWins;
        ExternalLogin = profileData.ProviderName;
        moneyBox.Chips = profileData.MoneyBoxAmount;
        PersonalCode = profileData.PersonalCode;
        SaveCountryLocally();
        BestHandName = BestHandType.ToCombinationString();
        OnProfileUpdated?.Invoke();
        if (!isFirstProfileUpdateReceived)
        {
            isFirstProfileUpdateReceived = true;
            string avatarImagePath = Path.Combine(Application.persistentDataPath, "AvatarImage.png");
            if (File.Exists(avatarImagePath))
            {
                var texture = new Texture2D(0, 0);
                texture.LoadImage(File.ReadAllBytes(avatarImagePath));
                ProfileImages[0] = texture;
                UpdateProfileImage();
            }
            else
                HTTP_SendGetImageRequest(Id);
            ReceiveMoneyBoxAmount(Id);
        }
    }

    public void UpdateTotalMoney(long newValue)
    {
        TotalMoney = newValue;
        OnMyTotalMoneyUpdated?.Invoke();
    }

    public void UpdateExperience(int newValue)
    {
        Experience = newValue;
        OnExperienceUpdated?.Invoke();
    }

    public void SetLastLoginProvider(ExternalProviderName provider)
    {
        LastLoginProvider = provider;
        PlayerPrefs.SetInt("LastLoginProvider", (int)provider);
        PlayerPrefs.Save();
    }

    public PlayerProfileDto GetProfileDto()
    {
        return new PlayerProfileDto()
        {
            Id = Id,
            BestHandType = BestHandType,
            BiggestWin = BiggestWin,
            Country = Country,
            Experience = Experience,
            GamesPlayed = GamesPlayed,
            GamesWon = GamesWon,
            Gender = Gender,
            HandsSprite = HandsSpriteType,
            MoneyBoxAmount = moneyBox.Chips,
            RegistrationDate = RegistrationDate,
            SitAndGoWins = SitAndGoWins,
            TotalMoney = TotalMoney,
            ProviderName = ExternalLogin,
            UserName = Nickname,
            PersonalCode = PersonalCode
        };
    }

    public void SaveCountry(string countryName)
    {
        Country = countryName.ToCountryCode();
        var newCountryPucturePath = $"Images/Flags/{countryName}";
        countryPicturePath = newCountryPucturePath;
        PlayerPrefs.SetString("CountryPicturePath", newCountryPucturePath);
        HTTP_SendUpdatePlayerProfile();
    }

    void SaveCountryLocally()
    {
        var newCountryPucturePath = $"Images/Flags/{Country.ToString().ToLower()}";
        countryPicturePath = newCountryPucturePath;
        PlayerPrefs.SetString("CountryPicturePath", newCountryPucturePath);
    }

    public void UpdateProfile(string nickname, Gender gender, HandsSpriteType handsType)
    {
        Nickname = nickname;
        Gender = gender;
        HandsSpriteType = handsType;
        HTTP_SendUpdatePlayerProfile();
        PlayerPrefs.SetString("Nickname", nickname);
    }

    public void SaveLanguage(Language language)
    {
        this.language = language;
        PlayerPrefs.SetInt("Language", Convert.ToInt32(language));
    }

    public void SaveTable(int index)
    {
        StringBuilder someString = new StringBuilder(visitedTables);
        someString[index] = '1';
        visitedTables = someString.ToString();
        PlayerPrefs.SetString("VisitedTables", visitedTables);
    }

    public bool IsVisitedTable(int index)
    {
        if (visitedTables[index].Equals('0'))
            return false;
        return true;
    }

    public void SaveBestHand(int combinationValue, string bestHand)
    {
        this.combinationValue = combinationValue;
        BestHandName = bestHand;
        PlayerPrefs.SetInt("CombinationValue", this.combinationValue);
        PlayerPrefs.SetString("BestHand", BestHandName);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("IsVibrationOn", Convert.ToInt32(isVibrationOn));
        PlayerPrefs.SetInt("IsHandsOn", Convert.ToInt32(isHandsOn));
        PlayerPrefs.SetInt("IsHintsOn", Convert.ToInt32(isHintsOn));
        PlayerPrefs.SetInt("IsSoundOn", Convert.ToInt32(isSoundOn));
        PlayerPrefs.SetInt("IsAllSoundOn", Convert.ToInt32(isAllSoundOn));
    }

    private Language LanguageParser(SystemLanguage language)
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