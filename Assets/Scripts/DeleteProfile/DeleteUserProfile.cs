using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Hub;

public class DeleteUserProfile : Singleton<DeleteUserProfile>
{
    public Action DeleteUser;
    public void DeleteAccount()
    {
        Client.HTTP_DeleteAccount();
        DeleteUser?.Invoke();
    }
    public static void DeleteFinal()
    {
        PlayerPrefs.DeleteAll();
        NetworkManager.Instance.ResetProfileData();
        OnDisconnectedFromServer += OnHubClosed;
        CloseAsync();
    }

    private static void OnHubClosed()
    {
        OnDisconnectedFromServer -= OnHubClosed;
        SceneManager.LoadScene("MainMenu");
    }
}
