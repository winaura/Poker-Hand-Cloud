using System.Collections.Generic;
using System.Linq;

public class OnePair : IRules
{
    public int id;
    private int _rate;

    public OnePair(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        var temp = new List<Card>();
        var result = false;
        newTemp = new List<Card>();
        Value = 0;
        NameOfCombination = GameConstants.Pair;
        if (playerHand == null) 
            return false;
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        if (IsJokerGame)
            for (int i = 0; i < temp.Count; i++)
                if (temp[i].value == ValueCard.JOKER)
                    temp[i].value = (ValueCard)MaxValue(temp);
        foreach (Card currentCard in temp)
        {
            newTemp = temp.Where(g => g.value == currentCard.value).ToList();
            if (newTemp.Count == 2) 
            {
                Value += (int)currentCard.value * 2;
                result = true;
                break;
            }
        }
        if (result)
            Value *= _rate;
        foreach (Card currentCard in temp)
            if (currentCard.Front.name == "Joker")
                currentCard.value = ValueCard.JOKER;
        return result;
    }

    private int MaxValue(List<Card> array)
    {
        int max = 0;
        foreach (Card card in array)
            if (max < (int)card.value && card.value != ValueCard.JOKER)
                max = (int)card.value;
        return max;
    }
}