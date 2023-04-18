using UnityEngine;

public enum Suite
{
    HEARTS = 0,
    DIAMONDS = 1,
    CLUBS = 2,
    SPADES = 3,
    RED = 4,
    BLACK = 5
}

public enum ValueCard
{
    TWO = 2,
    THREE = 3,
    FOUR = 4,
    FIVE = 5,
    SIX = 6,
    SEVEN = 7,
    EIGHT = 8,
    NINE = 9,
    TEN = 10,
    JACK = 11,
    QUEEN = 12,
    KING = 13,
    ACE = 14,
    JOKER = 15
}

[CreateAssetMenu(fileName = "Create new card", menuName = "Create new card")]
public class Card : ScriptableObject
{
    public int id;
    public Suite suite;
    public ValueCard value;   
    public Sprite Front;
    public Sprite Back;
}