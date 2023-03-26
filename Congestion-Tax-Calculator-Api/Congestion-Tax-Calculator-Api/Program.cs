using Congestion_Tax_Calculator_Api.Interfaces;
using Congestion_Tax_Calculator_Api.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ITaxCongestionPersistance, TaxCongestionPersistance>();
builder.Services.AddSingleton<ICongestionTaxCalculatorService, CongestionTaxCalculatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
