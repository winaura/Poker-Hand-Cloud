using PokerHand.Common.Entities;
using System.Collections.Generic;
using UnityEngine;

public class CardManager
{
    private List<Card> _blackList;
    private List<Card> _deck;

    public CardManager()
    {
        _deck = new List<Card>();
        _blackList = new List<Card>();
    }

    private Card GetCard()
    {
        var cardID = 0;
        if (_blackList.Count != 0)
            do
            {
                cardID = Random.Range(0, _deck.Count);
            } 
            while (_blackList.Exists(cardId => cardId.id == cardID));
        else
            cardID = Random.Range(0, _deck.Count);
        AddToBlackList(cardID);
        return _deck[cardID];
    }

    private void AddToBlackList(int idCard)
    {
        if (_blackList == null) 
            _blackList = new List<Card>();
        _blackList.Add(_deck.Find(cardId => cardId.id == idCard));
    }

    public List<Card> GetCards(int count)
    {
        var temp = new List<Card>();
        for (var i = 0; i < count; i++) temp.Add(GetCard());
        return temp;
    }

    public List<Card> MP_ConvertCardsDtoToCards(List<CardDto> cardsDto)
    {
        var cards = new List<Card>();
        foreach(var cardDto in cardsDto)
        {
            foreach(var card in _deck)
            {
                if ((int)cardDto.Rank == (int)card.value && (int)cardDto.Suit == (int)card.suite)
                {
                    cards.Add(card);
                    break;
                }
            }
        }
        return cards;
    }

    public void ResetBlackList() => _blackList.Clear();
    public void SetCards(List<Card> listCard) => _deck = listCard;   
}