public class MoneyBox
{
    public int MaxChips { get; } = 2_000_000;
    public float PercentForBet {  get; } = 0.1f;
    public int Chips { set; get; } = 0;

    public void IncreaseChips(int value)
    {
        Chips += value;
        if (Chips > MaxChips)
            Chips = MaxChips;
    }
}