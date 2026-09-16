using System.ComponentModel.DataAnnotations;

namespace ui_blazor.Models;

public sealed class PurchaseFormModel
{
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(50, ErrorMessage = "Description must be 50 characters or fewer.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Transaction date is required.")]
    public DateOnly? TransactionDate { get; set; }

    [Required(ErrorMessage = "Purchase amount is required.")]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "Purchase amount must be a positive amount.")]
    public decimal? Amount { get; set; }
}
