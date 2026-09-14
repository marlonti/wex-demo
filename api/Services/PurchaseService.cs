using api.Common;
using api.Data;
using api.Dtos;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class PurchaseService(PurchaseDbContext dbContext, IExchangeRateService exchangeRateService) : IPurchaseService
{
    public async Task<PurchaseResponse> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            TransactionDate = request.TransactionDate,
            Amount = MoneyMath.RoundToCents(request.Amount),
        };

        dbContext.Purchases.Add(purchase);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(purchase);
    }

    public async Task<PurchaseOperationResult> GetConvertedAsync(
        Guid id,
        string countryCurrencyDesc,
        CancellationToken cancellationToken)
    {
        var purchase = await dbContext.Purchases.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (purchase is null)
        {
            return new PurchaseNotFound();
        }

        var lookup = await exchangeRateService.GetRateAsync(countryCurrencyDesc, purchase.TransactionDate, cancellationToken);

        return lookup switch
        {
            ExchangeRateFound found => new PurchaseConverted(new PurchaseConversionResponse(
                purchase.Id,
                purchase.Description,
                purchase.TransactionDate,
                purchase.Amount,
                found.Rate,
                MoneyMath.RoundToCents(purchase.Amount * found.Rate),
                found.Country,
                found.Currency,
                found.RateDate)),

            ExchangeRateNotFound => new ConversionUnavailable(
                $"The purchase cannot be converted to '{countryCurrencyDesc}': no exchange rate available within 6 months on or before {purchase.TransactionDate:yyyy-MM-dd}."),

            _ => throw new InvalidOperationException($"Unhandled exchange rate lookup result: {lookup.GetType()}"),
        };
    }

    private static PurchaseResponse ToResponse(Purchase purchase) =>
        new(purchase.Id, purchase.Description, purchase.TransactionDate, purchase.Amount);
}
