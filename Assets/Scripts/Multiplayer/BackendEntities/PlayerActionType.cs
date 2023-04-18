namespace PokerHand.Common.Helpers
{
    [System.Serializable]
    public enum PlayerActionType
    {
        None = 0,
        Fold = 1,
        Check = 2,
        Bet = 3,
        Call = 4,
        Raise = 5,
        AllIn = 6
    }
}