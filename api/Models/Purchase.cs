namespace api.Models;

public class Purchase
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public decimal Amount { get; set; }
}
