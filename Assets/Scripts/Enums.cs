public enum Language
{
    English,
    Russian,
    French,
    Spanish,
    German
}

public enum RarityMenuButtons
{
    White,
    Blue,
    Orange,
    Pink,
    Lucky
}

public enum Worlds
{
    None = -1,
    ChillOut,
    Insomnia,
    Underground,
    Business
}

public enum GameModes
{
    Texas = 0,
    lowball,
    Royal,
    Joker,
    Dash,
    SitNGo
}

public enum ItemForPurchaseNumber
{
    SpinDollars099 = 1,
    SpinDollars1099 = 2,
    SpinDollars2599 = 3,
    SpinDollars7999 = 4,
    ChipsDollars099 = 5,
    ChipsDollars399 = 6,
    ChipsDollars599 = 7,
    ChipsDollars999 = 8,
    ChipsDollars1799 = 9,
    ChipsDollars2599 = 10,
    ChipsDollars4999 = 11,
    ChipsDollars9999 = 12,
    MoneyBoxDollars1599 = 13
}

[System.Serializable]
public enum SpinType
{
    WhitePower,
    BluePower,
    OrangePower,
    PurplePower
}

public enum Clips
{
    MenuMusic = 0,
    CardDistribution,
    OpenCard,
    TableCardDistribution,
    FoldCard,
    CheckCard,
    BlindBet,
    Raise,
    ChipsIntoBank,
    ChipsIntoPlayer,
    ChilloutMusic,
    CityMusic,
    PianoMusic,
    Spin,
    Winning,
    MoneyBoxChips
}

public enum TablesInWorlds
{
    Dash,
    Lowball,
    Private,
    Tournament,
    TropicalHouse,
    WetDeskLounge,
    IbizaDisco,
    ShishaBar,
    BeachClub,
    RivieraHotel,
    SevenNightsClub,
    MirageCasino,
    CityDreamsResort,
    SunriseCafe,
    BlueMoonYacht,
    GoldMine,
    DesertCaveHotel,
    ImperialBunker,
    MillenniumHotel,
    TradesmanClub,
    FortuneHippodrome,
    HeritageBank,
    Dash300K,
    Dash1M,
    Dash10M,
    Dash100M
}

public enum Status
{
    Free,
    Prepare,
    Active,
    End
}

public enum ConnectingType
{
    ConnectingToTable,
    DisconnectingFromTable,
    Reconnecting
}