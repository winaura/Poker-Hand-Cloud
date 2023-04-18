using PokerHand.Common.Dto;
using UnityEngine;

public class AnotherPlayerProfileController : MultiSceneSingleton<AnotherPlayerProfileController>
{
    [SerializeField] private AnotherPlayerProfileView viewPref;
    [SerializeField] private Canvas overlayCanvasPref;
    AnotherPlayerProfileView view;
    Texture pickedTexture;

    public void SetPickedTexture(Texture texture)
    {
        pickedTexture = texture;
    }

    public void Open(PlayerProfileDto playerProfileDto)
    {
        if (playerProfileDto.Id.Equals(PlayerProfileData.Instance.Id))
            return;
        if (view == null)
        {
            view = Instantiate(viewPref, Instantiate(overlayCanvasPref).transform);
        }
        view.UpdateData(playerProfileDto, pickedTexture);
        pickedTexture = null;
    }

    public void Close()
    {
        if (view != null)
            view.Close();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        Client.OnReceiveAnotherPlayerProfile += Open;
    }

    protected override void UnSubscribeFromEvents()
    {
        base.UnSubscribeFromEvents();
        Client.OnReceiveAnotherPlayerProfile -= Open;
    }
}