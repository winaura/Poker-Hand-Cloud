using System;
using System.Collections.Generic;

public class Straight1 : IRules
{
    public int id;
    private int _rate;

    public Straight1(int Rate) => _rate = Rate;    

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        Value = 0;
        var result = false;
        NameOfCombination = GameConstants.Straight;
        newTemp = new List<Card>();
        if (playerHand == null) 
            return false;
        Check(playerHand, tableCards, IsJokerGame, out List<Card> newCards);
        if (newCards.Count > 4)
        {
            for (var i = 0; i < newCards.Count; i++)
                Value += (int)newCards[i].value;
            result = true;
        }
        if (result)
            Value *= _rate;
        return result;
    }

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out List<Card> newCards)
    {
        var temp = new List<Card>();
        newCards = new List<Card>();
        var result = false;
        var subTrueCounter = 0;
        var trueCounter = 0;
        for (var i = 0; i < tableCards.Count; i++) 
            temp.Add(tableCards[i]);
        for (var i = 0; i < playerHand.Count; i++) 
            temp.Add(playerHand[i]);
        Sorting(temp);
        temp.Reverse();
        if (IsJokerGame)
        {
            if (temp.FindAll(Card => ValueCard.JOKER == Card.value).Count == 1)
            {
                for (int i = 0; i < tableCards.Count - 2; i++)
                {
                    if (temp[i].value - 1 == temp[i + 1].value) subTrueCounter++;
                    if (temp[i + 1].value - 1 == temp[i + 2].value) subTrueCounter++;
                    if (temp[i + 2].value - 1 == temp[i + 3].value) subTrueCounter++;
                    if (temp[i + 3].value - 1 == temp[i + 4].value) subTrueCounter++;
                    if (subTrueCounter > trueCounter) trueCounter = subTrueCounter;
                    subTrueCounter = 0;
                }
                if (trueCounter == 3)
                {
                    for (int i = 0; i < trueCounter; i++)
                    {
                        if (temp[i].value - 1 != temp[i + 1].value)
                        {
                            for (int j = 0; j < temp.Count; j++)
                                if (temp[j].value == ValueCard.JOKER)
                                    temp[j].value = temp[i].value - 1;
                            break;
                        }

                        if (temp[i + 3].value - 1 != temp[i + 4].value)
                        {
                            for (int j = 0; j < temp.Count; j++)
                                if (temp[j].value == ValueCard.JOKER)
                                    temp[j].value = temp[i + 3].value + 1;
                            break;
                        }
                    }
                }
            }
            else if (temp.FindAll(Card => ValueCard.JOKER == Card.value).Count == 2)
            {
                for (int i = 0; i < tableCards.Count - 2; i++)
                {
                    if (temp[i].value - 1 == temp[i + 1].value) subTrueCounter++;
                    if (temp[i + 1].value - 1 == temp[i + 2].value) subTrueCounter++;
                    if (temp[i + 2].value - 1 == temp[i + 3].value) subTrueCounter++;
                    if (temp[i + 3].value - 1 == temp[i + 4].value) subTrueCounter++;
                    if (subTrueCounter > trueCounter) trueCounter = subTrueCounter;
                    subTrueCounter = 0;
                }
                if (trueCounter == 2)
                {
                    for (int i = 0; i < trueCounter; i++)
                    {

                        if (temp[i].value - 1 != temp[i + 1].value)
                            for (int j = 0; j < temp.Count; j++)
                                if (temp[j].value == ValueCard.JOKER)
                                    temp[j].value = temp[i + 1].value + 1;
                        if (temp[i + 1].value - 1 != temp[i + 2].value)
                            for (int j = 0; j < temp.Count; j++)
                                if (temp[j].value == ValueCard.JOKER)
                                    temp[j].value = temp[i + 2].value + 1;
                        if (temp[i + 2].value - 1 != temp[i + 3].value)
                            for (int j = 0; j < temp.Count; j++)
                                if (temp[j].value == ValueCard.JOKER)
                                    temp[j].value = temp[i + 3].value + 1;
                        if (temp[i + 3].value - 1 != temp[i + 4].value)
                            for (int j = 0; j < temp.Count; j++)
                                if (temp[j].value == ValueCard.JOKER)
                                    temp[j].value = temp[i + 3].value - 1;
                    }
                }
                Sorting(temp);
                temp.Reverse();
            }
        }
        int count = 0;
        Card lastCard = null;
        Card card = null;
        for (int i = 0; i < tableCards.Count - 2; i++)
        {
            for (int j = 0; j < temp.Count; j++)
            {
                if (newCards.Find(e => (int)e.value == (int)temp[j].value) == null)
                {
                    if (lastCard == null)
                        lastCard = temp[j];
                    card = temp.Find(e => e.value == lastCard.value - 1);
                    if (count == 4)
                    {
                        count++;
                        newCards.Add(lastCard);
                        break;
                    }
                    if (card != null)
                    {
                        count++;
                        lastCard = card;
                        newCards.Add(temp[j]);
                    }
                    else
                    {
                        count = 0;
                        newCards.Clear();
                        lastCard = null;
                    }

                }
            }
            if (count != 5)
                newCards.Clear();
        }

        temp.Reverse();
        if (!result && (int)temp[0].value == 2
              && (int)temp[1].value == 3
              && (int)temp[2].value == 4
              && (int)temp[3].value == 5
              && (int)temp[temp.Count - 1].value == 14) //14 - ace number from enum 
        {
            result = true;
            newCards.Add(temp[0]);
            newCards.Add(temp[1]);
            newCards.Add(temp[2]);
            newCards.Add(temp[3]);
            newCards.Add(temp[temp.Count - 1]);
        }
        for (int i = 0; i < temp.Count; i++)
            if (temp[i].Front.name == "Joker")
                temp[i].value = ValueCard.JOKER;
        return result;
    }

    private void Sorting(List<Card> list)
    {
        Card sortingTemp = new Card();
        for (int i = 0; i < list.Count - 1; i++)
            for (int j = i + 1; j < list.Count; j++)
                if (list[i].value > list[j].value)
                {
                    sortingTemp = list[i];
                    list[i] = list[j];
                    list[j] = sortingTemp;
                }
    }
}