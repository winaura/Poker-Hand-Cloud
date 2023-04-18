using PokerHand.Common.Helpers;
using System.Collections.Generic;

namespace PokerHand.Common.Dto
{
    public class SidePotDto
    {
        public SidePotType Type { get; set; }
        public List<PlayerDto> Winners { get; set; }
        public long WinningAmountPerPlayer { get; set; }
        public List<PlayerDto> Players { get; set; }
    }
}