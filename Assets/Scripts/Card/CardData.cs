using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public List<Card> CardDeckTexas;
    public List<Card> CardDeckRoyal;
    public List<Card> CardDeckJoker;

    public void Awake()
    {
        switch (GameModeSettings.Instance?.gameMode)
        {
            case GameModes.Royal:
                for (var i = 0; i < CardDeckRoyal.Count; i++) 
                    CardDeckRoyal[i].id = i;
                break;
            case GameModes.Joker:
                for (var i = 0; i < CardDeckJoker.Count; i++)
                    CardDeckJoker[i].id = i;
                break;
            case GameModes.Texas:
                for (var i = 0; i < CardDeckTexas.Count; i++)
                    CardDeckTexas[i].id = i;
                break;
        }
    }
}