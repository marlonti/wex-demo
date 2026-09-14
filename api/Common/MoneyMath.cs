namespace api.Common;

public static class MoneyMath
{
    public static decimal RoundToCents(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
