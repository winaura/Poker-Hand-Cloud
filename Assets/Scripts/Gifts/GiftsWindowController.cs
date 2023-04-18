using PokerHand.Common.Dto;
using System;
using UnityEngine;

public class GiftsWindowController : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    [SerializeField] GameObject viewObjPrefab;
    [SerializeField] GiftData giftData;
    public event Action<PresentInfoDto> OnGiftToAll;
    public event Action<PresentInfoDto> OnGiftToPlayer;
    GameObject viewObject = null;
    GiftsWindowView view;
    PresentInfoDto currentPresent = null;

    public void Open()
    {
        if (viewObject == null)
            CreateAndInitView();
        var buttonState = PlayerProfileData.Instance.TotalMoney > 0 && currentPresent != null;
        var num = currentPresent == null ? 0 : currentPresent.Price;
        view.SetTexts();
        view.SetPlayerButtonState(buttonState, num);
        view.SetTableButtonState(buttonState, Client.TableData.Players.Count * num);
        viewObject.SetActive(true);
    }

    void CreateAndInitView()
    {
        viewObject = Instantiate(viewObjPrefab, parentTransform);
        view = viewObject.GetComponent<GiftsWindowView>();
        view.OnGiftButtonClick += OnGiftClick;
        view.OnTableButtonClick += OnTableClick;
        view.OnPlayerButtonClick += OnPlayerClick;
        view.OnCloseButtonClick += Close;
        view.CreateGiftButtons(Client.PresentsInfo.Count);
        for (var i = 0; i < Client.PresentsInfo.Count; i++)
        {
            var sprite = giftData.GiftsInfo.GetGiftSpriteByName(Client.PresentsInfo[i].Name);
            view.SetGiftInfo(i, Client.PresentsInfo[i].Price, sprite);
        }
    }

    void OnGiftClick(int index)
    {
        var gift = Client.PresentsInfo[index];
        currentPresent = gift;
        var totalMoney = PlayerProfileData.Instance.TotalMoney;
        if (totalMoney >= gift.Price)
            view.SetPlayerButtonState(true, gift.Price);
        else
            view.SetPlayerButtonState(false);
        var tableCost = Client.TableData.Players.Count * gift.Price;
        if (totalMoney >= tableCost)
            view.SetTableButtonState(true, tableCost);
        else
            view.SetTableButtonState(false);
    }

    void OnTableClick()
    {
        if (currentPresent != null)
        {
            OnGiftToAll?.Invoke(currentPresent);
            Close();
        }
    }

    void OnPlayerClick()
    {
        if (currentPresent != null)
        {
            OnGiftToPlayer?.Invoke(currentPresent);
            Close();
        }
    }

    public void Close() => viewObject.SetActive(false);
}