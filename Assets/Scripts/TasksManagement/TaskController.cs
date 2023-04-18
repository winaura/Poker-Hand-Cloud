using PokerHand.Common.Dto;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Client;

public class TaskController : MonoBehaviour
{
    PlayerActionType playerAction;
    HandType playerHandType;

    private void Awake()
    {
        OnReceiveWinningList += CheckWinDailyTasks;
        OnReceiveTableState += ReceiveTableState;
        OnReceivePlayerActionType += ReceivePlayerAction;
    }

    private void OnDestroy() => OnReceivePlayerActionType -= ReceivePlayerAction;

    // Метод для постоянного обновления комбинации на руках
    private void ReceiveTableState(TableDto tableDto)
    {
        for (int i = 0; i < tableDto.ActivePlayers.Count; i++)
            if (tableDto.ActivePlayers[i].LocalIndex == 0)
                playerHandType = tableDto.ActivePlayers[i].Hand;
    }

    //Отдельный метод для фолда c комбинацией на руках
    private void ReceivePlayerAction(PlayerActionType playerAction)
    {
        this.playerAction = playerAction;
        if (playerAction == PlayerActionType.Fold)
            TasksTracker.Instance.FoldWithCombination(playerHandType);
    }

    private void CheckWinDailyTasks(List<PlayerDto> Winners, List<SidePotDto> WinnersPots)
    {
        if (Winners.Any(t => t.Id == PlayerProfileData.Instance.Id))
        {
            var player = Winners.First(t => t.Id == PlayerProfileData.Instance.Id);
            PlayedAndWinRound(WinnersPots);
            TasksTracker.Instance.WinWithCombination(player.Hand);
            if (playerAction == PlayerActionType.AllIn)
                TasksTracker.Instance.WinAfterAllIn();
        }
        PlayedRound();
    }

    // Случай если игрок просто сыграл раунд
    private void PlayedRound()
    {
        var gameMode = GameModeSettings.Instance.gameMode;
        var currentTable = GameModeSettings.Instance.TablesInWorlds;
        TasksTracker.Instance.PlayHand();
        TasksTracker.Instance.PlayOnOneTable(currentTable);
        TasksTracker.Instance.PlayOnDifferentTables(currentTable);
        TasksTracker.Instance.PlayCertainMode(gameMode);
    }

    // Случай если игрок сыграл раунд и выиграл
    private void PlayedAndWinRound(List<SidePotDto> sidePotDtoList)
    {
        long totalAmountPerWinner = 0;
        for (int j = 0; j < sidePotDtoList.Count; j++)
            totalAmountPerWinner += sidePotDtoList[j].WinningAmountPerPlayer;
        TasksTracker.Instance.WinChips(totalAmountPerWinner);
        TasksTracker.Instance.WinHand();
    }
}
