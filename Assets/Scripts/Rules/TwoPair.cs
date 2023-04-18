using System.Collections.Generic;
using System.Linq;

public class TwoPair : IRules
{
    public int id;
    private int _rate;

    public TwoPair(int Rate) => _rate = Rate;    

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        List<Card> temp = new List<Card>();
        bool result = false;
        int lastValue = -1;
        int counter = 0;
        Value = 0;
        NameOfCombination = GameConstants.TwoPairs;
        newTemp = new List<Card>();
        if (playerHand == null) 
            return false;
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        CheckJokerGame(IsJokerGame, temp);
        for (var i = 0; i < 2; i++)
            foreach (Card currentCard in temp)
                if (temp.FindAll(card => card.value == currentCard.value).Count == 2)
                {
                    Value += (int)currentCard.value * 2;
                    lastValue = (int)currentCard.value;
                    counter++;
                    Card[] cards = temp.Where(g => g.value == currentCard.value).ToArray();
                    if (cards.Length > 1)
                    {
                        newTemp.Add(cards[0]);
                        newTemp.Add(cards[1]);
                    }
                    if (lastValue != -1)
                        temp.RemoveAll(e => (int)e.value == lastValue);
                    break;
                }
        if (counter == 2)
        {
            result = true;
            Value *= _rate;
        }
        else
        {
            result = false;
            Value *= 0;
        }
        JokerReset(temp);
        return result;
    }

    private static void JokerReset(List<Card> temp)
    {
        foreach (Card card in temp)
            if (card.Front.name == "Joker")
                card.value = ValueCard.JOKER;
    }

    private void CheckJokerGame(bool IsJokerGame, List<Card> temp)
    {
        if (!IsJokerGame)        
            return;        
        foreach (var card in temp.Where(card => card.value == ValueCard.JOKER).Select(card => card))        
            card.value = (ValueCard)MaxValue(temp);        
    }

    private int MaxValue(List<Card> array)
    {
        var max = 0;
        foreach (Card card in array)
            if (max < (int)card.value && array.FindAll(Card => Card.value == card.value).Count < 2 && card.value != ValueCard.JOKER)
                max = (int)card.value;
        return max;
    }
}