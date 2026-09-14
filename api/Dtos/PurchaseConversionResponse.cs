namespace api.Dtos;

public record PurchaseConversionResponse(
    Guid Id,
    string Description,
    DateOnly TransactionDate,
    decimal OriginalAmountUsd,
    decimal ExchangeRate,
    decimal ConvertedAmount,
    string Country,
    string Currency,
    DateOnly RateDate);
