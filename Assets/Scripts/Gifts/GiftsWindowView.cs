using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftsWindowView : MonoBehaviour
{
    [SerializeField] Button playerButton;
    [SerializeField] Button tableButton;
    [SerializeField] TextMeshProUGUI headerLabel;
    [SerializeField] TextMeshProUGUI tableButtonLabel;
    [SerializeField] TextMeshProUGUI playerButtonLabel;
    [Space, SerializeField] GameObject giftButtonPrefab;
    [SerializeField] Transform parentTransform;
    public event Action<int> OnGiftButtonClick;
    public event Action OnTableButtonClick;
    public event Action OnPlayerButtonClick;
    public event Action OnCloseButtonClick;
    GiftButton[] giftButtons;
    string tableString = string.Empty;
    string playerString = string.Empty;

    public void SetGiftInfo(int index, int cost, Sprite sprite)
    {
        var button = giftButtons[index].GetComponent<Button>();
        button.onClick.AddListener(() => OnGiftClick(index));
        giftButtons[index].Init(cost, sprite);
    }

    public void OnTableButtonClickHandler() => OnTableButtonClick?.Invoke();

    public void OnPlayerButtonClickHandler() => OnPlayerButtonClick?.Invoke();

    public void OnCloseButtonClickHandler() => OnCloseButtonClick?.Invoke();

    public void CreateGiftButtons(int amount)
    {
        giftButtons = new GiftButton[amount];
        for (var i = 0; i < giftButtons.Length; i++)
        {
            var obj = Instantiate(giftButtonPrefab, parentTransform);
            giftButtons[i] = obj.GetComponent<GiftButton>();
        }
    }

    void OnGiftClick(int index) => OnGiftButtonClick?.Invoke(index);

    public void SetPlayerButtonState(bool state, int num = 0) 
    {
        playerButton.interactable = state;
        playerButtonLabel.text = num == 0 ? playerString : $"{playerString} {num.IntoCluttered()}";
    }

    public void SetTableButtonState(bool state, int num = 0)
    {
        tableButton.interactable = state;
        tableButtonLabel.text = num == 0 ? tableString : $"{tableString} {num.IntoCluttered()}";
    }

    public void SetTexts()
    {
        headerLabel.text = SettingsManager.Instance.GetString("Gifts.Header");
        tableString = SettingsManager.Instance.GetString("Gifts.Table");
        playerString = SettingsManager.Instance.GetString("Gifts.Player");
    }
}