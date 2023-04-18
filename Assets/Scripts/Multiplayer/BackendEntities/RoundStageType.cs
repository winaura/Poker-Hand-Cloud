namespace PokerHand.Common.Helpers
{
    [System.Serializable]
    public enum RoundStageType
    {
        Created = -1,
        NotStarted = 0,
        PrepareTable = 1,
        MakeBlindBets = 2,
        WageringPreFlopRound = 3,
        DealCommunityCards = 4,
        WageringSecondRound = 5,
        WageringThirdRound = 6,
        WageringFourthRound = 7,
        Showdown = 8,
        Refresh = 9,
        Ended = 10
    }
}