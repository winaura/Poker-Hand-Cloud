using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.Player;
using PokerHand.Common.Helpers.Present;
using System;
using System.Collections.Generic;

namespace PokerHand.Common.Dto
{
    [Serializable]
    public class PlayerDto
    {
        public int LocalIndex => IndexNumber.ToLocalIndex();
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public HandsSpriteType HandsSprite { get; set; } = HandsSpriteType.WhiteMan;
        public List<CardDto> PocketCards { get; set; }
        public long CurrentBet { get; set; }
        public long StackMoney { get; set; }
        public long TotalMoney { get; set; }
        public int IndexNumber { get; set; }
        public HandType Hand { get; set; }
        public PresentName PresentName { get; set; }
        public List<CardDto> HandCombinationCards { get; set; }
        public long CurrentBuyIn { get; set; }
    }
}