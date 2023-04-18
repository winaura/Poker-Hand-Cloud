using System.Linq;
using System.Collections.Generic;

public class Straight : IRules
{
    public int id;
    private int _rate;
    private bool result;
    private List<Card> newCards;

    public Straight(int Rate) => _rate = Rate;

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame, out int Value, out string NameOfCombination, out List<Card> newTemp)
    {
        NameOfCombination = GameConstants.Straight;
        Value = 0;
        newTemp = new List<Card>();
        if (playerHand == null) return false;
        Check(playerHand, tableCards, IsJokerGame, out List<Card> newCards);
        newTemp = newCards;
        Value = GetScore();
        return result;
    }

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool isJokerGame, out List<Card> newDeckOfCard)
    {
        newCards = new List<Card>();
        List<Card> temp = new List<Card>();
        foreach (Card tableCard in tableCards) 
            temp.Add(tableCard);
        foreach (Card handCard in playerHand) 
            temp.Add(handCard);
        Sorting(temp);
        MainStraightCheck(CheckOnJoker(temp, isJokerGame));
        CheckOnLowAceStraight(temp);
        ResetJokerCards(isJokerGame, temp);
        newDeckOfCard = newCards;
        return result;
    }   

    private List<Card> CheckOnJoker(List<Card> temp, bool isJokerGame)
    {
        if (!isJokerGame) 
            return temp;
        int jokerCount = 0;
        foreach (Card card in temp)        
            if (card.value == ValueCard.JOKER)
                jokerCount++;
        switch(jokerCount)
        {
            case 1:
                temp = CountOneJoker(temp);
                break;
            case 2:
                temp = CountOneJoker(CountOneJoker(temp));
                break;
        }
        return temp;
    }

    private List<Card> CountOneJoker(List<Card> temp)
    {
        List<Card> result = new List<Card>(0);
        var distinctTemp = temp.Select(card => card.value).Distinct().ToList();
        if (distinctTemp.Count > 4)
        {
            for (int i = distinctTemp.Count - 2; i > 2; i--)
            {
                var subtraction = distinctTemp[i] - distinctTemp[i - 3];
                if (subtraction == 4 || subtraction == 3)
                {
                    var newDistinctTemp = distinctTemp.GetRange(i - 3, 4);
                    SetNewResult(result, newDistinctTemp);
                    JokerChange(result, temp);
                    break;
                }
            }
        }
        return result;
    }

    private List<Card> CountTwoJoker(List<Card> temp)
    {
        List<Card> result = new List<Card>(0);
        var distinctTemp = temp.Select(card => card.value).Distinct().ToList();
        return result;
    }

    private void SetNewResult(List<Card> result, List<ValueCard> distinctTemp)
    {
        foreach (ValueCard newValue in distinctTemp)
        {
            result.Add(new Card
            {
                value = newValue
            });
        }
    }

    private void JokerChange(List<Card> result, List<Card> temp)
    {
        //Если два джокера, то один из них может оказаться с таким же value как и второй
        //но это не точно
        foreach (Card card in temp)
            if (card.value == ValueCard.JOKER)
                for (int j = 0; j < result.Count - 1; j++)
                {
                    if (result[j].value + 1 == result[j + 1].value) 
                        continue;
                    Card newCard = new Card { value = result[j].value + 1 };
                    if (newCard.value > ValueCard.ACE)
                    {
                        newCard.value = result[0].value - 1;
                        result.Add(newCard);
                    }
                    else
                        result.Add(newCard);
                    break;
                }
    }

    private void MainStraightCheck(List<Card> temp)
    {
        var distinctTemp = temp.Select(card => card.value).Distinct().ToList();
        if (distinctTemp.Count > 4)
        {
            for (int i = 0; i < temp.Count() - 4; i++)
            {
                result = IsFiveCardsStraight(distinctTemp);
                if (result)
                {
                    AddToNewCard(temp, distinctTemp);
                    break;
                }
            }
        }
        else        
            result = false;        
    }

    private bool IsFiveCardsStraight(List<ValueCard> array)
    {
        return array[0] == array[1] - 1
        && array[0] == array[2] - 2
        && array[0] == array[3] - 3
        && array[0] == array[4] - 4;
    }

    private void AddToNewCard(List<Card> temp, List<ValueCard> distinctTemp)
    {
        for (int i = 0; i < distinctTemp.Count; i++)
            foreach (Card card in temp)
                if (card.value == distinctTemp[i])
                {
                    newCards.Add(card);
                    break;
                }
    }

    private void CheckOnLowAceStraight(List<Card> temp)
    {
        if (result)        
            return;
        if (temp[0].value == ValueCard.ACE
           && temp[temp.Count - 4].value == ValueCard.FIVE
           && temp[temp.Count - 3].value == ValueCard.FOUR
           && temp[temp.Count - 2].value == ValueCard.THREE
           && temp[temp.Count - 1].value == ValueCard.TWO)
        {
            result = true;
            newCards.Add(temp[0]);
            newCards.Add(temp[temp.Count - 4]);
            newCards.Add(temp[temp.Count - 3]);
            newCards.Add(temp[temp.Count - 2]);
            newCards.Add(temp[temp.Count - 1]);
        }
    }

    private int GetScore()
    {
        int count = 0;
        if (newCards.Count > 4)
        {
            for (var i = 0; i < newCards.Count; i++)
                count += (int)newCards[i].value;
            result = true;
        }
        if (result)
            count *= _rate;
        return count;
    }

    private void ResetJokerCards(bool isJokerGame, List<Card> temp)
    {
        if (isJokerGame)
            foreach (Card card in temp)
                if (card.Front.name == "Joker")
                    card.value = ValueCard.JOKER;
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