using System.Text.Json.Nodes;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
    // o.RequestBodyLogLimit = 1024;
});

builder.Services.AddW3CLogging(o =>
{
    o.LoggingFields = W3CLoggingFields.All;
});

var app = builder.Build();

app.UseHttpLogging();
app.UseW3CLogging();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", (JsonNode obj) => obj);

app.Run();
