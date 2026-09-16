namespace ui_blazor.Services;

public sealed class ApiException(int statusCode, string message,
    IReadOnlyDictionary<string, string[]>? validationErrors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; } = validationErrors;
}
