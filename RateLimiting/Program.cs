using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = OnRateLimitRejected;
    options.AddPolicy("FiveRequestsEveryTenSecondsPerSession", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(ctx.Request.Cookies["EXAMPLE_SESSIONID"], id =>
            new()
            {
                Window = TimeSpan.FromSeconds(10),
                PermitLimit = 5,
                QueueLimit = 2
            }
        ));
});

var app = builder.Build();

app.Use((context, next) =>
{
    if (!context.Request.Cookies.ContainsKey("EXAMPLE_SESSIONID"))
    {
        var id = Guid.NewGuid();
        context.Response.Cookies.Append("EXAMPLE_SESSIONID", id.ToString(), new() { MaxAge = TimeSpan.FromMinutes(10) });
    }

    return next(context);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.MapGet("/", () => "Hello World");

app.MapGet("/limited", () => "This API is limited!")
    .RequireRateLimiting("FiveRequestsEveryTenSecondsPerSession");

app.Run();

static ValueTask OnRateLimitRejected(OnRejectedContext context, CancellationToken cancellationToken)
{
    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var window))
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers.RetryAfter = window.TotalSeconds.ToString();
    }
    return ValueTask.CompletedTask;
}
