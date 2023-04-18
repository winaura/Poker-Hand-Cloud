using System.Collections.Generic;
using UnityEngine;

public class OpenCardsChoiseWindowController : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    [SerializeField] GameObject viewObjPrefab;
    GameObject viewObject = null;
    OpenCardsChoiseWindowView view;
    List<SpriteRenderer> _mySpritesCard = new List<SpriteRenderer>();

    public void Open(List<SpriteRenderer> cards)
    {
        if (viewObject == null)
        {
            viewObject = Instantiate(viewObjPrefab, parentTransform);
            view = viewObject.GetComponent<OpenCardsChoiseWindowView>();
            view.OnNoClick += Decline;
            view.OnYesClick += Accept;
            view.OnTimerEnds += Decline;
        }
        view.StartTimer(GameConstants.OpenCardsChoiseLifetime);
        for(int i = 1; i < cards.Count; i += 2)
        {
            _mySpritesCard.Add(cards[i]);
            cards[i].enabled = true;
        }
    }

    public void Decline() => Close();
       
    public void Accept()
    {
        Client.SendShowWinnerCards();
        Client.ShowWinnerCards();
        Close();
    }

    public void Close()
    {
        _mySpritesCard.Clear();
        Destroy(viewObject);
    }
}