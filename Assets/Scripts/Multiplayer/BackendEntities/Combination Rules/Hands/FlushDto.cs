﻿using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands
{
    public class FlushDto : IRulesDto
    {
        private const int Rate = 1300;
        public EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult();
            var numbersOfSuits = new Dictionary<int, int>();
            var allCards = tableCards.Concat(playerHand).ToList();
            var numberOfJokers = allCards.Count(c => c.Rank == CardRankType.Joker);
            if (numberOfJokers > 0)
            {
                // Write info about all cards to dictionary
                foreach (var card in allCards.Where(c => c.Rank != CardRankType.Joker))
                {
                    if (!numbersOfSuits.ContainsKey((int) card.Suit))
                        numbersOfSuits.Add((int) card.Suit, 1);
                    else
                        numbersOfSuits[(int) card.Suit]++;
                }
                if (numbersOfSuits.Any(c => c.Value >= (5 - numberOfJokers)) is false)
                    return result;
                result.IsWinningHand = true;
                result.Hand.HandType = HandType.Flush;
                result.Hand.Cards = new List<CardDto>();
                var maxSuit = (CardSuitType) numbersOfSuits
                    .Where(c => c.Value >= (5 - numberOfJokers))
                    .OrderByDescending(c => c.Key)
                    .First()
                    .Key;
                var cardsToAdd = allCards
                    .Where(c => c.Suit == maxSuit)
                    .OrderByDescending(c => c.Rank)
                    .ToList();
                for (var index = 0; index < 5 - numberOfJokers; index++)
                {
                    result.Hand.Cards.Add(cardsToAdd[index]);
                    result.Hand.Value += (int) cardsToAdd[index].Rank * Rate;
                }
                // Add jokers
                foreach (var card in allCards.Where(c => c.Rank == CardRankType.Joker))
                {
                    var maxValue = GetMaxValue(result.Hand.Cards);
                    card.SubstitutedCard = new CardDto {Rank = (CardRankType) maxValue, Suit = result.Hand.Cards[0].Suit};
                    card.Rank = (CardRankType) maxValue;
                    result.Hand.Cards.Add(card);
                    result.Hand.Value += maxValue * Rate;
                }
                result.Hand.Cards = result.Hand.Cards.OrderByDescending(c => c.Rank).ToList();
                foreach (var card in result.Hand.Cards.Where(card => card.SubstitutedCard != null))
                    card.Rank = CardRankType.Joker;
                return result;
            }
            foreach (var card in allCards)
            {
                if (!numbersOfSuits.ContainsKey((int) card.Suit))
                    numbersOfSuits.Add((int) card.Suit, 1);
                else
                    numbersOfSuits[(int) card.Suit]++;
            }
            if (numbersOfSuits.Any(c => c.Value >= 5) is false)
                return result;
            result.IsWinningHand = true;
            result.Hand.HandType = HandType.Flush;
            result.Hand.Cards = new List<CardDto>();
            var winningSuit = (CardSuitType) numbersOfSuits.First(c => c.Value >= 5).Key;
            var winningCards = allCards.Where(c => c.Suit == winningSuit).OrderByDescending(c => c.Rank).ToList();
            for (var index = 0; index < 5; index++)
            {
                result.Hand.Cards.Add(winningCards[index]);
                result.Hand.Value += (int) winningCards[index].Rank * Rate;
            }
            result.Hand.Cards = result.Hand.Cards.OrderByDescending(c => c.Rank).ToList();
            return result;
        }

        private int GetMaxValue(List<CardDto> cards)
        {
            for (var cardRank = CardRankType.Ace; cardRank >= CardRankType.Deuce; --cardRank)
                if (cards.Any(c => c.Rank == cardRank) is false)
                    return (int)cardRank;
            return 2;
        }
    }
}