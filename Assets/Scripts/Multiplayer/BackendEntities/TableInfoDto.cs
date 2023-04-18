﻿namespace PokerHand.Common.Dto
{
    [System.Serializable]
    public class TableInfoDto
    {
        public TablesInWorlds Title { get; set; }
        public int TableType { get; set; }
        public int Experience { get; set; }
        public int SmallBlind { get; set; }
        public int BigBlind { get; set; }
        public int MinBuyIn { get; set; }
        public int MaxBuyIn { get; set; }
        public int MaxPlayers { get; set; }
        public int InitialStack { get; set; }
        public int FirstPlacePrize { get; set; }
        public int SecondPlacePrize { get; set; }
    }
}