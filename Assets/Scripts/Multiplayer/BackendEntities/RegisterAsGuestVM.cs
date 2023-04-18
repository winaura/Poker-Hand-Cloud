using PokerHand.Common.Helpers.Player;

namespace PokerHand.Common.ViewModels.Profile
{
    public class RegisterAsGuestVM
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public Gender Gender { get; set; }
        public HandsSpriteType HandsSprite { get; set; }
    }
}