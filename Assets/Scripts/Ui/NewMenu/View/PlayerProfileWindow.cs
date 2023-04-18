using PokerHand.Common.Helpers.Player;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileWindow : MonoBehaviour
{
    [SerializeField] private RawImage _avatarImage;
    [SerializeField] private Image _countryImage;
    [SerializeField] private Text _nickname;
    [SerializeField] private Button _closeProfileWindowButton;
    [SerializeField] private Button _editNicknameButton;
    [SerializeField] private Button _editImageButton;
    [SerializeField] private Slider _levelSlider;   
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _chipsText;
    [SerializeField] private Text _bitcoinsText;
    [SerializeField] private Text _firstLoginText;
    [SerializeField] private GameObject _countryEditor;
    [SerializeField] private Button _confirmCountryButton;
    [SerializeField] private Button _countryButton;
    [Header("Achievements"), SerializeField] private Text _handsPlayedCountText;
    [SerializeField] private Text _bestHandCombinationText;
    [SerializeField] private Text _handsWonCountText;
    [SerializeField] private Text _biggestPotCountText;
    [SerializeField] private Text _sitNGoTournamentsWonCountText;
    [Header("Profile texts"), SerializeField] private Text _handsPlayedText;
    [SerializeField] private Text _bestHandText;
    [SerializeField] private Text _handsWonText;
    [SerializeField] private Text _biggestPotText;
    [SerializeField] private Text _sitNGoTournamentsWonText;
    [Header("Country editor texts"), SerializeField] private Text _choseCountryText;
    [SerializeField] private Text _confirmCountryButtonText;
    [Header("Profile Editor"), SerializeField] Transform profileEditorParentTransform;
    [SerializeField] GameObject profileEditorPrefab;
    GameObject profileEditorObj = null;
    ProfileEditorController profileEditorController = null;
    public event Action OnEditImageButtonPressed;
    public event Action OnCloseButtonPressed;
    public event Action OnEditNicknameButtonPressed;
    public event Action OnConfirmNicknameButtonPressed;
    public event Action OnConfirmCountryButtonPressed;
    PlayerProfileData _profileData;

    private void Awake()
    {
        _profileData = PlayerProfileData.Instance;
        _closeProfileWindowButton.onClick.AddListener(() => OnCloseButtonPressed.Invoke());
        _editNicknameButton.onClick.AddListener(() => OnEditNicknameButtonPressed.Invoke());
        _editImageButton.onClick.AddListener(() => OnEditImageButtonPressed.Invoke());
        _confirmCountryButton.onClick.AddListener(() => OnConfirmCountryButtonPressed.Invoke());
        _profileData.OnProfileUpdated += UpdateProfileView;
        _profileData.OnMyTotalMoneyUpdated += UpdateTotalMoney;
        _profileData.OnExperienceUpdated += UpdateLevelData;
        Client.OnReceiveMyProfileImage += SetAvatarFromImage;
        SetAvatarFromImage();
    }

    private void Start()
    {
        SetWindowTexts();
        UpdateProfileView();
    }

    private void OnDestroy()
    {
        _profileData.OnExperienceUpdated -= UpdateLevelData;
        _profileData.OnProfileUpdated -= UpdateProfileView;
        _profileData.OnMyTotalMoneyUpdated -= UpdateTotalMoney;
        Client.OnReceiveMyProfileImage -= SetAvatarFromImage;
        if (profileEditorController != null)
        {
            profileEditorController.OnProfileEditingComplete -= ConfirmProfileEditing;
            profileEditorController.OnProfileEditingClose -= CloseProfileEditing;
        }
    }

    public void UpdateProfileView()
    {
        _nickname.text = _profileData.Nickname;
        _chipsText.text = _profileData.TotalMoney.IntoDivided();
        _firstLoginText.text = $"{SettingsManager.Instance.GetString("ProfileWindow.PlayerSince")}: {PlayerPrefs.GetString("FirstLogin", DateTime.Now.ToString("dd.MM.yyyy"))}";
        _handsPlayedCountText.text = _profileData.GamesPlayed.ToString();
        _handsWonCountText.text = _profileData.GamesWon.ToString();
        _sitNGoTournamentsWonCountText.text = _profileData.SitAndGoWins.ToString();
        _biggestPotCountText.text = _profileData.BiggestWin.IntoDivided();
        if (_profileData.BestHandType != PokerHand.Common.Helpers.HandType.None)
            _bestHandCombinationText.text = SettingsManager.Instance.GetString(_profileData.BestHandName);
        UpdateLevelData();
        SetCountryPicture(_profileData.countryPicturePath);
    }

    public void UpdateLevelData()
    {
        LevelCounter levelCounter = new LevelCounter(_profileData.Experience);
        _levelSlider.minValue = levelCounter.MinExperienceBorder;
        _levelSlider.maxValue = levelCounter.MaxExperienceBorder;
        _levelSlider.value = levelCounter.Experience;
        _levelText.text = $"{SettingsManager.Instance.GetString("ProfileWindow.Level")} {levelCounter.Level}";
    }

    public void OpenProfileEditor()
    {
        if (profileEditorObj == null)
        {
            profileEditorObj = Instantiate(profileEditorPrefab, profileEditorParentTransform);
            profileEditorController = profileEditorObj.GetComponent<ProfileEditorController>();
            profileEditorController.OnProfileEditingComplete += ConfirmProfileEditing;
            profileEditorController.OnProfileEditingClose += CloseProfileEditing;
        }
        profileEditorObj.SetActive(true);
        profileEditorController.SetInitialData(_profileData.Nickname, _profileData.Gender, _profileData.HandsSpriteType, _profileData.ExternalLogin);
    }

    public void ConfirmProfileEditing(string nickname, Gender gender, HandsSpriteType handsType)
    {
        _profileData.UpdateProfile(nickname, gender, handsType);
        UpdateProfileView();
        CloseProfileEditing();
    }

    public void ConfirmCountry()
    {
        var countryName = _countryButton.GetComponent<Image>().sprite.name;
        _profileData.SaveCountry(countryName);
        _countryEditor.SetActive(false);
        UpdateProfileView(); // remove it if we got a profile update after send update profile
    }

    public void SetAvatar()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                var bytes = File.ReadAllBytes(path);
                Client.HTTP_SendUpdateProfileImage(bytes);
            }
        }, SettingsManager.Instance.GetString("ProfileWindow.PickImage"), "image/png");
    }

    public void SetWindowTexts()
    {
        _handsPlayedText.text = SettingsManager.Instance.GetString("ProfileWindow.HandsPlayed");
        _bestHandText.text = SettingsManager.Instance.GetString("ProfileWindow.BestHand");
        _handsWonText.text = SettingsManager.Instance.GetString("ProfileWindow.HandsWon");
        _biggestPotText.text = SettingsManager.Instance.GetString("ProfileWindow.BiggestPot");
        _sitNGoTournamentsWonText.text = SettingsManager.Instance.GetString("ProfileWindow.SitNGoWon");
        _choseCountryText.text = SettingsManager.Instance.GetString("ProfileWindow.ChoseCountry");
        _confirmCountryButtonText.text = SettingsManager.Instance.GetString("ProfileWindow.Confirm");
    }

    private void CloseProfileEditing()
    {
        Destroy(profileEditorObj);
    }

    private void SetCountryPicture(string path) => _countryImage.sprite = Resources.Load<Sprite>(path);

    private void SetAvatarFromImage()
    {
        if (Client.ProfileImages != null)
            _avatarImage.texture = Client.ProfileImages[0];
    }

    private void UpdateTotalMoney() => _chipsText.text = _profileData.TotalMoney.IntoDivided();
}