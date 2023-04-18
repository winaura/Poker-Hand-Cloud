using System;

namespace PokerHand.Common.Helpers.Friends
{
    [Serializable]
    public class FriendRequestModel
    {
        public Guid requestReceiverId { get; set; }
        public Guid requestSenderId { get; set; }
    }
}