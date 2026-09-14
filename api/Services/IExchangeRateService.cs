namespace api.Services;

public interface IExchangeRateService
{
    /// <param name="countryCurrencyDesc">
    /// The country-currency pair, in the Treasury API's own "Country-Currency" format
    /// (i.e. its country_currency_desc field, e.g. "Canada-Dollar").
    /// </param>
    Task<ExchangeRateLookupResult> GetRateAsync(
        string countryCurrencyDesc,
        DateOnly purchaseDate,
        CancellationToken cancellationToken);
}

public abstract record ExchangeRateLookupResult;

public sealed record ExchangeRateFound(decimal Rate, DateOnly RateDate, string Country, string Currency)
    : ExchangeRateLookupResult;

public sealed record ExchangeRateNotFound : ExchangeRateLookupResult;
