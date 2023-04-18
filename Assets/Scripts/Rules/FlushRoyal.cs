using System.Collections.Generic;
using System.Linq;

public class FlushRoyal : IRules
{
    public int id;
    private int _rate;
    public FlushRoyal(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        List<Card> temp = new List<Card>();
        bool result = false;
        Value = 0;
        newTemp = new List<Card>();
        NameOfCombination = GameConstants.RoyalFlush;
        if (playerHand == null || tableCards.Count == 0)
            return false;
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        CheckCombination(IsJokerGame, ref Value, temp, ref result);
        Value = result ? Value *= _rate : 0;
        ResetJokerCards(temp);
        return result;
    }

    private void CheckCombination(bool IsJokerGame, ref int Value, List<Card> temp, ref bool result)
    {
        if (new Flush(0).Check(temp, IsJokerGame, out List<Card> newCards))
        {
            Sorting(newCards);
            newCards.Reverse();
            CheckJokerCount(IsJokerGame, temp, newCards);
            if (newCards.Count > 4 && newCards[0].value == ValueCard.ACE
                   && newCards[1].value == ValueCard.KING
                   && newCards[2].value == ValueCard.QUEEN
                   && newCards[3].value == ValueCard.JACK
                   && newCards[4].value == ValueCard.TEN)
            {
                foreach (Card card in temp)
                    Value += (int)card.value;
                result = true;
            }
        }
    }

    private void CheckJokerCount(bool IsJokerGame, List<Card> temp, List<Card> newCards)
    {
        if (!IsJokerGame)        
            return;
        int trueCounter = 0;
        int jokerCount = 0;
        jokerCount += (temp.Where(card => card.value == ValueCard.JOKER).Select(card => card)).Count();
        if (newCards[0].value == ValueCard.ACE) trueCounter++;
        if (newCards[1].value == ValueCard.KING) trueCounter++;
        if (newCards[2].value == ValueCard.QUEEN) trueCounter++;
        if (newCards[3].value == ValueCard.JACK) trueCounter++;
        if (newCards[4].value == ValueCard.TEN) trueCounter++;
        switch(jokerCount)
        {
            case 1:
                OneJokerCheck(trueCounter, newCards);
                break;
            case 2:
                TwoJokerCheck(trueCounter, newCards);
                break;
        }
    }

    private void ResetJokerCards(List<Card> temp)
    {
        foreach (Card card in temp)
            if (card.Front.name == "Joker")
                card.value = ValueCard.JOKER;
    }

    private void TwoJokerCheck(int trueCounter, List<Card> newCards)
    {
        if (trueCounter == 3)
        {
            for (int i = 0; i < newCards.Count; i++)
            {
                if (newCards[i].value == ValueCard.JOKER)
                {
                    if (newCards[0].value != ValueCard.ACE) { newCards[i].value = ValueCard.ACE; break; }
                    if (newCards[1].value != ValueCard.KING) { newCards[i].value = ValueCard.KING; break; }
                    if (newCards[2].value != ValueCard.QUEEN) { newCards[i].value = ValueCard.QUEEN; break; }
                    if (newCards[3].value != ValueCard.JACK) { newCards[i].value = ValueCard.JACK; break; }
                    if (newCards[4].value != ValueCard.TEN) { newCards[i].value = ValueCard.TEN; break; }
                }
            }
        }
    }

    private void OneJokerCheck(int trueCounter, List<Card> newCards)
    {
        if (trueCounter == 4)
        {
            for (int i = 0; i < newCards.Count; i++)
            {
                if (newCards[i].value == ValueCard.JOKER)
                {
                    if (newCards[0].value != ValueCard.ACE) { newCards[i].value = ValueCard.ACE; break; }
                    if (newCards[1].value != ValueCard.KING) { newCards[i].value = ValueCard.KING; break; }
                    if (newCards[2].value != ValueCard.QUEEN) { newCards[i].value = ValueCard.QUEEN; break; }
                    if (newCards[3].value != ValueCard.JACK) { newCards[i].value = ValueCard.JACK; break; }
                    if (newCards[4].value != ValueCard.TEN) { newCards[i].value = ValueCard.TEN; break; }
                }
            }
        }
    }

    private void Sorting(List<Card> temp)
    {
        Card sortingTemp = new Card();
        for (int i = 0; i < temp.Count - 1; i++)
            for (int j = i + 1; j < temp.Count; j++)
                if (temp[i].value > temp[j].value)
                {
                    sortingTemp = temp[i];
                    temp[i] = temp[j];
                    temp[j] = sortingTemp;
                }
    }
}
