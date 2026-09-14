using api.Services;

namespace api.Tests;

public class TreasuryExchangeRateServiceTests
{
    private static TreasuryExchangeRateService CreateService(string jsonResponse, out FakeHttpMessageHandler handler)
    {
        handler = new FakeHttpMessageHandler(jsonResponse);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/services/api/fiscal_service/"),
        };
        return new TreasuryExchangeRateService(httpClient);
    }

    private static string RatesJson(params (string Country, string Currency, string RecordDate, string Rate)[] rows)
    {
        var items = rows.Select(r =>
            $$"""
            {"country":"{{r.Country}}","currency":"{{r.Currency}}","record_date":"{{r.RecordDate}}","exchange_rate":"{{r.Rate}}"}
            """);
        return $$"""{"data":[{{string.Join(",", items)}}]}""";
    }

    [Fact]
    public async Task GetRateAsync_RateWithinWindow_ReturnsFound()
    {
        var json = RatesJson(("Canada", "Dollar", "2026-03-31", "1.393"));
        var service = CreateService(json, out _);

        var result = await service.GetRateAsync("Canada-Dollar", new DateOnly(2026, 6, 15), CancellationToken.None);

        var found = Assert.IsType<ExchangeRateFound>(result);
        Assert.Equal(1.393m, found.Rate);
        Assert.Equal(new DateOnly(2026, 3, 31), found.RateDate);
        Assert.Equal("Canada", found.Country);
        Assert.Equal("Dollar", found.Currency);
    }

    [Fact]
    public async Task GetRateAsync_RateExactlySixMonthsBefore_IsIncludedByFilter()
    {
        var purchaseDate = new DateOnly(2026, 6, 15);
        var boundaryDate = purchaseDate.AddMonths(-6);
        var json = RatesJson(("Canada", "Dollar", boundaryDate.ToString("yyyy-MM-dd"), "1.30"));
        var service = CreateService(json, out var handler);

        var result = await service.GetRateAsync("Canada-Dollar", purchaseDate, CancellationToken.None);

        Assert.IsType<ExchangeRateFound>(result);
        Assert.Contains($"record_date:gte:{boundaryDate:yyyy-MM-dd}", handler.LastRequest!.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task GetRateAsync_NoRowsReturned_ReturnsNotFound()
    {
        var service = CreateService("""{"data":[]}""", out _);

        var result = await service.GetRateAsync("Canada-Dollar", new DateOnly(2026, 6, 15), CancellationToken.None);

        Assert.IsType<ExchangeRateNotFound>(result);
    }

    [Fact]
    public async Task GetRateAsync_BuildsExpectedFilterQuery()
    {
        var service = CreateService("""{"data":[]}""", out var handler);

        await service.GetRateAsync("Canada-Dollar", new DateOnly(2026, 6, 15), CancellationToken.None);

        var uri = handler.LastRequest!.RequestUri!.AbsoluteUri;
        Assert.Contains("country_currency_desc:eq:Canada-Dollar", uri);
        Assert.Contains("record_date:lte:2026-06-15", uri);
        Assert.Contains("record_date:gte:2025-12-15", uri);
        Assert.Contains("sort=-record_date", uri);
    }

    [Fact]
    public async Task GetRateAsync_EscapesSpacesInCountryCurrencyDesc()
    {
        var service = CreateService("""{"data":[]}""", out var handler);

        await service.GetRateAsync("United Kingdom-Pound Sterling", new DateOnly(2026, 6, 15), CancellationToken.None);

        var uri = handler.LastRequest!.RequestUri!.AbsoluteUri;
        Assert.Contains("country_currency_desc:eq:United%20Kingdom-Pound%20Sterling", uri);
    }
}
