using System;
using System.Collections.Generic;

namespace PokerHand.Common.Entities
{
    [Serializable]
    public class Pot
    {
        private TableDto Table { get; set; }
        public long TotalAmount { get; set; }
        public Dictionary<Guid, long> Bets { get; set; }
    }
}