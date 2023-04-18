using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands
{
    public class StraightFlushDto : IRulesDto
    {
        private const int Rate = 180000;
        public EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult();
            var allCards = tableCards.Concat(playerHand).ToList();
            var numberOfJokers = allCards.Count(c => c.Rank is CardRankType.Joker);
            switch (numberOfJokers)
            {
                case 0:
                    return CheckWithNoJokers(playerHand, tableCards, result);
                case 1:
                    return CheckWithOneJoker(playerHand, tableCards, result);
                case 2:
                    return result;
                default:
                    return result;
            }
        }

        private EvaluationResult CheckWithOneJoker(List<CardDto> playerHand, List<CardDto> tableCards, EvaluationResult result)
        {
            var isStraightResult = new StraightDto().Check(playerHand, tableCards);
            if (isStraightResult.IsWinningHand is false)
                return result;
            var (isStraightFlush, numberOfUsedJokers) =
                AnalyzeStraightCardsSuits(isStraightResult.Hand.Cards);
            if (isStraightFlush is false)
                return result;
            result.IsWinningHand = true;
            result.Hand.HandType = HandType.StraightFlush;
            result.Hand.Cards = isStraightResult.Hand.Cards.ToList();
            var joker = isStraightResult.Hand.Cards.First(c => c.Rank is CardRankType.Joker);
            joker.SubstitutedCard.Suit = result.Hand.Cards[0].Suit;
            EvaluateHand(result);
            result.Hand.Cards = SortByDescending(result.Hand.Cards);
            return result;
        }

        private List<CardDto> SortByDescending(List<CardDto> cards)
        {
            foreach (var card in cards.Where(c => c.Rank == CardRankType.Joker))
                card.Rank = card.SubstitutedCard.Rank;
            cards = cards.OrderByDescending(c => c.Rank).ToList();
            foreach (var card in cards.Where(c => c.SubstitutedCard != null))
                card.Rank = CardRankType.Joker;
            return cards;
        }

        private static void EvaluateHand(EvaluationResult result)
        {
            foreach (var card in result.Hand.Cards)
            {
                if (card.Rank == CardRankType.Joker)
                {
                   result.Hand.Value += (int) card.SubstitutedCard.Rank * Rate;
                   continue;
                }
                result.Hand.Value += (int) card.Rank * Rate;
            }    
        }

        private EvaluationResult CheckWithNoJokers(List<CardDto> playerHand, List<CardDto> tableCards, EvaluationResult result)
        {
            var isStraightResult = new StraightDto().Check(playerHand, tableCards);
            if (isStraightResult.IsWinningHand is false)
                return result;
            var (isStraightFlush, numberOfUsedJokers) =
                AnalyzeStraightCardsSuits(isStraightResult.Hand.Cards);
            if (isStraightFlush is false)
                return result;
            result.IsWinningHand = true;
            result.Hand.HandType = HandType.StraightFlush;
            result.Hand.Cards = isStraightResult.Hand.Cards.ToList();
            foreach (var card in result.Hand.Cards)
                result.Hand.Value += (int) card.Rank * Rate;
            return result;
        }

        private (bool, int) AnalyzeStraightCardsSuits(List<CardDto> cards)
        {
            var cardsWithoutJokers = cards.Where(c => c.Rank != CardRankType.Joker).ToList();
            if (cardsWithoutJokers.All(c => c.Suit == cardsWithoutJokers[0].Suit))
                return (true, cards.Count - cardsWithoutJokers.Count);
            return (false, cards.Count - cardsWithoutJokers.Count);
        }
    }
}