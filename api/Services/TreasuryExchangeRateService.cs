using System.Globalization;
using System.Text.Json.Serialization;

namespace api.Services;

/// <summary>
/// Looks up exchange rates from the Treasury Reporting Rates of Exchange API, applying the
/// "most recent rate on or before the purchase date, within the last 6 months" rule.
/// Filters/sorts on record_date (the field used in the API's own documented example query).
/// </summary>
public class TreasuryExchangeRateService(HttpClient httpClient) : IExchangeRateService
{
    public async Task<ExchangeRateLookupResult> GetRateAsync(
        string countryCurrencyDesc,
        DateOnly purchaseDate,
        CancellationToken cancellationToken)
    {
        var lowerBound = purchaseDate.AddMonths(-6);

        var filter =
            $"country_currency_desc:eq:{Uri.EscapeDataString(countryCurrencyDesc)}," +
            $"record_date:lte:{purchaseDate:yyyy-MM-dd}," +
            $"record_date:gte:{lowerBound:yyyy-MM-dd}";

        var requestUri =
            $"v1/accounting/od/rates_of_exchange" +
            $"?fields=country,currency,record_date,exchange_rate" +
            $"&filter={filter}" +
            $"&sort=-record_date" +
            $"&page[size]=1";

        var response = await httpClient.GetFromJsonAsync<TreasuryRatesResponse>(requestUri, cancellationToken);
        var records = response?.Data ?? [];

        if (records.Count == 0)
        {
            return new ExchangeRateNotFound();
        }

        var record = records[0];
        return new ExchangeRateFound(
            decimal.Parse(record.ExchangeRate, CultureInfo.InvariantCulture),
            DateOnly.Parse(record.RecordDate, CultureInfo.InvariantCulture),
            record.Country,
            record.Currency);
    }

    private sealed record TreasuryRatesResponse(
        [property: JsonPropertyName("data")] List<TreasuryRateRecord> Data);

    private sealed record TreasuryRateRecord(
        [property: JsonPropertyName("country")] string Country,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("record_date")] string RecordDate,
        [property: JsonPropertyName("exchange_rate")] string ExchangeRate);
}
