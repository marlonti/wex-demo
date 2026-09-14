using api.Services;

namespace api.Tests;

internal sealed class FakeExchangeRateService(ExchangeRateLookupResult result) : IExchangeRateService
{
    public Task<ExchangeRateLookupResult> GetRateAsync(
        string countryCurrencyDesc,
        DateOnly purchaseDate,
        CancellationToken cancellationToken) => Task.FromResult(result);
}
