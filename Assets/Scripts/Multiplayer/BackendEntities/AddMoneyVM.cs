using System;

namespace PokerHand.Common.ViewModels.Profile
{
    public class AddMoneyVM
    {
        public Guid PlayerId { get; set; }
        public long Amount { get; set; }
    }
}