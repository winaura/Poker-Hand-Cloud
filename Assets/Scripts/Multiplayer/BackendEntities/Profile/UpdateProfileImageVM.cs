using System;

namespace PokerHand.Common.ViewModels.Media
{
    public class UpdateProfileImageVM
    {
        public Guid PlayerId { get; set; }
        public string NewProfileImage { get; set; }
    }
}