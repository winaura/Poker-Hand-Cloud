using UnityEngine;
using UnityEngine.UI;

public class LastCombinationPanelView : MonoBehaviour
{
    [SerializeField] private WinnersDataContainer dataContainer;
    [SerializeField] private OnePlayerUIData onePlayerUIPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private Text tableCardsText;
    [SerializeField] private Text lastRoundResultText;
    [Header("Table Cards"), SerializeField] private Image[] tableCardsArray;

    public void CloseTab() => Destroy(gameObject);

    private void OnEnable()
    {
        tableCardsText.text = SettingsManager.Instance.GetString("LastCombination.TableCards");
        lastRoundResultText.text = SettingsManager.Instance.GetString("LastCombination.Distribution");
        for (int i = 0; i < dataContainer.lastCardsOnTableList.Count; ++i)
            tableCardsArray[i].sprite = dataContainer.lastCardsOnTableList[i];
        for (int i = 0; i < dataContainer.lastCombinationActivePlayersList.Count; i++)
        {
            ActivePlayer player = dataContainer.lastCombinationActivePlayersList[i];
            if (i > 2) 
                contentPanel.sizeDelta += new Vector2(0, 130);
            var onePlayerUI = Instantiate(onePlayerUIPrefab, contentPanel);
            if (!string.IsNullOrEmpty(player.winningAmountPerPlayer))
                onePlayerUI.playerGainText.text = player.winningAmountPerPlayer;
            onePlayerUI.userNameText.text = player.userName;
            onePlayerUI.serialNumber.text = (i + 1).ToString();
            if (dataContainer.isAvaibleToShow)
            {
                for (int j = 0; j < player.playerCardList.Count; ++j)
                    onePlayerUI.pocketCards[j].sprite = player.playerCardList[j];
                for (int j = 0; j < player.playerWinningCombinationList.Count; j++)
                    onePlayerUI.playerCombinationImages[j].sprite = player.playerWinningCombinationList[j];
                if (!string.IsNullOrEmpty(player.combinationName))
                    onePlayerUI.playerCombinationText.text = SettingsManager.Instance.GetString("Combinations." + player.combinationName);
            }
        }
    }
}
