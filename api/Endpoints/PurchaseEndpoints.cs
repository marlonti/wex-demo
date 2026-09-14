using api.Dtos;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints;

public static class PurchaseEndpoints
{
    public static void MapPurchaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/purchases").WithTags("Purchases");

        group.MapPost("/", async (CreatePurchaseRequest request, IPurchaseService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/purchases/{created.Id}", created);
        })
        .WithName("CreatePurchase");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromQuery] string? countryCurrency,
            IPurchaseService service,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(countryCurrency))
            {
                return Results.Problem(
                    title: "Missing query parameter",
                    detail: "The 'countryCurrency' query parameter is required, in the Treasury API's 'Country-Currency' format (e.g. 'Canada-Dollar').",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var result = await service.GetConvertedAsync(id, countryCurrency, cancellationToken);

            return result switch
            {
                PurchaseConverted converted => Results.Ok(converted.Response),

                PurchaseNotFound => Results.Problem(
                    title: "Purchase not found",
                    detail: $"Purchase '{id}' was not found.",
                    statusCode: StatusCodes.Status404NotFound),

                ConversionUnavailable unavailable => Results.Problem(
                    title: "Conversion unavailable",
                    detail: unavailable.Reason,
                    statusCode: StatusCodes.Status422UnprocessableEntity),

                _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError),
            };
        })
        .WithName("GetPurchaseConverted");
    }
}
