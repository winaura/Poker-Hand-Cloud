using System.Collections.Generic;
using System.Linq;

public class FullHouse : IRules
{
    private int _rate;
    public int id;

    public FullHouse(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        List<Card> temp = new List<Card>();
        bool result = false;
        int lastValue = -1;
        int counter = 0;
        Value = 0;
        newTemp = new List<Card>();
        NameOfCombination = GameConstants.FullHouse;
        if (playerHand == null) 
            return false;
        foreach (Card tableCard in tableCards) temp.Add(tableCard);
        foreach (Card handCard in playerHand) temp.Add(handCard);
        JokerCheck(IsJokerGame, temp);
        CheckOnThreeCard(ref Value, temp, ref lastValue, ref counter, ref newTemp);
        if (lastValue != -1) temp.RemoveAll(e => (int)e.value == lastValue);
        CheckOnTwoCards(ref Value, temp, ref counter, ref newTemp);
        CalculateScore(ref Value, ref result, counter);
        ResetJokerCards(temp);
        return result;
    }

    private void CheckOnTwoCards(ref int Value, List<Card> temp, ref int counter, ref List<Card> newTemp)
    {
        int index = 0;
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp.FindAll(Card => Card.value == temp[i].value).Count >= 2)
            {
                index = i;
                break;
            }
        }
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp.FindAll(Card => Card.value == temp[i].value).Count >= 2 && temp[i].value >= temp[index].value)
                index = i;
        }
        if (index == 0) 
            return;
        Value += (int)temp[index].value * 2;
        counter++;
        newTemp.AddRange(temp.Where(g => g.value == temp[index].value).Select(item => item));
    }

    private void CheckOnThreeCard(ref int Value, List<Card> temp, ref int lastValue, ref int counter, ref List<Card> newTemp)
    {
        int index = 0;
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp.FindAll(Card => Card.value == temp[i].value).Count >= 3)
            {
                index = i;
                break;
            }
        }
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp.FindAll(Card => Card.value == temp[i].value).Count == 3 && temp[i].value >= temp[index].value)
                index = i;           
        }

        if (index == 0) 
            return;
        Value += (int)temp[index].value * 3;
        lastValue = (int)temp[index].value;
        counter++;
        newTemp.AddRange(temp.Where(g => g.value == temp[index].value).Select(item => item));
    }

    private void CalculateScore(ref int Value, ref bool result, int counter)
    {
        if (counter == 2)
        {
            result = true;
            Value *= _rate;
        }
    }

    private void ResetJokerCards(List<Card> temp)
    {
        foreach (Card card in temp.Where(card => card.Front.name == "Joker").Select(card => card))
            card.value = ValueCard.JOKER;
    }

    private void JokerCheck(bool IsJokerGame, List<Card> temp)
    {
        if (!IsJokerGame)
            return;
        int jokerCount = temp.Where(card => card.value == ValueCard.JOKER).Select(card => card).Count();
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
        for (var i = 0; i < temp.Count; i++)
        {
            if (temp.FindAll(Card => Card.value == temp[i].value).Count == 3) 
                continue;
            if (temp.FindAll(Card => Card.value == temp[i].value).Count == 2)
            {
                ModifyJokerToNeededCard(temp, i);
                break;
            }
            GetJokerHighValueCard(temp);
        }
    }

    private void OneJokerCheck(List<Card> temp)
    {
        for (var i = 0; i < temp.Count; i++)
        {
            if (temp.FindAll(Card => Card.value == temp[i].value).Count == 3) 
                continue;
            if (temp.FindAll(Card => Card.value == temp[i].value).Count == 2)
            {
                ModifyJokerToNeededCard(temp, i);
                break;
            }
            GetJokerHighValueCard(temp);
        }
    }

    private void ModifyJokerToNeededCard(List<Card> temp, int index)
    {
        foreach (Card currentCard in temp)
            if (currentCard.value == ValueCard.JOKER)
                currentCard.value = temp[index].value;
    }

    private void GetJokerHighValueCard(List<Card> temp)
    {
        foreach (Card currentCard in temp)
            if (currentCard.value == ValueCard.JOKER)
                currentCard.value = (ValueCard)MaxValue(temp);
    }

    private int MaxValue(List<Card> array)
    {
        var max = 0;
        for (var i = 1; i < array.Count; i++)
            if (max < (int)array[i].value && array[i].value != ValueCard.JOKER)
                max = (int)array[i].value;
        return max;
    }
}