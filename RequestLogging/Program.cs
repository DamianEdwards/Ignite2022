using System.Text.Json.Nodes;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
});

builder.Services.AddW3CLogging(o =>
{
    o.LoggingFields = W3CLoggingFields.All;
});

var app = builder.Build();

app.UseHttpLogging();
app.UseW3CLogging();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", (JsonObject obj) => obj);

app.Run();
