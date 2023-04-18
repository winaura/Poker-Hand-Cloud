using UnityEngine;

public class PlayerProfileWindowController : MonoBehaviour
{
    [SerializeField] Canvas overlayCanvas;
    [SerializeField] PlayerProfileWindow playerProfileWindowPrefab;
    PlayerProfileWindow playerProfileWindow = null;

    private void OnDisable()
    {
        if (playerProfileWindow != null)
        {
            playerProfileWindow.OnCloseButtonPressed -= OnCloseButtonPressed;
            playerProfileWindow.OnEditImageButtonPressed -= OnEditImageButtonPressed;
            playerProfileWindow.OnEditNicknameButtonPressed -= OnEditNicknameButtonPressed;
            playerProfileWindow.OnConfirmCountryButtonPressed -= OnConfirmCountryButtonPressed;
        }
    }

    public void OpenWindow()
    {
        Client.HTTP_SendGetPlayerProfile(PlayerProfileData.Instance.Id);
        playerProfileWindow = Instantiate(playerProfileWindowPrefab, Instantiate(overlayCanvas).transform);
        StartListenEvents();
        playerProfileWindow.SetWindowTexts();
        playerProfileWindow.UpdateProfileView();
    }

    private void StartListenEvents()
    {
        playerProfileWindow.OnCloseButtonPressed += OnCloseButtonPressed;
        playerProfileWindow.OnEditImageButtonPressed += OnEditImageButtonPressed;
        playerProfileWindow.OnEditNicknameButtonPressed += OnEditNicknameButtonPressed;
        playerProfileWindow.OnConfirmCountryButtonPressed += OnConfirmCountryButtonPressed;
    }

    private void OnCloseButtonPressed() => Destroy(playerProfileWindow.transform.parent.gameObject);

    private void OnEditImageButtonPressed() => playerProfileWindow.SetAvatar();

    private void OnEditNicknameButtonPressed() => playerProfileWindow.OpenProfileEditor();  

    private void OnConfirmCountryButtonPressed() => playerProfileWindow.ConfirmCountry();
}