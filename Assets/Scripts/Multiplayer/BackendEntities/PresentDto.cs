using System;
using System.Collections.Generic;

namespace PokerHand.Common.Helpers.Present
{
    [System.Serializable]
    public class PresentDto
    {
        public PresentName Name { get; set; }
        public Guid SenderId { get; set; }
        public List<Guid> RecipientsIds { get; set; }
    }
}