using System.ComponentModel.DataAnnotations;

namespace api.Dtos;

public record CreatePurchaseRequest
{
    [Required, StringLength(50)]
    public required string Description { get; init; }

    [Required]
    public required DateOnly TransactionDate { get; init; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public required decimal Amount { get; init; }
}
