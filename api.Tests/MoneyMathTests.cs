using api.Common;

namespace api.Tests;

public class MoneyMathTests
{
    [Theory]
    [InlineData(10.005, 10.01)]
    [InlineData(10.994, 10.99)]
    [InlineData(10.995, 11.00)]
    [InlineData(0.005, 0.01)]
    [InlineData(123.456, 123.46)]
    [InlineData(100, 100)]
    public void RoundToCents_RoundsHalfAwayFromZero(decimal input, decimal expected)
    {
        Assert.Equal(expected, MoneyMath.RoundToCents(input));
    }

    [Fact]
    public void RoundToCents_RoundsNegativeHalfAwayFromZero()
    {
        Assert.Equal(-10.01m, MoneyMath.RoundToCents(-10.005m));
    }
}
