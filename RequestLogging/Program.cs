using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
    // o.RequestBodyLogLimit = 1024;
    // o.RequestHeaders.Add(HeaderNames.ContentLength);
});

builder.Services.AddW3CLogging(o =>
{
    o.LoggingFields = W3CLoggingFields.All;
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var ms = new MemoryStream();
    context.Response.Body = ms;

    var reader = new StreamReader(context.Response.Body);

    var sb = new StringBuilder();
    foreach (var header in context.Request.Headers)
    {
        sb.Append($"{header.Key}={header.Value}");
    }

    app.Logger.LogInformation(sb.ToString());

    app.Logger.LogDebug(await reader.ReadLineAsync());

    await next(context);

    app.Logger.LogDebug(Encoding.UTF8.GetString(ms.ToArray()));
});

// app.UseHttpLogging();
// app.UseW3CLogging();

app.MapGet("/", () => "Hello from request logging");
app.MapPost("/", (JsonNode obj) => obj);

app.Run();
