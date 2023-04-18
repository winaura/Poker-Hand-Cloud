using PokerHand.Common.Helpers.Player;
using System;

namespace PokerHand.Common.Dto
{
    public class PlayerWithRankDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public CountryCode Country { get; set; }
        public int Experience { get; set; }
        public long TotalMoney { get; set; }
        public int Rank { get; set; }
        public string BinaryImage { get; set; }
    }
}