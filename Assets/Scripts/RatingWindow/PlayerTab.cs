using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ReceiveFriendsDataFromServer;
using PokerHand.Common.Dto;

public class PlayerTab : MonoBehaviour
{
    [SerializeField] private Image liderNumberImage;
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_Text prizeNumberText;
    [SerializeField] private Sprite[] liderNumberSprites;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private RawImage avatarImage;
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private Image countryImage;
    [SerializeField] private TMP_Text countryText;
    [SerializeField] private TMP_Text ratingAmountText;

    private PlayerWithRankDto playerRank;
    private PlayerProfileWindowController playerProfileWindowController;

    private void Start()
    {
        SettingsManager.Instance.UpdateTextsEvent += SetTabTexts;
    }

    private void OnDestroy()
    {
        SettingsManager.Instance.UpdateTextsEvent -= SetTabTexts;
    }

    public void SetPlayerRank(PlayerWithRankDto playerRank)
    {
        this.playerRank = playerRank;
        SetTabTexts();
        if (playerRank.Id == PlayerProfileData.Instance.Id)
            playerProfileWindowController = FindObjectOfType<PlayerProfileWindowController>();
    }

    private void SetTabTexts()
    {
        if (playerRank.Rank < 0)
        {
            liderNumberImage.gameObject.SetActive(false);
            numberText.gameObject.SetActive(true);
            numberText.fontSize = 33;
            numberText.alignment = TextAlignmentOptions.Center;
            numberText.text = string.Format(SettingsManager.Instance.GetString("RatingWindow.PercentAmount"), -playerRank.Rank);
        }
        else
        {
            if (playerRank.Rank <= 3)
            {
                liderNumberImage.gameObject.SetActive(true);
                numberText.gameObject.SetActive(false);
                prizeNumberText.text = playerRank.Rank.ToString();
                liderNumberImage.sprite = liderNumberSprites[playerRank.Rank - 1];
            }
            else
            {
                liderNumberImage.gameObject.SetActive(false);
                numberText.gameObject.SetActive(true);
                numberText.alignment = TextAlignmentOptions.Left;
                numberText.fontSize = 70;
                numberText.text = playerRank.Rank.ToString();
            }
        }
        levelText.text = SettingsManager.Instance.GetString("PlayerPanel.Level") + " " + new LevelCounter(playerRank.Experience).Level;
        avatarImage.texture = ParseStringToTexture(playerRank.BinaryImage);
        nicknameText.text = playerRank.UserName;
        countryImage.sprite = playerRank.Country.LoadCountryFlagSprite();
        countryText.text = playerRank.Country.LoadCountryCodeString();
        ratingAmountText.text = playerRank.TotalMoney.IntoDivided();
    }

    public void OpenPlayerProfile()
    {
        if (playerRank.Id == PlayerProfileData.Instance.Id)
        {
            playerProfileWindowController.OpenWindow();
        }
        else
        {
            AnotherPlayerProfileController.Instance.SetPickedTexture(avatarImage.texture);
            Client.HTTP_SendGetPlayerProfile(playerRank.Id);
        }
    }
}
