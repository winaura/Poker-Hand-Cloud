using System.Collections.Generic;

public interface IRules
{
    bool Check(List<Card> playerHand, List<Card> tableCards, bool isJokerGame, out int Value, out string NameOfCombination, out List<Card> winnerCards);
}