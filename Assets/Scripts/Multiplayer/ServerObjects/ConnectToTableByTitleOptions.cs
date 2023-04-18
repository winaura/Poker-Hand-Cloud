using System;

namespace PokerHand.Common.Helpers.Table
{
    public class ConnectToTableByTitleOptions
    {
        public TableTitle TableTitle { get; set; }
        public Guid PlayerId { get; set; }
        public Guid? CurrentTableId { get; set; }
        public string PlayerConnectionId { get; set; }
        public long BuyInAmount { get; set; }
        public bool IsAutoTop { get; set; }
    }
}