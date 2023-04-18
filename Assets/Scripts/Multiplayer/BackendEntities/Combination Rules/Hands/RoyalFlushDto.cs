﻿using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands
{
    public class RoyalFlushDto : IRulesDto
    {
        private const int Rate = 200000;
        
        public EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult();
            var straightFlushCheckResult = new StraightFlushDto().Check(playerHand, tableCards);
            switch (straightFlushCheckResult.IsWinningHand)
            {
                case true when straightFlushCheckResult.Hand.Cards[0].Rank == CardRankType.Ace:
                {
                    result.IsWinningHand = true;
                    result.Hand.HandType = HandType.RoyalFlush;
                    result.Hand.Cards = straightFlushCheckResult.Hand.Cards.ToList();
                    foreach (var card in result.Hand.Cards)
                        result.Hand.Value += (int) card.Rank * Rate;
                    return result;
                }
                case false:
                    return EvaluationResult(playerHand, tableCards);
            }
            return result;
        }

        private static EvaluationResult EvaluationResult(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult();
            var allCards = playerHand.Concat(tableCards).ToList();
            var numberOfJokers = allCards.Count(c => c.Rank == CardRankType.Joker);
            switch (numberOfJokers)
            {
                case 1:
                    break;
                case 2:
                    break;
            }
            return result;
        }
    }
}