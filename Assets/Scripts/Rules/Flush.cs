using System.Collections.Generic;
using System.Linq;

public class Flush : IRules
{
    public int id;
    private int _rate;
    public Flush(int Rate) => _rate = Rate;    

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        Value = 0;
        bool result = false;
        var temp = new List<Card>();
        newTemp = new List<Card>();
        NameOfCombination = GameConstants.Flush;
        if (playerHand == null) 
            return false;
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        if (Check(temp, IsJokerGame, out List<Card> newCards) && newCards.Count > 4)
        {
            for (var i = 0; i < newCards.Count; i++)
                Value += (int)newCards[i].value;
            newTemp = newCards;
            Value *= _rate;
            result = true;
        }
        return result;
    }

    public bool Check(List<Card> temp, bool IsJokerGame, out List<Card> newCards)
    {
        newCards = new List<Card>();
        bool result = false;
        int jokerCount = temp.FindAll(Card => ValueCard.JOKER == Card.value).Count;
        FindAndTransformJoker(temp, IsJokerGame, jokerCount);
        result = FindFlush(temp, newCards, result);
        ResetJoker(temp);
        return result;
    }

    private void ResetJoker(List<Card> temp)
    {
        foreach (Card tempCard in temp.Where(tempCard => tempCard.Front.name == "Joker").Select(tempCard => tempCard))
            tempCard.value = ValueCard.JOKER;
    }

    private bool FindFlush(List<Card> temp, List<Card> newCards, bool result)
    {
        if (temp.Count < 3)
            return false;
        //нет смысла проверять больше 3-х
        //т.к. уже не будут 5 карт для комбинации
        for (var i = 0; i < 3; i++)
        {
            var CountSuits = temp.FindAll(Card => Card.suite == temp[i].suite).Count;
            if (CountSuits >= 5)
            {
                foreach (Card newCard in temp)
                    if (newCard.suite == temp[i].suite)
                        newCards.Add(newCard);
                result = true;
                break;
            }
        }
        return result;
    }

    private void FindAndTransformJoker(List<Card> temp, bool IsJokerGame, int jokerCount)
    {
        if (IsJokerGame)
        {
            switch(jokerCount)
            {
                case 1:
                    if (temp.FindAll(Card => Card.suite == temp[0].suite).Count == 4)
                        foreach (Card currentCard in temp)
                            if (currentCard.value == ValueCard.JOKER)
                                currentCard.suite = temp[0].suite;
                    break;
                case 2:
                    if (temp.FindAll(Card => Card.suite == temp[0].suite).Count == 3)
                        foreach (Card currentCard in temp)
                            if (currentCard.value == ValueCard.JOKER)
                                currentCard.suite = temp[0].suite;
                    break;
            }
        }
    }
}
