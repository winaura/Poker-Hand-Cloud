using System.Collections.Generic;
using System.Linq;

public class FourCard : IRules
{
    public int id;
    private int _rate;

    public FourCard(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        Value = 0;
        newTemp = new List<Card>();
        NameOfCombination = GameConstants.FourOfAKind;
        if (playerHand == null)
            return false;
        var temp = new List<Card>();
        var result = false;
        foreach (Card card in tableCards) { temp.Add(card); }
        foreach (Card card in playerHand) { temp.Add(card); }
        JokerCheck(IsJokerGame, temp);
        CheckCombination(ref Value, temp, ref result, ref newTemp);
        JokerReset(temp);
        return result;
    }

    private void CheckCombination(ref int Value, List<Card> temp, ref bool result, ref List<Card> newTemp)
    {
        foreach (Card card in temp)
        {
            newTemp = temp.Where(g => g.value == card.value).ToList();
            if (newTemp.Count == 4)
            {
                Value += (int)card.value * 4;
                result = true;
                Value *= _rate;
                break;
            }
        }
    }

    private void JokerCheck(bool IsJokerGame, List<Card> temp)
    {
        if (!IsJokerGame)
            return;
        int jokerCount = 0;
        for (int i = 0; i < temp.Count; i++)
            if (temp[i].value == ValueCard.JOKER)
                jokerCount++;
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

    private void JokerReset(List<Card> temp)
    {
        for (int i = 0; i < temp.Count; i++)
            if (temp[i].Front.name == "Joker")
                temp[i].value = ValueCard.JOKER;
    }

    private void TwoJokerCheck(List<Card> temp)
    {
        foreach (Card card in temp)
        {
            if (temp.FindAll(Card => Card.value == card.value).Count == 2)
            {
                if (card.value == ValueCard.JOKER) 
                    continue;
                foreach (Card tempCard in temp)
                    if (tempCard.value == ValueCard.JOKER)
                        tempCard.value = card.value;
            }
            OneJokerCheck(temp);
        }
    }

    private void OneJokerCheck(List<Card> temp)
    {
        foreach (Card card in temp)
            if (temp.FindAll(Card => Card.value == card.value).Count == 3)
                foreach (Card tempCard in temp)
                    if (tempCard.value == ValueCard.JOKER)
                    {
                        tempCard.value = card.value;
                        break;
                    }
    }
}
