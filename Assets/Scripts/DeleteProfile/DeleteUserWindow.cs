using UnityEngine;
using UnityEngine.UI;

public class DeleteUserWindow : MonoBehaviour
{
    [SerializeField] private Text Title;
    [SerializeField] private TMPro.TMP_Text Messenger;
    [SerializeField] private Text Yes;
    [SerializeField] private Text No;

    [SerializeField] private Button DeleteUser;
    [SerializeField] private Button NotDeleteUser;

    SettingsManager _settingsManager;

    private void Start()
    {
        DeleteUser.onClick.AddListener(ButtonDeleteUser);
        NotDeleteUser.onClick.AddListener(ButtonNotDeleteUser);
        _settingsManager = SettingsManager.Instance;
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        Title.text = _settingsManager.GetString("DeleteUser.Title");
        Messenger.text = _settingsManager.GetString("DeleteUser.Messenger");
        Yes.text = _settingsManager.GetString("DeleteUser.Yes");
        No.text = _settingsManager.GetString("DeleteUser.No");
    }

    void ButtonDeleteUser()
    {
        DeleteUserProfile.Instance.DeleteAccount();
    }
    void ButtonNotDeleteUser()
    {
        this.gameObject.SetActive(false);
    }
}
