using System.Collections.Generic;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers.CardEvaluation;

namespace PokerHand.BusinessLogic.Helpers.CardEvaluationLogic.Interfaces
{
    public interface IRulesDto
    {
        EvaluationResult Check(List<CardDto> playerHand, List<CardDto> tableCards);
    }
}