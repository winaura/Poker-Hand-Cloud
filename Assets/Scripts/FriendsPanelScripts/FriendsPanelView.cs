using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FriendsPanelView : MonoBehaviour
{
    [Header("Panels"), SerializeField] private Transform _content;
    [SerializeField] private GameObject _friendDescription;
    [SerializeField] private GameObject _friendRequests;
    [Header("Buttons"), SerializeField] private Button _showFriendsDescriptionPanel;
    [SerializeField] private Button _showRequestsPanel;
    [SerializeField] private Button _closeFriendsPanel;
    [Header("FriendsWindowTexts"), SerializeField] private Text _friends;
    [SerializeField] private Text _friendsOutline;
    [SerializeField] private Text _requests;
    [SerializeField] private Text _requestsOutline;
    [SerializeField] private Text _codeForFriends;
    [SerializeField] private Text _addToFriendText;
    [SerializeField] private Text _inviteToFriendText;
    private WindowType windowType;

    private void Awake()
    {
        SetTexts();
        _closeFriendsPanel.onClick.AddListener(ClosePanel);
        _showFriendsDescriptionPanel.onClick.AddListener(ShowFriendsPanel);
        _showRequestsPanel.onClick.AddListener(ShowRequestsPanel);
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView += UpdateFriendsPanelView;
    }

    private void OnEnable()
    {
        AddToStaticDictionary();
        ShowFriendsPanel();
        StartCoroutine(CheckOnlineStatus());
        StaticRuntimeSets.Add(gameObject.name, gameObject);
    }

    private void OnDestroy()
    {
        ReceiveFriendsDataFromServer.OnUpdateFriendsPanelView -= UpdateFriendsPanelView;
        StaticRuntimeSets.Items.Clear();
    }

    private void SetTexts()
    {
        _friends.text = SettingsManager.Instance.GetString("FriendsPanel.Friends");
        _friendsOutline.text = SettingsManager.Instance.GetString("FriendsPanel.Friends");
        _requests.text = SettingsManager.Instance.GetString("FriendsPanel.Requests");
        _requestsOutline.text = SettingsManager.Instance.GetString("FriendsPanel.Requests");
        _addToFriendText.text = SettingsManager.Instance.GetString("FriendsPanel.AddToFriendText");
        _inviteToFriendText.text = SettingsManager.Instance.GetString("FriendsPanel.InviteToFriendText");
    }

    private void ShowFriendsPanel()
    {
        windowType = WindowType.FriendsWindow;
        CleanContent();
        _codeForFriends.text = $"{ SettingsManager.Instance.GetString("FriendsPanel.CodeForFriends")}: { ReceiveFriendsDataFromServer.PersonalCode}";
        if (ReceiveFriendsDataFromServer.friendRequestsList.Count != 0)
        {
            _requests.text += ReceiveFriendsDataFromServer.friendRequestsList.Count;
            _requestsOutline.text = SettingsManager.Instance.GetString("FriendsPanel.Requests") + " " + ReceiveFriendsDataFromServer.friendRequestsList.Count;
        }
        for (int i = 0; i < ReceiveFriendsDataFromServer.friendsList.Count; i++)
        {
            var descriptionPanel = Instantiate(_friendDescription, _content);
            var descriptionComponent = descriptionPanel.GetComponent<FriendsDescriptionView>();
            descriptionComponent.SetFriendData
                (
                ReceiveFriendsDataFromServer.friendsList[i].UserName,
                ReceiveFriendsDataFromServer.friendsList[i].Experience,
                ReceiveFriendsDataFromServer.friendsList[i].TotalMoney,
                ReceiveFriendsDataFromServer.friendsImagesList[i],
                ReceiveFriendsDataFromServer.friendsList[i].IsOnline,
                ReceiveFriendsDataFromServer.friendsList[i].Id
                );
        }
    }

    private void ShowRequestsPanel()
    {
        windowType = WindowType.RequestsWindow;
        CleanContent();
        if (ReceiveFriendsDataFromServer.friendRequestsList.Count != 0)
        {
            _requests.text = SettingsManager.Instance.GetString("FriendsPanel.Requests") + " " + ReceiveFriendsDataFromServer.friendRequestsList.Count;
            _requestsOutline.text = SettingsManager.Instance.GetString("FriendsPanel.Requests") + " " + ReceiveFriendsDataFromServer.friendRequestsList.Count;
        }
        else
        {
            _requests.text = SettingsManager.Instance.GetString("FriendsPanel.Requests");
            _requestsOutline.text = SettingsManager.Instance.GetString("FriendsPanel.Requests");
        }
        for (int i = 0; i < ReceiveFriendsDataFromServer.friendRequestsList.Count; i++)
        {
            var requestPanel = Instantiate(_friendRequests, _content);
            var requestComponent = requestPanel.GetComponent<RequestView>();
            requestComponent.SetRequestData
                (
                ReceiveFriendsDataFromServer.friendRequestsList[i].UserName,
                ReceiveFriendsDataFromServer.friendRequestsList[i].Experience,
                ReceiveFriendsDataFromServer.friendRequestsList[i].TotalMoney,
                ReceiveFriendsDataFromServer.friendRequestsImagesList[i],
                ReceiveFriendsDataFromServer.friendRequestsList[i].IsOnline,
                ReceiveFriendsDataFromServer.friendRequestsList[i].Id
                );
        }
    }

    private void UpdateFriendsPanelView()
    {
        if (windowType.Equals(WindowType.FriendsWindow))
            ShowFriendsPanel();
        else
            ShowRequestsPanel();
    }

    private void CleanContent()
    {
        for (int i = 0; i < _content.childCount; i++)
            Destroy(_content.GetChild(i).gameObject);
    }

    private void AddToStaticDictionary()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var panel = transform.GetChild(i).gameObject;
            StaticRuntimeSets.Add(panel.name, panel);
        }
    }

    IEnumerator CheckOnlineStatus()
    {
        while (true)
        {
            ReceiveFriendsDataFromServer.CheckOnline();
            yield return new WaitForSeconds(3f);
        }
    }

    public void ClosePanel() => Destroy(gameObject);
}

enum WindowType
{
    FriendsWindow,
    RequestsWindow
}

