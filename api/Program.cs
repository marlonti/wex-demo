using api.Data;
using api.Endpoints;
using api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddValidation();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PurchaseDbContext>(options => options.UseInMemoryDatabase("Purchases"));

builder.Services.AddHttpClient<IExchangeRateService, TreasuryExchangeRateService>(client =>
{
    client.BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/services/api/fiscal_service/");
});

builder.Services.AddScoped<IPurchaseService, PurchaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPurchaseEndpoints();

app.Run();
