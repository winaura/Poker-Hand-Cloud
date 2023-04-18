using System.Collections.Generic;
using UnityEngine;

public class RulesSessions : MonoBehaviour
{
    public static RulesSessions Instance;
    private IRules _currentRules;
    public List<IRules> listRules;
    private bool result;

    private void Awake()
    {
        Instance = this;
        listRules = new List<IRules>
        {
            new FlushRoyal(200000),
            new StraightFlush(180000),
            new FourCard(60000),
            new FullHouse(6700),
            new Flush(1300),
            new Straight(400),
            new Triple(170),
            new TwoPair(17),
            new OnePair(5),
            new RuleHighValue(1)
        };
    }

    public bool Check(List<Card> playerHand, List<Card> tableCards, bool IsJokerGame ,out int value, out string nameOfCombination, out List<Card> newTemp)
    {
        value = 0;
        nameOfCombination = string.Empty;
        newTemp = new List<Card>();
        foreach (IRules rule in listRules)
        {
            _currentRules = rule;
            result = _currentRules.Check(playerHand, tableCards, IsJokerGame, out value, out nameOfCombination, out newTemp);
            if (result) 
                break;
        }
        return result;
    }
}