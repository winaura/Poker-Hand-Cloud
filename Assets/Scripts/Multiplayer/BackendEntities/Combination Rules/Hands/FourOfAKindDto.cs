using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands
{
    public class FourOfAKindDto : IRulesDto
    {
        private const int Rate = 60000;
        public EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult();
            var allCards = tableCards.Concat(playerHand).ToList();
            var dict = new Dictionary<CardRankType, int>();
            var numberOfJokers = allCards.Count(c => c.Rank is CardRankType.Joker);
            foreach (var card in allCards)
            {
                if (!dict.ContainsKey(card.Rank))
                    dict.Add(card.Rank, 1);
                else
                    dict[card.Rank]++;
            }
            if (numberOfJokers > 0)
            {
                switch (numberOfJokers)
                {
                    case 1:
                        // ThreeOfAKind + Joker + SideCard
                        var threeOfAKindCheck = CheckForThreeOfAKind(allCards);

                        if (threeOfAKindCheck.isThreeOfAKind)
                        {
                            result.IsWinningHand = true;
                            result.Hand.HandType = HandType.FourOfAKind;
                            result.Hand.Cards = new List<CardDto>();
                            result.Hand.Cards.AddRange(threeOfAKindCheck.threeCards);
                            result.Hand.Value += (int) threeOfAKindCheck.threeCards[0].Rank * 4 * Rate;
                            var joker = allCards.First(c => c.Rank == CardRankType.Joker);
                            joker.SubstitutedCard = new CardDto {Rank = threeOfAKindCheck.threeCards[0].Rank};
                            result.Hand.Cards.Add(joker);
                            var side = allCards.Except(result.Hand.Cards).OrderByDescending(c => c.Rank).First();
                            result.Hand.Cards.Add(side);
                            result.Hand.Value += (int) side.Rank * Rate;
                            return result;
                        }
                        return result;
                    case 2:
                        // OnePair + Joker + Joker + SideCard
                        var onePairCheck = CheckForOnePair(allCards);
                        if (onePairCheck.isOnePair)
                        {
                            result.IsWinningHand = true;
                            result.Hand.HandType = HandType.FourOfAKind;
                            result.Hand.Cards = new List<CardDto>();
                            result.Hand.Cards.AddRange(onePairCheck.onePair);
                            result.Hand.Value += (int) onePairCheck.onePair[0].Rank * 4 * Rate;
                            var jokers = allCards.Where(c => c.Rank == CardRankType.Joker).ToList();
                            foreach (var card in jokers)
                                card.SubstitutedCard = new CardDto {Rank = result.Hand.Cards[0].Rank};
                            result.Hand.Cards.AddRange(jokers);
                            var side = allCards.Except(result.Hand.Cards).OrderByDescending(c => c.Rank).First();
                            result.Hand.Cards.Add(side);
                            result.Hand.Value += (int) side.Rank * Rate;
                            return result;
                        }
                        return result;
                }
            }
            if (dict.Any(c => c.Value == 4) == false)
                return result;
            result.IsWinningHand = true;
            result.Hand.HandType = HandType.FourOfAKind;
            result.Hand.Cards = new List<CardDto>();
            var winningRank = dict.First(c => c.Value is 4).Key;
            var winningCards = allCards.Where(c => c.Rank == winningRank).OrderByDescending(c => c.Rank).Take(4).ToList();
            var sideCard = allCards.Where(c => c.Rank != winningRank).OrderByDescending(c => c.Rank).First();
            result.Hand.Cards.AddRange(winningCards);
            result.Hand.Cards.Add(sideCard);
            result.Hand.Value = ((int)winningCards[0].Rank * 4 + (int)sideCard.Rank) * Rate;
            return result;
        }
        
        private (bool isThreeOfAKind, List<CardDto> newCards, List<CardDto> threeCards) CheckForThreeOfAKind(List<CardDto> allCards)
        {
            // Key is Rank, Value is number of cards
            var dict = new Dictionary<int, int>();
            foreach (var card in allCards)
            {
                if (!dict.ContainsKey((int) card.Rank))
                    dict.Add((int) card.Rank, 1);
                else
                    dict[(int) card.Rank]++;
            }
            if (dict.Any(pair => pair.Value >= 3))
            {
                var maxRank = -1;
                foreach (var pair in dict.Where(p => p.Value >= 3))
                    if (pair.Key >= maxRank)
                        maxRank = pair.Key;
                var threeCards = allCards.Where(c => (int) c.Rank == maxRank).Take(3).ToList();
                var newCards = allCards.ToList();
                foreach (var card in threeCards)
                    newCards.Remove(card);
                return (true, newCards, threeCards);
            }
            return (false, null, null);
        }
        
        private (bool isOnePair, List<CardDto> newCards, List<CardDto> onePair) CheckForOnePair(List<CardDto> allCards)
        {
            // Key is Rank, Value is number of cards
            var dict = new Dictionary<int, int>();
            allCards = allCards.Where(c => c.Rank != CardRankType.Joker).ToList();
            foreach (var card in allCards)
            {
                if (!dict.ContainsKey((int) card.Rank))
                    dict.Add((int) card.Rank, 1);
                else
                    dict[(int) card.Rank]++;
            }
            if (dict.Any(pair => pair.Value >= 2))
            {
                var maxRank = -1;
                foreach (var pair in dict.Where(p => p.Value >= 2))
                    if (pair.Key >= maxRank)
                        maxRank = pair.Key;
                var onePair = allCards.Where(c => (int) c.Rank == maxRank).Take(2).ToList();
                foreach (var card in onePair)
                    allCards.Remove(card);
                return (true, allCards, onePair);
            }
            return (false, null, null);
        }
    }
}