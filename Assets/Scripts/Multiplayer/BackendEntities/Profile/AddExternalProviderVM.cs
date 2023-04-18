using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.Player;

namespace PokerHand.Common.ViewModels.Profile
{
    public class AddExternalProviderVM
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public Gender Gender { get; set; }
        public HandsSpriteType HandsSprite { get; set; }
        public ExternalProviderName ProviderName { get; set; }
        public string ProviderKey { get; set; }
    }
}