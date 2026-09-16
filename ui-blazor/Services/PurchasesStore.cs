using ui_blazor.Models;

namespace ui_blazor.Services;

public sealed class PurchasesStore(PurchasesApiClient apiClient)
{
    private readonly List<PurchaseResponse> _purchases = [];

    public IReadOnlyList<PurchaseResponse> Purchases => _purchases;

    public event Action? Changed;

    public async Task<PurchaseResponse> CreateAsync(CreatePurchaseRequest request, CancellationToken ct = default)
    {
        var created = await apiClient.CreatePurchaseAsync(request, ct);
        _purchases.Add(created);
        Changed?.Invoke();
        return created;
    }
}
