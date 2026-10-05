using Api.Errores;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json.Serialization;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    TareaNoEncontradaExceptionHandler>();

builder.Services.Configure<ExceptionHandlerOptions>(options =>
{
    options.AllowStatusCode404Response = true;
});

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();