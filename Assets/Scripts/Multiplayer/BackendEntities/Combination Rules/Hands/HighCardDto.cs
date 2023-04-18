using PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.CardEvaluation;
using System.Collections.Generic;
using System.Linq;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Hands
{
    public class HighCardDto : IRulesDto
    {
        public EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards)
        {
            var result = new EvaluationResult
            {
                IsWinningHand = true,
                Hand = new Hand
                {
                    Cards = new List<CardDto>(5),
                    HandType = HandType.HighCard,
                    Value = 0
                }
            };
            var allCards = playerHand.Concat(tableCards).OrderByDescending(c => (int)c.Rank).ToList();
            for (var index = 0; index < allCards.Count; index++)
            {
                result.Hand.Cards.Add(allCards[index]);
                result.Hand.Value += (int)allCards[index].Rank;
            }
            return result;
        }
    }
}