using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

#region services

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

#endregion

#region top secret

//builder.WebHost.ConfigureKestrel(o =>
//{
//    o.ConfigureEndpointDefaults(ed => ed.UseConnectionLogging());
//});

#endregion

var app = builder.Build();

// Don't do this
app.Use(async (context, next) =>
{
    // Request
    var sb = new StringBuilder();
    foreach (var header in context.Request.Headers)
    {
        sb.Append($"{header.Key}={header.Value}");
    }

    app.Logger.LogInformation(sb.ToString());

    // Body
    context.Request.EnableBuffering();
    using var reader = new StreamReader(context.Request.Body);

    app.Logger.LogDebug(await reader.ReadLineAsync());

    context.Request.Body.Position = 0;

    // Response
    var ms = new MemoryStream();
    var old = context.Response.Body;
    context.Response.Body = ms;

    await next(context);

    sb.Clear();
    foreach (var header in context.Response.Headers)
    {
        sb.Append($"{header.Key}={header.Value}");
    }

    app.Logger.LogInformation(sb.ToString());

    app.Logger.LogDebug(Encoding.UTF8.GetString(ms.ToArray()));

    ms.Position = 0;
    await ms.CopyToAsync(old);
});

#region middleware
// app.UseHttpLogging();
// app.UseW3CLogging();
#endregion

app.MapGet("/", () => "Hello from request logging");
app.MapPost("/", (JsonNode obj) => obj);

app.Run();
