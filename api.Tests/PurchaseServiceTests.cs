using api.Data;
using api.Dtos;
using api.Services;
using Microsoft.EntityFrameworkCore;

namespace api.Tests;

public class PurchaseServiceTests
{
    private static PurchaseDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<PurchaseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PurchaseDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_RoundsAmountAndPersists()
    {
        await using var db = CreateDbContext();
        var service = new PurchaseService(db, new FakeExchangeRateService(new ExchangeRateNotFound()));

        var request = new CreatePurchaseRequest
        {
            Description = "Office chair",
            TransactionDate = new DateOnly(2026, 6, 15),
            Amount = 123.456m,
        };

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal(123.46m, response.Amount);
        Assert.NotEqual(Guid.Empty, response.Id);

        var stored = await db.Purchases.SingleAsync();
        Assert.Equal(response.Id, stored.Id);
        Assert.Equal(123.46m, stored.Amount);
    }

    [Fact]
    public async Task GetConvertedAsync_HappyPath_ComputesConvertedAmountRoundedToCents()
    {
        await using var db = CreateDbContext();
        var rate = new ExchangeRateFound(1.393m, new DateOnly(2026, 3, 31), "Canada", "Dollar");
        var service = new PurchaseService(db, new FakeExchangeRateService(rate));

        var created = await service.CreateAsync(new CreatePurchaseRequest
        {
            Description = "Office chair",
            TransactionDate = new DateOnly(2026, 6, 15),
            Amount = 100m,
        }, CancellationToken.None);

        var result = await service.GetConvertedAsync(created.Id, "Canada-Dollar", CancellationToken.None);

        var converted = Assert.IsType<PurchaseConverted>(result);
        Assert.Equal(139.30m, converted.Response.ConvertedAmount);
        Assert.Equal(1.393m, converted.Response.ExchangeRate);
        Assert.Equal(100m, converted.Response.OriginalAmountUsd);
    }

    [Fact]
    public async Task GetConvertedAsync_UnknownId_ReturnsNotFound()
    {
        await using var db = CreateDbContext();
        var service = new PurchaseService(db, new FakeExchangeRateService(new ExchangeRateNotFound()));

        var result = await service.GetConvertedAsync(Guid.NewGuid(), "Canada-Dollar", CancellationToken.None);

        Assert.IsType<PurchaseNotFound>(result);
    }

    [Fact]
    public async Task GetConvertedAsync_NoRateAvailable_ReturnsUnavailable()
    {
        await using var db = CreateDbContext();
        var service = new PurchaseService(db, new FakeExchangeRateService(new ExchangeRateNotFound()));

        var created = await service.CreateAsync(new CreatePurchaseRequest
        {
            Description = "Old thing",
            TransactionDate = new DateOnly(2010, 1, 1),
            Amount = 50m,
        }, CancellationToken.None);

        var result = await service.GetConvertedAsync(created.Id, "Narnia-Gold", CancellationToken.None);

        var unavailable = Assert.IsType<ConversionUnavailable>(result);
        Assert.Contains("Narnia-Gold", unavailable.Reason);
    }
}
