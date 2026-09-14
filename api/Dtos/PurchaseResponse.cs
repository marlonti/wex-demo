namespace api.Dtos;

public record PurchaseResponse(Guid Id, string Description, DateOnly TransactionDate, decimal Amount);
