﻿using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands
{
    public class StraightDto : IRulesDto
    {
        private const int Rate = 400;

        public EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult();
            var allCards = tableCards.Concat(playerHand).ToList();
            var numberOfJokers = allCards.Count(c => c.Rank == CardRankType.Joker);
            var jokers = allCards.Where(c => c.Rank == CardRankType.Joker).ToList();
            allCards = allCards
                .Where(c => c.Rank != CardRankType.Joker)
                .GroupBy(c => c.Rank)
                .Select(g => g.First())
                .OrderByDescending(c => c.Rank)
                .ToList();
            allCards.AddRange(jokers);
            if (allCards.Count < 5)
                return result;
            switch (numberOfJokers)
            {
                case 0:
                    for (var index = 0; index < allCards.Count - 4; index++)
                    {
                        var isStraight =    (int) allCards[index].Rank == (int) allCards[index + 1].Rank + 1
                                         && (int) allCards[index].Rank == (int) allCards[index + 2].Rank + 2
                                         && (int) allCards[index].Rank == (int) allCards[index + 3].Rank + 3
                                         && (int) allCards[index].Rank == (int) allCards[index + 4].Rank + 4;
                        if (isStraight is false)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = index; i <= index + 4; i++)
                        {
                            cardsToAdd.Add(allCards[i]);
                            result.Hand.Value += (int) allCards[i].Rank * Rate;
                        }
                        result.Hand.Cards = new List<CardDto>();
                        result.Hand.Cards = cardsToAdd.ToList();

                        return result;
                    }
                    // Check on low ace
                    if (allCards.Any(c => c.Rank is CardRankType.Ace))
                    {
                        var isStraight = allCards.Any(c => c.Rank is CardRankType.Deuce) &&
                                         allCards.Any(c => c.Rank is CardRankType.Three) &&
                                         allCards.Any(c => c.Rank is CardRankType.Four) &&
                                         allCards.Any(c => c.Rank is CardRankType.Five);
                        if (isStraight is false)
                            return result;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = 0; i < 4; i++)
                        {
                            cardsToAdd.Add(allCards.First(c => (int) c.Rank == i + 2));
                            result.Hand.Value += (int) cardsToAdd.Last().Rank * Rate;
                        }
                        cardsToAdd.Add(allCards.First(c => c.Rank == CardRankType.Ace));
                        result.Hand.Value += (int) cardsToAdd.Last().Rank * Rate;
                        result.Hand.Cards = new List<CardDto>();
                        result.Hand.Cards = cardsToAdd.ToList();
                        return result;
                    }
                    return result;
                case 1:
                    // Check for Five straight cards
                    for (var index = 0; index < allCards.Count - 4; index++)
                    {
                        if (allCards[index].Rank == CardRankType.Joker)
                            continue;
                        var isFiveStraight = (int) allCards[index].Rank == (int) allCards[index + 1].Rank + 1
                                             && (int) allCards[index].Rank == (int) allCards[index + 2].Rank + 2
                                             && (int) allCards[index].Rank == (int) allCards[index + 3].Rank + 3
                                             && (int) allCards[index].Rank == (int) allCards[index + 4].Rank + 4;
                        if (isFiveStraight is false)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = index; i <= index + 4; i++)
                            cardsToAdd.Add(allCards[i]);
                        // Add Joker
                        var maxCardRank = cardsToAdd[0].Rank;
                        if (maxCardRank != CardRankType.Ace)
                        {
                            cardsToAdd.Remove(cardsToAdd.Last());
                            cardsToAdd.Add(allCards.First(c => c.Rank is CardRankType.Joker));
                            cardsToAdd.Last().SubstitutedCard = new CardDto {Rank = CardRankType.Ace};
                            cardsToAdd = cardsToAdd.OrderByDescending(c => c.Rank).ToList();
                            result.Hand.Value += ((int) allCards[index].Rank * 5 - 5) * Rate;
                        }
                        else
                            result.Hand.Value += ((int) CardRankType.Ten + (int) CardRankType.Jack +
                                                           (int) CardRankType.Queen + (int) CardRankType.King +
                                                           (int) CardRankType.Ace) * Rate;
                        result.Hand.Cards = new List<CardDto>();
                        result.Hand.Cards = cardsToAdd.ToList();
                        return result;
                    }
                    // Check for Four straight cards
                    for (var index = 0; index < allCards.Count - 3; index++)
                    {
                        if (allCards[index].Rank is CardRankType.Joker)
                            continue;
                        var isFourStraight = (int) allCards[index].Rank == (int) allCards[index + 1].Rank + 1
                                             && (int) allCards[index].Rank == (int) allCards[index + 2].Rank + 2
                                             && (int) allCards[index].Rank == (int) allCards[index + 3].Rank + 3;
                        if (isFourStraight is false)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = index; i <= index + 3; i++)
                        {
                            cardsToAdd.Add(allCards[i]);
                            result.Hand.Value += (int) allCards[i].Rank * Rate;
                        }
                        // Add Joker
                        var maxCardRank = cardsToAdd
                            .Where(c => c.Rank != CardRankType.Joker)
                            .OrderByDescending(c => c.Rank)
                            .First()
                            .Rank;
                        var joker = allCards.First(c => c.Rank is CardRankType.Joker);
                        if (maxCardRank is CardRankType.Ace)
                        {
                            result.Hand.Value += (int) CardRankType.Ten * Rate;
                            joker.SubstitutedCard = new CardDto {Rank = CardRankType.Ten};
                        }
                        else
                        {
                            result.Hand.Value += ((int) maxCardRank + 1) * Rate;
                            joker.SubstitutedCard = new CardDto {Rank = maxCardRank + 1};
                        }
                        cardsToAdd = cardsToAdd
                            .OrderBy(c => c.Rank)
                            .ToList();
                        cardsToAdd.Add(joker);
                        result.Hand.Cards = new List<CardDto>();
                        result.Hand.Cards = cardsToAdd.ToList();
                        return result;
                    }
                    // Check for Three straight and One card AND for 2 x Two straight
                    for (var index = 0; index < allCards.Count - 3; index++)
                    {
                        if (allCards[index].Rank == CardRankType.Joker)
                            continue;
                        if ((int) allCards[index].Rank != (int) allCards[index + 3].Rank + 4)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        result.Hand.Cards = new List<CardDto>();
                        for (var i = 0; i < 4; i++)
                        {
                            result.Hand.Cards.Add(allCards[index + i]);
                            if (i > 0 &&
                                result.Hand.Cards[i].Rank == result.Hand.Cards[i - 1].Rank - 2)
                            {
                                var joker = allCards.First(c => c.Rank is CardRankType.Joker);
                                joker.SubstitutedCard = new CardDto {Rank = result.Hand.Cards[i].Rank + 1};
                            }
                        }
                        result.Hand.Cards.Add(allCards.First(c => c.Rank is CardRankType.Joker));
                        var jokerToSubstitute = result.Hand.Cards.First(c => c.Rank is CardRankType.Joker);
                        jokerToSubstitute.Rank = jokerToSubstitute.SubstitutedCard.Rank;
                        result.Hand.Cards = result.Hand.Cards.OrderByDescending(c => c.Rank).ToList();
                        jokerToSubstitute.Rank = CardRankType.Joker;
                        result.Hand.Value = ((int) allCards[index].Rank * 5 - 10) * Rate;
                        return result;
                    }
                    break;
                case 2:
                    // Check for Five straight cards
                    for (var index = 0; index < allCards.Count - 4; index++)
                    {
                        if (allCards[index].Rank == CardRankType.Joker)
                            continue;
                        var isFiveStraight =    (int) allCards[index].Rank == (int) allCards[index + 1].Rank + 1
                                             && (int) allCards[index].Rank == (int) allCards[index + 2].Rank + 2
                                             && (int) allCards[index].Rank == (int) allCards[index + 3].Rank + 3
                                             && (int) allCards[index].Rank == (int) allCards[index + 4].Rank + 4;
                        if (isFiveStraight == false)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = index; i <= index + 4; i++)
                            cardsToAdd.Add(allCards[i]);
                        // Add Jokers
                        var maxCardRank = cardsToAdd
                            .Where(c => c.Rank != CardRankType.Joker)
                            .OrderByDescending(c => c.Rank)
                            .First()
                            .Rank;
                        switch (maxCardRank)
                        {
                            case CardRankType.King:
                                cardsToAdd.Remove(cardsToAdd.Last());

                                foreach (var card in cardsToAdd)
                                    result.Hand.Value += (int) card.Rank * Rate;
                                result.Hand.Value += (int) CardRankType.Ace * Rate;
                                cardsToAdd.Add(allCards.First(c => c.Rank is CardRankType.Joker));
                                cardsToAdd.First(c => c.Rank is CardRankType.Joker).SubstitutedCard =
                                    new CardDto { Rank = CardRankType.Ace };
                                break;
                            case CardRankType.Ace:
                                foreach (var card in cardsToAdd)
                                    result.Hand.Value += (int) card.Rank * Rate;
                                break;
                            default:
                                cardsToAdd.Remove(cardsToAdd.Last());
                                cardsToAdd.Remove(cardsToAdd.Last());
                                foreach (var card in cardsToAdd)
                                    result.Hand.Value += (int) card.Rank * Rate;
                                result.Hand.Value += ((int) maxCardRank * 2 + 3) * Rate;
                                var counter = 2;
                                foreach (var card in allCards.Where(c => c.Rank == CardRankType.Joker))
                                {
                                    card.SubstitutedCard = new CardDto {Rank = cardsToAdd[0].Rank + counter};
                                    counter--;
                                }
                                cardsToAdd.AddRange(allCards.Where(c => c.Rank == CardRankType.Joker));
                                break;
                        }
                        cardsToAdd = cardsToAdd.OrderByDescending(c => c.Rank).ToList();
                        result.Hand.Cards = new List<CardDto>();
                        result.Hand.Cards = cardsToAdd.ToList();
                        return result;
                    }
                    // Check fot Four straight cards
                    for (var index = 0; index < allCards.Count - 3; index++)
                    {
                        if (allCards[index].Rank == CardRankType.Joker)
                            continue;
                        var isFourStraight =    (int) allCards[index].Rank == (int) allCards[index + 1].Rank + 1
                                             && (int) allCards[index].Rank == (int) allCards[index + 2].Rank + 2
                                             && (int) allCards[index].Rank == (int) allCards[index + 3].Rank + 3;
                        if (isFourStraight is false)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = index; i <= index + 3; i++)
                            cardsToAdd.Add(allCards[i]);
                        // Add Jokers
                        var maxCardRank = cardsToAdd
                            .Where(c => c.Rank != CardRankType.Joker)
                            .OrderByDescending(c => c.Rank)
                            .First()
                            .Rank;
                        switch (maxCardRank)
                        {
                            case CardRankType.King:
                                foreach (var card in cardsToAdd)
                                    result.Hand.Value += (int) card.Rank * Rate;
                                result.Hand.Value += (int) CardRankType.Ace * Rate;
                                cardsToAdd.Add(allCards.First(c => c.Rank == CardRankType.Joker));
                                cardsToAdd.First(c => c.Rank is CardRankType.Joker).SubstitutedCard =
                                    new CardDto {Rank = CardRankType.Ace};
                                break;
                            case CardRankType.Ace:
                                foreach (var card in cardsToAdd)
                                    result.Hand.Value += (int) card.Rank * Rate;
                                result.Hand.Value += (int) CardRankType.Ten * Rate;
                                cardsToAdd.Add(allCards.First(c => c.Rank is CardRankType.Joker));
                                cardsToAdd.Last().SubstitutedCard = new CardDto {Rank = cardsToAdd[cardsToAdd.Count - 2].Rank - 1};
                                result.Hand.Cards = new List<CardDto>();
                                result.Hand.Cards = cardsToAdd.ToList();
                                return result;
                            default:
                                cardsToAdd.Remove(cardsToAdd.Last());
                                foreach (var card in cardsToAdd)
                                    result.Hand.Value += (int) card.Rank * Rate;
                                result.Hand.Value += ((int) maxCardRank * 2 + 3) * Rate;
                                cardsToAdd.AddRange(allCards.Where(c => c.Rank == CardRankType.Joker));
                                var counter = 1;
                                foreach (var card in cardsToAdd.Where(c => c.Rank is CardRankType.Joker))
                                {
                                    card.SubstitutedCard = new CardDto {Rank = cardsToAdd[0].Rank + counter};
                                    card.Rank = cardsToAdd[0].Rank + counter;
                                    counter++;
                                }
                                cardsToAdd = cardsToAdd.OrderByDescending(c => c.Rank).ToList();
                                foreach (var card in cardsToAdd.Where(c => c.SubstitutedCard != null))
                                    card.Rank = CardRankType.Joker;
                                result.Hand.Cards = new List<CardDto>();
                                result.Hand.Cards = cardsToAdd.ToList();
                                return result;
                        }
                        cardsToAdd = cardsToAdd.OrderByDescending(c => c.Rank).ToList();
                        result.Hand.Cards = new List<CardDto>();
                        result.Hand.Cards = cardsToAdd.ToList();
                        return result;
                    }
                    // Check for Three straight cards
                    for (var index = 0; index < allCards.Count - 2; index++)
                    {
                        if (allCards[index].Rank is CardRankType.Joker)
                            continue;
                        var isThreeStraight = (int) allCards[index].Rank == (int) allCards[index + 1].Rank + 1
                                              && (int) allCards[index].Rank == (int) allCards[index + 2].Rank + 2;
                        if (isThreeStraight == false)
                            continue;
                        result.IsWinningHand = true;
                        result.Hand.HandType = HandType.Straight;
                        var cardsToAdd = new List<CardDto>();
                        for (var i = index; i <= index + 2; i++)
                        {
                            cardsToAdd.Add(allCards[i]);
                            result.Hand.Value += (int) allCards[i].Rank * Rate;
                        }
                        // Add Jokers
                        var maxCardRank = cardsToAdd
                            .Where(c => c.Rank == CardRankType.Joker)
                            .OrderByDescending(c => c.Rank)
                            .First()
                            .Rank;
                        switch (maxCardRank)
                        {
                            case CardRankType.King:
                                cardsToAdd.AddRange(allCards.Where(c => c.Rank is CardRankType.Joker));
                                result.Hand.Value += ((int) CardRankType.Ace + (int) CardRankType.Ten) * Rate;
                                cardsToAdd[3].Rank = CardRankType.Ace;
                                cardsToAdd[3].SubstitutedCard = new CardDto {Rank = CardRankType.Ace};
                                cardsToAdd[4].Rank = CardRankType.Ten;
                                cardsToAdd[4].SubstitutedCard = new CardDto {Rank = CardRankType.Ten};
                                cardsToAdd = cardsToAdd.OrderByDescending(c => c.Rank).ToList();
                                foreach (var card in cardsToAdd.Where(c => c.SubstitutedCard == null))
                                    card.Rank = CardRankType.Joker;
                                result.Hand.Cards = new List<CardDto>();
                                result.Hand.Cards = cardsToAdd.ToList();
                                return result;
                            case CardRankType.Ace:
                                cardsToAdd.AddRange(allCards.Where(c => c.Rank is CardRankType.Joker));
                                result.Hand.Value += ((int) CardRankType.Jack + (int) CardRankType.Ten) * Rate;
                                cardsToAdd[3].SubstitutedCard = new CardDto {Rank = CardRankType.Jack};
                                cardsToAdd[4].SubstitutedCard = new CardDto {Rank = CardRankType.Ten};
                                result.Hand.Cards = new List<CardDto>();
                                result.Hand.Cards = cardsToAdd.ToList();
                                return result;
                            default:
                                cardsToAdd.AddRange(allCards.Where(c => c.Rank is CardRankType.Joker));
                                result.Hand.Value += ((int) cardsToAdd.First().Rank * 2 + 3) * Rate;
                                var counter = 1;
                                foreach (var card in cardsToAdd.Where(c => c.Rank is CardRankType.Joker))
                                {
                                    card.SubstitutedCard = new CardDto {Rank = cardsToAdd[0].Rank + counter};
                                    card.Rank = cardsToAdd[0].Rank + counter;
                                    counter++;
                                }
                                cardsToAdd = cardsToAdd.OrderByDescending(c => c.Rank).ToList();
                                foreach (var card in cardsToAdd.Where(c => c.SubstitutedCard != null))
                                    card.Rank = CardRankType.Joker;
                                result.Hand.Cards = new List<CardDto>();
                                result.Hand.Cards = cardsToAdd.ToList();
                                return result;
                        }
                    }
                    break;
            }
            return result;
        }
    }
}