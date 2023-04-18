using System;

namespace PokerHand.Common.Helpers.Table
{
    public class TableConnectByIdOptions
    {
        public Guid TableId { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerConnectionId { get; set; }
        public long BuyInAmount { get; set; }
        public bool IsAutoTop { get; set; }
    }
}