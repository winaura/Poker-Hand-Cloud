using PokerHand.Common.Helpers.Present;

namespace PokerHand.Common.Dto
{
    [System.Serializable]
    public class PresentInfoDto
    {
        public PresentName Name { get; set; }
        public int Price { get; set; }
    }
}