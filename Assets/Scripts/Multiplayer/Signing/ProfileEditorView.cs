using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEditorView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headerText;
    [Header("Name"), SerializeField] TextMeshProUGUI inputNameHeaderText;
    [SerializeField] TextMeshProUGUI inputFieldNameInnerText;
    [SerializeField] TMP_InputField inputFieldName;
    [Header("Hands"), SerializeField] TextMeshProUGUI chooseHandsText;
    [SerializeField] Color defaultColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Image[] handButtonsBackground;
    [SerializeField] Image[] handImages;
    [SerializeField] Sprite[] maleSprites;
    [SerializeField] Sprite[] femaleSprites;
    [Header("Providers buttons"), SerializeField] Button googleButton;
    [SerializeField] Button facebookButton;
    [SerializeField] TextMeshProUGUI approveButtonText;
    public event Action<string, Gender, HandsSpriteType> OnContinueButtonClick;
    public event Action<ExternalProviderName> OnProviderButtonClick;
    public event Action OnCloseButtonClick;
    string username = string.Empty;
    Gender gender = GameConstants.DefaultGender;
    HandsSpriteType handsType = GameConstants.DefaultHandsSpriteType;
    ExternalProviderName externalLogin;
    int currentIndex = 0;

    private void Awake()
    {
        UpdateExternalLoginsStates();
        SetTexts();
    }

    private void OnEnable()
    {
        if (PlayerProfileData.Instance.ExternalLogin == ExternalProviderName.None)
        {
            googleButton.gameObject.SetActive(true);
            facebookButton.gameObject.SetActive(true);
        }
        else
        {
            googleButton.gameObject.SetActive(false);
            facebookButton.gameObject.SetActive(false);
        }
    }

    private void SetTexts()
    {
        inputFieldName.text = PlayerPrefs.GetString("Nickname", "");
        headerText.text = SettingsManager.Instance.GetString("ProfileEditing.Profile");
        inputNameHeaderText.text = SettingsManager.Instance.GetString("ProfileEditing.Name");
        chooseHandsText.text = SettingsManager.Instance.GetString("ProfileEditing.Hands");
        approveButtonText.text = SettingsManager.Instance.GetString("ProfileEditing.Apply");
    }

    public void SetData(string nickname, Gender gender, HandsSpriteType handsType, ExternalProviderName externalProviderName)
    {
        username = nickname;
        this.gender = gender;
        this.handsType = handsType;
        inputFieldNameInnerText.text = username;
        switch((gender, handsType))
        {
            case (Gender.Male, HandsSpriteType.WhiteMan):
                OnHandButtonClick(0);
                break;
            case (Gender.Male, HandsSpriteType.BlackMan):
                OnHandButtonClick(1);
                break;
            case (Gender.Female, HandsSpriteType.WhiteWoman):
                OnHandButtonClick(2);
                break;
            case (Gender.Female, HandsSpriteType.BlackWoman):
                OnHandButtonClick(3);
                break;
        }
        UpdateExternalLoginsStates();
    }

    void UpdateExternalLoginsStates()
    {
        switch (PlayerProfileData.Instance.ExternalLogin)
        {
            case ExternalProviderName.None:
                googleButton.interactable = true;
                facebookButton.interactable = true;
                break;
            case ExternalProviderName.Google:
                googleButton.interactable = false;
                facebookButton.interactable = false;
                break;
            case ExternalProviderName.Facebook:
                googleButton.interactable = false;
                facebookButton.interactable = false;
                break;
        }
    }

    public void OnGoogleButtonClick() => OnProviderButtonClick?.Invoke(ExternalProviderName.Google);

    public void OnFacebookButtonClick() => OnProviderButtonClick?.Invoke(ExternalProviderName.Facebook);

    public void OnInputFieldNameValueChanged(string value)
    {
        inputFieldNameInnerText.text = value;
        username = value;
    }

    public void OnHandButtonClick(int index)
    {
        handButtonsBackground[currentIndex].color = defaultColor;
        switch (index)
        {
            case 0:
                gender = Gender.Male;
                handsType = HandsSpriteType.WhiteMan;
                currentIndex = 0;
                break;
            case 1:
                gender = Gender.Male;
                handsType = HandsSpriteType.BlackMan;
                currentIndex = 1;
                break;
            case 2:
                gender = Gender.Female;
                handsType = HandsSpriteType.WhiteWoman;
                currentIndex = 2;
                break;
            case 3:
                gender = Gender.Female;
                handsType = HandsSpriteType.BlackWoman;
                currentIndex = 3;
                break;
        }
        handButtonsBackground[currentIndex].color = selectedColor;
    }

    public void OnConfirmButtonClickHandler()
    {
        OnContinueButtonClick?.Invoke(username, gender, handsType);
    }

    public void OnCloseButtonClickHandler() => OnCloseButtonClick?.Invoke();
}