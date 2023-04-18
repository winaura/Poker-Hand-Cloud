using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands;
using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Dto;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Services
{
    public static class CardEvaluationService
    {
        static List<IRulesDto> rules = new List<IRulesDto>
        {
            new FiveOfAKindDto(),
            new RoyalFlushDto(),
            new StraightFlushDto(),
            new FourOfAKindDto(),
            new FullHouseDto(),
            new FlushDto(),
            new StraightDto(),
            new ThreeOfAKindDto(),
            new TwoPairsDto(),
            new OnePairDto(),
            new HighCardDto()
        };

        public static void FindCombination(List<Card> playerHand, List<Card> tableCards, out string combinationName, out List<Card> combinationCards)
        {
            var result = new Hand();
            List<CardDto> playerHandDto = playerHand.Select(c => c.ToCardDto()).ToList();
            List<CardDto> tableCardsDto = tableCards.Select(c => c.ToCardDto()).ToList();
            foreach (var rule in rules)
            {
                var evaluationResult = rule.Check(playerHandDto, tableCardsDto);
                if (evaluationResult.IsWinningHand == false)
                    continue;
                result.HandType = evaluationResult.Hand.HandType;
                result.Cards = evaluationResult.Hand.Cards;
                break;
            }
            combinationName = result.HandType.ToCombinationString();
            combinationCards = ExtractWinCards(playerHand, tableCards, result.Cards, result.HandType);
        }

        public static void FindCombination(List<Card> playerHand, List<Card> tableCards, out string combinationName)
        {
            var result = new Hand();
            List<CardDto> playerHandDto = playerHand.Select(c => c.ToCardDto()).ToList();
            List<CardDto> tableCardsDto = tableCards.Select(c => c.ToCardDto()).ToList();
            foreach (var rule in rules)
            {
                var evaluationResult = rule.Check(playerHandDto, tableCardsDto);
                if (evaluationResult.IsWinningHand == false)
                    continue;
                result.HandType = evaluationResult.Hand.HandType;
                break;
            }
            combinationName = result.HandType.ToCombinationString();
        }

        static List<Card> ExtractWinCards(List<Card> playerHand, List<Card> tableCards, List<CardDto> combinationCards, HandType handType)
        {
            var extractedCards = new List<Card>();
            var activeCardsAmount = GetActiveWinCardsAmount(handType);
            for (var i = 0; i < activeCardsAmount; i++)
            {
                bool isFounded = false;
                for (var h = 0; h < playerHand.Count; h++)
                {
                    if ((int)combinationCards[i].Rank == (int)playerHand[h].value &&
                        (int)combinationCards[i].Suit == (int)playerHand[h].suite)
                    {
                        extractedCards.Add(playerHand[h]);
                        isFounded = true;
                        break;
                    }
                }
                if (isFounded)
                    continue;
                for (var t = 0; t < tableCards.Count; t++)
                {
                    if ((int)combinationCards[i].Rank == (int)tableCards[t].value &&
                        (int)combinationCards[i].Suit == (int)tableCards[t].suite)
                    {
                        extractedCards.Add(tableCards[t]);
                        break;
                    }
                }
            }
            return extractedCards;
        }

        static int GetActiveWinCardsAmount(HandType handType)
        {
            switch (handType)
            {
                case HandType.None: return 0;
                case HandType.HighCard: return 1;
                case HandType.OnePair: return 2;
                case HandType.TwoPairs: return 4;
                case HandType.ThreeOfAKind: return 3;
                case HandType.Straight: return 5;
                case HandType.Flush: return 5;
                case HandType.FullHouse: return 5;
                case HandType.FourOfAKind: return 4;
                case HandType.StraightFlush: return 5;
                case HandType.RoyalFlush: return 5;
                case HandType.FiveOfAKind: return 5;
                default: return 0;
            }
        }
    }
}