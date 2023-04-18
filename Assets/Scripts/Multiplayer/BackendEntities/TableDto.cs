using System;
using System.Collections.Generic;
using PokerHand.Common.Dto;
using PokerHand.Common.Helpers;

namespace PokerHand.Common.Entities
{
    [Serializable]
    public class TableDto
    {
        public Guid Id { get; set; }
        public int SmallBlind { get; set; }
        public int BigBlind { get; set; }
        public RoundStageType CurrentStage { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<PlayerDto> ActivePlayers { get; set; }
        public List<CardDto> CommunityCards { get; set; }
        public PlayerDto CurrentPlayer { get; set; }       
        public long CurrentMaxBet { get; set; }
        public Pot Pot { get; set; }
        public int DealerIndex { get; set; }
        public int SmallBlindIndex { get; set; }
        public int BigBlindIndex { get; set; }
        public List<PlayerDto> Winners { get; set; }
        public DateTime SitAndGoTimeout { get; set; }
    }
}