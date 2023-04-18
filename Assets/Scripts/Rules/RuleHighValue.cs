using System.Collections.Generic;

public class RuleHighValue : IRules
{
    public int id;
    private int _rate;

    public RuleHighValue(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        List<Card> temp = new List<Card>();
        var result = true;
        Value = 0;
        newTemp = new List<Card>();
        NameOfCombination = GameConstants.HighValue;
        if (playerHand == null)
        {
            Value *= _rate;
            return result;
        }
        foreach (Card card in playerHand) 
            temp.Add(card);
        foreach (Card card in temp)
            if ((int)card.value > Value) 
            {
                Value = (int)card.value;
                newTemp.Add(card);
            }
        if (result)
            Value *= _rate;
        return result;
    }
}