using System;

public static class IntReductionParcer
{
    public static string IntoCluttered(this int value)
    {
        if (value < 10000)
            return value.ToString();
        string[] reductionStrings = new string[] { "B", "M", "K"};
        float[] reductionValues = new float[] { 1_000_000_000, 1_000_000, 1_000};
        for(int i = 0; i < 3; ++i)
        {
            float reduction = value / reductionValues[i];
            if (reduction >= 1) 
                return Math.Round(reduction, 1) + reductionStrings[i];
        }
        return value.ToString();
    }

    public static string IntoCluttered(this long value)
    {
        if (value < 10000)
            return value.ToString();
        string[] reductionStrings = new string[] { "B", "M", "K" };
        float[] reductionValues = new float[] { 1_000_000_000, 1_000_000, 1_000 };
        for (int i = 0; i < 3; ++i)
        {
            float reduction = value / reductionValues[i];
            if (reduction >= 1)
                return Math.Round(reduction, 1) + reductionStrings[i];
        }
        return value.ToString();
    }

    public static string IntoClutteredForTablePrize(this int value)
    {
        if (value < 10000)
        {
            int thousand = value / 1000;
            int hundreds = value % 1000;
            if (thousand == 0)
                return hundreds.ToString();
            else
                return $"{thousand:d1} {hundreds:d3}";
        }
        string[] reductionStrings = new string[] { "B", "M", "K" };
        float[] reductionValues = new float[] { 1_000_000_000, 1_000_000, 1_000 };
        for (int i = 0; i < 3; ++i)
        {
            float reduction = value / reductionValues[i];
            if (reduction >= 1)
                return Math.Round(reduction, 1) + reductionStrings[i];
        }
        return value.ToString();
    }

    public static string IntoClutteredForTablePrize(this long value)
    {
        if (value < 10000)
        {
            long thousand = value / 1000;
            long hundreds = value % 1000;
            if (thousand == 0)
                return hundreds.ToString();
            else
                return $"{thousand:d1} {hundreds:d3}";
        }
        string[] reductionStrings = new string[] { "B", "M", "K" };
        float[] reductionValues = new float[] { 1_000_000_000, 1_000_000, 1_000 };
        for (int i = 0; i < 3; ++i)
        {
            float reduction = value / reductionValues[i];
            if (reduction >= 1)
                return Math.Round(reduction, 1) + reductionStrings[i];
        }
        return value.ToString();
    }

    public static string IntoDivided(this int value)
    {
        string valueStr = value.ToString();
        for (int i = valueStr.Length - 3; i > 0; i-=3)
            valueStr = valueStr.Insert(i, " ");
        return valueStr;
    }

    public static string IntoDivided(this long value)
    {
        string valueStr = value.ToString();
        for (int i = valueStr.Length - 3; i > 0; i -= 3)
            valueStr = valueStr.Insert(i, " ");
        return valueStr;
    }
}