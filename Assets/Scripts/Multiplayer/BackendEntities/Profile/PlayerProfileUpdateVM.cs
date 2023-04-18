using System;

namespace PokerHand.Common.Helpers.Player
{
    [Serializable]
    public class PlayerProfileUpdateVM
    {
        public Guid Id { get; set; }
        public Gender Gender { get; set; }
        public CountryCode Country { get; set; }
        public HandsSpriteType HandsSprite { get; set; }
        public string UserName { get; set; }
    }
}