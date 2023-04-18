using PokerHand.Common.Helpers;

[System.Serializable]
public class PlayerAction
{
    public int PlayerIndexNumber { get; set; }
    public PlayerActionType ActionType { get; set; }
    public long? Amount { get; set; }
}