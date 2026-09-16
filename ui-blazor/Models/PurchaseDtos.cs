namespace ui_blazor.Models;

public sealed record CreatePurchaseRequest(string Description, DateOnly TransactionDate, decimal Amount);

public sealed record PurchaseResponse(Guid Id, string Description, DateOnly TransactionDate, decimal Amount);

public sealed record PurchaseConversionResponse(
    Guid Id,
    string Description,
    DateOnly TransactionDate,
    decimal OriginalAmountUsd,
    decimal ExchangeRate,
    decimal ConvertedAmount,
    string Country,
    string Currency,
    DateOnly RateDate);
