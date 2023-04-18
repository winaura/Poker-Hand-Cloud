using System.Collections.Generic;
using UnityEngine;
using System;
using PokerHand.Common.Entities;

[CreateAssetMenu]
public class WinnersDataContainer : ScriptableObject
{
    [Header("All Cards")] public Sprite[] clubsSpriteList;
    public Sprite[] diamondsSpriteList;
    public Sprite[] heartsSpriteList;
    public Sprite[] spadesSpriteList;
    public Sprite jokerSprite;
    [Header("Last Combination Active Players List")] public List<ActivePlayer> lastCombinationActivePlayersList;
    [Header("Last Cards On Table List")] public List<Sprite> lastCardsOnTableList;
    [Header("Active Players List")] public List<ActivePlayer> activePlayersList;
    [Header("Cards On Table List")] public List<Sprite> cardsOnTableList;
    public Dictionary<Guid, long> winningAmountPerPlayerDictionary = new Dictionary<Guid, long>();
    public bool isAvaibleToShow;

    public Sprite GetCardSprite(CardDto card)
    {
        var index = (int)card.Rank - 2;
        var suit = (int)card.Suit;
        switch (suit)
        {
            case 0:
                return heartsSpriteList[index];
            case 1:
                return diamondsSpriteList[index];
            case 2:
                return clubsSpriteList[index];
            case 3:
                return spadesSpriteList[index];
            default:
                return jokerSprite;
        }
    }
}

[Serializable]
public class ActivePlayer
{
    public Guid Id;
    public string userName;
    public string winningAmountPerPlayer;
    public string combinationName;
    public List<Sprite> playerCardList;
    public List<Sprite> playerWinningCombinationList;
}