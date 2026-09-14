using api.Dtos;

namespace api.Services;

public interface IPurchaseService
{
    Task<PurchaseResponse> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken);

    /// <param name="countryCurrencyDesc">
    /// The country-currency pair to convert to, in the Treasury API's own "Country-Currency"
    /// format (e.g. "Canada-Dollar").
    /// </param>
    Task<PurchaseOperationResult> GetConvertedAsync(
        Guid id,
        string countryCurrencyDesc,
        CancellationToken cancellationToken);
}

public abstract record PurchaseOperationResult;

public sealed record PurchaseConverted(PurchaseConversionResponse Response) : PurchaseOperationResult;

public sealed record PurchaseNotFound : PurchaseOperationResult;

public sealed record ConversionUnavailable(string Reason) : PurchaseOperationResult;
