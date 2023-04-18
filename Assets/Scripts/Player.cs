using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    //Unchanged data
    public int iD;
    public string nickname = "Anonim";
    public long money = 1000;
    public int level = 1;
    public float currentExp = 0;
    public int bigCoins = 0;
    //Changed data
    public long bet;
    public int allMoneyBet;
    public List<Card> CardsHand;
    public List<CardUnit> myCards;
    public int maxValue;
    public string nameOfCombination;
    public int kickCounter;
    public List<Card> combination = new List<Card>();
    public AudioSource audioSource;
    public bool fold;
    public bool call;
    public bool callAny;
    public bool allIn;
    public bool isBankCollected = false;
    public bool _isMyPlayer;
    private Player player;

    public void FullReset()
    {
        allIn = false;
        call = false;               
        callAny = false;
        fold = false;
        CardsHand = null;
        bet = 0;
        allMoneyBet = 0;
        myCards = new List<CardUnit>();
        combination = new List<Card>();
        maxValue = 0;
    }

    public List<Card> CardsOnHand
    {
        get => CardsHand;
        set
        {
            CardsHand = value;
            maxValue = (int)CardsHand[0].value 
                > (int)CardsHand[1].value
                ? (int)CardsHand[0].value
                : (int)CardsHand[1].value;
        }
    }
}