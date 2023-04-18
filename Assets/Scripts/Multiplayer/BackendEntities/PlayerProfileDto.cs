using System;
using System.Collections.Generic;
using PokerHand.Common.Entities;
using PokerHand.Common.Helpers;
using PokerHand.Common.Helpers.Authorization;
using PokerHand.Common.Helpers.CardEvaluation;
using PokerHand.Common.Helpers.Player;

namespace PokerHand.Common.Dto
{
    public class PlayerProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Gender Gender { get; set; }
        public CountryCode Country { get; set; }
        public DateTime RegistrationDate { get; set; }
        public HandsSpriteType HandsSprite { get; set; }
        public long TotalMoney { get; set; }
        public int MoneyBoxAmount { get; set; }
        public int Experience { get; set; }
        public int GamesPlayed { get; set; }
        public HandType BestHandType { get; set; }
        public int GamesWon { get; set; }
        public long BiggestWin { get; set; }
        public int SitAndGoWins { get; set; }
        public string PersonalCode { get; set; }
        public ExternalProviderName ProviderName { get; set; }
    }
}