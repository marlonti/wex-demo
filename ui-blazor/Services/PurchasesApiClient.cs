using System.Net.Http.Json;
using System.Text.Json;
using ui_blazor.Models;

namespace ui_blazor.Services;

public sealed class PurchasesApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<PurchaseResponse> CreatePurchaseAsync(CreatePurchaseRequest request, CancellationToken ct = default)
    {
        using var response = await httpClient.PostAsJsonAsync("purchases/", request, JsonOptions, ct);
        if (!response.IsSuccessStatusCode)
            throw await CreateExceptionAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<PurchaseResponse>(JsonOptions, ct))!;
    }

    public async Task<PurchaseConversionResponse> GetConvertedAsync(Guid id, string countryCurrency, CancellationToken ct = default)
    {
        var url = $"purchases/{id}?countryCurrency={Uri.EscapeDataString(countryCurrency)}";
        using var response = await httpClient.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
            throw await CreateExceptionAsync(response, ct);
        return (await response.Content.ReadFromJsonAsync<PurchaseConversionResponse>(JsonOptions, ct))!;
    }

    private static async Task<ApiException> CreateExceptionAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var statusCode = (int)response.StatusCode;
        try
        {
            var raw = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.TryGetProperty("errors", out var errorsEl) && errorsEl.ValueKind == JsonValueKind.Object)
            {
                var errors = errorsEl.EnumerateObject().ToDictionary(
                    p => p.Name,
                    p => p.Value.EnumerateArray().Select(v => v.GetString() ?? "").ToArray());
                var first = errors.Values.SelectMany(v => v).FirstOrDefault() ?? "Validation failed.";
                return new ApiException(statusCode, first, errors);
            }

            var detail = root.TryGetProperty("detail", out var d) ? d.GetString() : null;
            var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            return new ApiException(statusCode, detail ?? title ?? response.ReasonPhrase ?? "Request failed.");
        }
        catch (JsonException)
        {
            return new ApiException(statusCode, response.ReasonPhrase ?? $"Request failed with status {statusCode}.");
        }
    }
}
