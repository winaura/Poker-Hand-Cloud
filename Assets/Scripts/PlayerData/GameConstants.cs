using PokerHand.Common.Helpers.Player;
using UnityEngine;

public static class GameConstants
{
    // Profile parameters
    public const Gender DefaultGender = Gender.Male;
    public const HandsSpriteType DefaultHandsSpriteType = HandsSpriteType.WhiteMan;
    // Game parameters
    public const int MainMenuNotificationDuration = 10;
    public const int PlayerTurnTime = 13;
    public const float ChipsLifetime = 0.2f;
    public const float GiftsFlightTime = 0.5f;
    public const float QuickMessageLifetime = 2.5f;
    public const float OpenCardsChoiseLifetime = 3f;
    public const int MaxAvatarSize = 256;
    private const int WindowWithMoneyTime = 60;
    // Scenes
    public const string SceneMainMenu = "MainMenu";
    public const string SceneGame = "Game";
    // Values
    public static readonly Vector2 FullCardSize = new Vector2(0.385f, 0.385f);
    public static readonly Vector2 DefaultCardSize = new Vector2(0.2f, 0.2f);
    public static readonly Vector2 CardLightBorderSize = new Vector2(0.47f, 0.44f);
    public static readonly Vector2 DefaultOutOfViewPosition = Vector3.right * 100;
    public static readonly Vector2 DealerChipsPosition = new Vector2(0, 2f);
    public static readonly Vector2 WorldsLerpVector = new Vector2(0.8f, 0.8f);
    public static readonly Vector3 DealerPosition = new Vector3(0, 2f, 0);
    public static readonly Quaternion ZeroQuaternion = new Quaternion(0, 0, 0, 0);
    public static readonly Color WhiteNotTransparentColor = new Color(1, 1, 1, 1f);
    public static readonly Color WhiteHalfTransparentColor = new Color(1, 1, 1, 0.5f);
    public static readonly Color WhiteTransparentColor = new Color(1, 1, 1, 0f);
    // Timings
    public static readonly WaitForSeconds WaitSeconds_TurnTime = new WaitForSeconds(PlayerTurnTime);
    public static readonly WaitForSeconds WaitSeconds_WindowWithMoneyTime = new WaitForSeconds(WindowWithMoneyTime);
    public static readonly WaitForSeconds WaitSeconds_3 = new WaitForSeconds(3f);
    public static readonly WaitForSeconds WaitSeconds_2 = new WaitForSeconds(2f);
    public static readonly WaitForSeconds WaitSeconds_1 = new WaitForSeconds(1f);
    public static readonly WaitForSeconds WaitSeconds_05 = new WaitForSeconds(0.5f);
    public static readonly WaitForSeconds WaitSeconds_03 = new WaitForSeconds(0.3f);
    public static readonly WaitForSeconds WaitSeconds_02 = new WaitForSeconds(0.2f);
    public static readonly WaitForSeconds WaitSeconds_0_02 = new WaitForSeconds(0.02f);
    // Combinations
    public const string FiveOfAKind = "Combinations.FiveOfAKind";
    public const string RoyalFlush = "Combinations.RoyalFlush";
    public const string StraightFlush = "Combinations.StraightFlush";
    public const string FourOfAKind = "Combinations.FourOfAKind";
    public const string FullHouse = "Combinations.FullHouse";
    public const string Flush = "Combinations.Flush";
    public const string Straight = "Combinations.Straight";
    public const string ThreeOfAKind = "Combinations.ThreeOfAKind";
    public const string TwoPairs = "Combinations.TwoPair";
    public const string Pair = "Combinations.Pair";
    public const string HighValue = "Combinations.HighValue";
    // Localization keys
    public const string WaitingNewGameMessage = "OnTable.WaitingMessage";
    public const string WaitingForPlayersMessage = "OnTable.WaitingForPlayersMessage";
    public const string KickedByAFKNotification = "Notification.KickedAFK";
    public const string AutoTopOn = "WindowWithMoney.AutoTopOn";
    public const string AutoTopOff = "WindowWithMoney.AutoTopOff";
    // Animation flags
    public const string AnimThinking = "Thinking";
    public const string AnimFold = "Fold";
    public const string AnimCheck = "Check";
    public const string AnimRaise = "Raise";
    public const string AnimGrab = "Grab";
    public const string AnimGrabCards = "GrabCards";
    public const string AnimIdle = "Idle";
    public const string AnimStartDeal = "StartDeal";
    public const string AnimDealAction = "DealAction";
    public const string AnimEndDeal = "EndDeal";
    // Date
    public const string dateTimeFormat = "dd.MM.yyyy HH:mm:ss";
}