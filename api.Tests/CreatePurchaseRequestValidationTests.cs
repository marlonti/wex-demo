using System.ComponentModel.DataAnnotations;
using api.Dtos;

namespace api.Tests;

public class CreatePurchaseRequestValidationTests
{
    private static IList<ValidationResult> Validate(CreatePurchaseRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Validate_ValidRequest_HasNoErrors()
    {
        var request = new CreatePurchaseRequest
        {
            Description = "Office chair",
            TransactionDate = new DateOnly(2026, 6, 15),
            Amount = 123.45m,
        };

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void Validate_DescriptionOver50Characters_FailsValidation()
    {
        var request = new CreatePurchaseRequest
        {
            Description = new string('a', 51),
            TransactionDate = new DateOnly(2026, 6, 15),
            Amount = 10m,
        };

        var errors = Validate(request);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreatePurchaseRequest.Description)));
    }

    [Fact]
    public void Validate_DescriptionExactly50Characters_IsValid()
    {
        var request = new CreatePurchaseRequest
        {
            Description = new string('a', 50),
            TransactionDate = new DateOnly(2026, 6, 15),
            Amount = 10m,
        };

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_NonPositiveAmount_FailsValidation(decimal amount)
    {
        var request = new CreatePurchaseRequest
        {
            Description = "Something",
            TransactionDate = new DateOnly(2026, 6, 15),
            Amount = amount,
        };

        var errors = Validate(request);
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreatePurchaseRequest.Amount)));
    }
}
