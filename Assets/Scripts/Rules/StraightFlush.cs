using System.Collections.Generic;

public class StraightFlush : IRules
{
    public int id;
    private int _rate;

    public StraightFlush(int Rate) => _rate = Rate;
    

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        var temp = new List<Card>();
        var result = false;
        Straight straightCheck = new Straight(0);
        Flush flushCheck = new Flush(0);
        Value = 0;
        newTemp = new List<Card>();
        NameOfCombination = GameConstants.StraightFlush;
        if (playerHand == null) 
            return false;
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        if (straightCheck.Check(playerHand, tableCards, IsJokerGame, out List<Card> newCards) && flushCheck.Check(newCards, IsJokerGame, out List<Card> newFlushCards))
        {
            Value = 0;
            if (newFlushCards.Count > 4)
            {
                foreach (Card card in newFlushCards)
                    Value += (int)card.value;
                newTemp = newFlushCards;
                result = true;
            }
        }
        Value = CalculateScore(Value, result);
        return result;
    }

    private int CalculateScore(int Value, bool result)
    {
        if (result)
            Value *= _rate;
        return Value;
    }
}