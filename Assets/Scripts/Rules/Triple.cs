using System.Collections.Generic;
using System.Linq;

public class Triple : IRules
{
    public int id;
    private int _rate;

    public Triple(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        var temp = new List<Card>();
        var result = false;
        Value = 0;
        NameOfCombination = GameConstants.ThreeOfAKind;
        newTemp = new List<Card>();
        if (playerHand == null) 
            return false;
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        JokerCheck(temp, GetJokerCount(IsJokerGame, temp));
        CheckCombination(ref Value, ref newTemp, temp, ref result);
        ResetJokers(temp);
        return result;
    }

    private void CheckCombination(ref int Value, ref List<Card> newTemp, List<Card> temp, ref bool result)
    {
        foreach (Card currentCard in temp)
        {
            newTemp = temp.Where(g => g.value == currentCard.value).ToList();
            if (newTemp.Count == 3)
            {
                Value += (int)currentCard.value * 3;
                result = true;
                break;
            }
        }
        if (result)
            Value *= _rate;
    }

    private void ResetJokers(List<Card> temp)
    {
        foreach (var card in temp.Where(card => card.Front.name == "Joker").Select(card => card))
            card.value = ValueCard.JOKER;
    }

    private int GetJokerCount(bool IsJokerGame, List<Card> temp)
    {
        if (!IsJokerGame)
            return 0;
        return temp.Where(card => card.value == ValueCard.JOKER).Select(card => card).Count();
    }

    private void JokerCheck(List<Card> temp, int jokerCount)
    {
        switch(jokerCount)
        {
            case 1:
                OneJokerCheck(temp);
                break;
            case 2:
                TwoJokerCheck(temp);
                break;
        }
    }

    private void TwoJokerCheck(List<Card> temp)
    {
        foreach (Card card in temp)
            if (card.value == ValueCard.JOKER)
                card.value = (ValueCard)MaxValue(temp);
    }

    private void OneJokerCheck(List<Card> temp)
    {
        foreach (Card tempCard in temp)
            if (temp.FindAll(Card => Card.value == tempCard.value).Count == 2)
                foreach (Card currentCard in temp.Where(currentCard => currentCard.value == ValueCard.JOKER).Select(currentCard => currentCard))
                {
                    currentCard.value = tempCard.value;
                    break;
                }
    }

    private int MaxValue(List<Card> array)
    {
        var max = 0;
        foreach (Card card in array)
            if (max < (int)card.value && card.value != ValueCard.JOKER)
                max = (int)card.value;
        return max;
    }
}
