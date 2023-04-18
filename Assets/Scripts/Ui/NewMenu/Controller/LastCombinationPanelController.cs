using System.Collections.Generic;
using UnityEngine;
using PokerHand.Common.Dto;
using PokerHand.Common.Entities;
using UnityEngine.UI;
using static Client;
using System.Linq;

public class LastCombinationPanelController : MonoBehaviour
{
    [SerializeField] private Button showLastCombinationButton;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private GameObject viewObjPrefab;
    [SerializeField] private WinnersDataContainer dataContainer;

    private void Awake()
    {
        if (GameModeSettings.Instance.gameMode.Equals(GameModes.Dash))
            showLastCombinationButton.gameObject.SetActive(false);
        OnReceiveWinningList += ReceiveWinningList;
        OnReceiveTableState += ReceiveTableState;
        showLastCombinationButton.onClick.AddListener(delegate { ShowLastCombination(); });
        AwakeClearCollections();
        OnReceiveShowWinnerCards += EnableToShowOnePlayer;
    }

    private void EnableToShowOnePlayer() => dataContainer.isAvaibleToShow = true;

    private void OnDestroy()
    {
        if (showLastCombinationButton != null)
            showLastCombinationButton.onClick.RemoveListener(delegate { ShowLastCombination(); });
        OnReceiveShowWinnerCards -= EnableToShowOnePlayer;
        OnReceiveTableState -= ReceiveTableState;
    }


    private void ReceiveWinningList(List<PlayerDto> winners, List<SidePotDto> pots)
    {
        dataContainer.winningAmountPerPlayerDictionary.Clear();
        var mainPot = pots.First(t => t.Type == PokerHand.Common.Helpers.SidePotType.Main);
        foreach (var winner in mainPot.Winners)
        {
            foreach (var pot in pots)
            {
                if (pot.Winners.Any(t => t.Id == winner.Id))
                {
                    if (dataContainer.winningAmountPerPlayerDictionary.ContainsKey(winner.Id))
                        dataContainer.winningAmountPerPlayerDictionary[winner.Id] += pot.WinningAmountPerPlayer;
                    else
                        dataContainer.winningAmountPerPlayerDictionary.Add(winner.Id, pot.WinningAmountPerPlayer);
                }
            }
        }
    }

    private void ReceiveTableState(TableDto tableDto)
    {
        switch(tableDto.CurrentStage)
        {
            case PokerHand.Common.Helpers.RoundStageType.MakeBlindBets:
                dataContainer.winningAmountPerPlayerDictionary.Clear();
                dataContainer.cardsOnTableList.Clear();
                dataContainer.activePlayersList.Clear();
                foreach (PlayerDto player in tableDto.Players)
                {
                    ActivePlayer activePlayers = new ActivePlayer()
                    {
                        Id = player.Id,
                        userName = player.UserName,
                        playerCardList = new List<Sprite>(),
                        playerWinningCombinationList = new List<Sprite>(),
                        combinationName = "",
                        winningAmountPerPlayer = ""
                    };
                    dataContainer.activePlayersList.Add(activePlayers);
                }
                break;
            case PokerHand.Common.Helpers.RoundStageType.Showdown:
                if (dataContainer.activePlayersList.Count == 0)
                    return;
                dataContainer.cardsOnTableList.Clear();
                foreach (CardDto card in tableDto.CommunityCards)
                    dataContainer.cardsOnTableList.Add(dataContainer.GetCardSprite(card));
                for (int i = 0; i < dataContainer.activePlayersList.Count; ++i)
                {
                    ActivePlayer activePlayer = dataContainer.activePlayersList[i];
                    if (tableDto.ActivePlayers.Any(t => t.Id == activePlayer.Id))
                    {
                        PlayerDto playerDto = tableDto.ActivePlayers.First(t => t.Id == activePlayer.Id);
                        if (dataContainer.winningAmountPerPlayerDictionary.ContainsKey(activePlayer.Id))
                            activePlayer.winningAmountPerPlayer = "$" + dataContainer.winningAmountPerPlayerDictionary[activePlayer.Id].IntoDivided();
                        else
                            activePlayer.winningAmountPerPlayer = "";
                        activePlayer.combinationName = playerDto.Hand.ToString();
                        foreach (CardDto card in playerDto.PocketCards)
                            activePlayer.playerCardList.Add(dataContainer.GetCardSprite(card));
                        foreach (CardDto card in playerDto.HandCombinationCards)
                            activePlayer.playerWinningCombinationList.Add(dataContainer.GetCardSprite(card));
                    }
                }
                dataContainer.isAvaibleToShow = tableDto.ActivePlayers.Count > 1;
                dataContainer.lastCombinationActivePlayersList.Clear();
                dataContainer.lastCombinationActivePlayersList.AddRange(dataContainer.activePlayersList);
                dataContainer.lastCardsOnTableList.Clear();
                dataContainer.lastCardsOnTableList.AddRange(dataContainer.cardsOnTableList);
                if (showLastCombinationButton != null)
                    showLastCombinationButton.interactable = true;
                break;
        }
    }

    public void ShowLastCombination() => Instantiate(viewObjPrefab, parentTransform);

    private void AwakeClearCollections()
    {
        dataContainer.activePlayersList.Clear();
        dataContainer.cardsOnTableList.Clear();
        dataContainer.lastCardsOnTableList.Clear();
        dataContainer.lastCombinationActivePlayersList.Clear();
    }
}