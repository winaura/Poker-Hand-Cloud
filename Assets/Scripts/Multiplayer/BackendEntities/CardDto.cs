using PokerHand.Common.Helpers;

namespace PokerHand.Common.Entities
{
    [System.Serializable]
    public class CardDto
    {
        public CardRankType Rank { get; set; }
        public CardSuitType Suit { get; set; }
        public CardDto SubstitutedCard { get; set; }
    }
}