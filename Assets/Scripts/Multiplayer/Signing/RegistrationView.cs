using PokerHand.Common.Helpers.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headerText;
    [Header("Name"), SerializeField] TextMeshProUGUI inputNameHeaderText;
    [SerializeField] TextMeshProUGUI inputFieldNamePlaceholderText;
    [SerializeField] TextMeshProUGUI inputFieldNameInnerText;
    [SerializeField] TMP_InputField inputFieldName;
    [Header("Hands"), SerializeField] TextMeshProUGUI chooseHandsText;
    [SerializeField] Color defaultColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Image[] handButtonsBackground;
    [SerializeField] Image[] handImages;
    [Header("Policy"), SerializeField] Text policyText;
    [SerializeField] Toggle policyAcceptToggle;
    [Header("Continue button"), SerializeField] Button continueButton;
    [SerializeField] TextMeshProUGUI continueText;
    public event Action<string, Gender, HandsSpriteType> OnContinueButtonClick;
    public event Action OnCloseRegistrationFormButtonClick;
    string username = string.Empty;
    Gender gender = GameConstants.DefaultGender;
    HandsSpriteType handsType = GameConstants.DefaultHandsSpriteType;
    int currentIndex = 0;

    private void Start()
    {
        OnHandButtonClick(0);
        UpdateContinueButtonAvailability();
        headerText.text = SettingsManager.Instance.GetString("Registration.Header");
        inputNameHeaderText.text = SettingsManager.Instance.GetString("Registration.InputName");
        inputFieldNamePlaceholderText.text = SettingsManager.Instance.GetString("Registration.NamePlaceholder");
        chooseHandsText.text = SettingsManager.Instance.GetString("Registration.ChooseHands");
        policyText.text = SettingsManager.Instance.GetString("Registration.Policy");
        continueText.text = SettingsManager.Instance.GetString("Registration.Continue");
    }

    public void OnInputFieldNameValueChanged(string value)
    {
        username = value;
        UpdateContinueButtonAvailability();
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

    public void OnPolicyToggleClick(bool newState) => UpdateContinueButtonAvailability();

    public void OnContinueButtonClickHandler()
    {
        OnContinueButtonClick?.Invoke(username, gender, handsType);
    }

    public void SetNameField(string name)
    {
        inputFieldNamePlaceholderText.text = string.Empty;
        inputFieldNameInnerText.text = name;
        OnInputFieldNameValueChanged(name);
        inputFieldName.text = name;
    }

    public void OnRegistrationFromCloseButtonClick() => OnCloseRegistrationFormButtonClick?.Invoke();

    void UpdateContinueButtonAvailability()
    {
        if (policyAcceptToggle.isOn && !string.IsNullOrEmpty(username))
            continueButton.interactable = true;
        else
            continueButton.interactable = false;
    }
}