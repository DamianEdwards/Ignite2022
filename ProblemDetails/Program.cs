using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseStatusCodePages();

app.MapGet("/", () => new HelloMessage("Hello World"));

app.MapGet("/problem", () => Results.Problem("This is a problem"))
    .AddEndpointFilter<ProblemDetailsServiceEndpointFilter>();

app.MapGet("/status", (int? statusCode) => Results.StatusCode(statusCode ?? 200));

app.MapGet("/throw", (int? statusCode) =>
{
    throw statusCode switch
    {
        >= 400 and < 500 => new BadHttpRequestException(
            $"{statusCode} {ReasonPhrases.GetReasonPhrase(statusCode.Value)}",
            statusCode.Value),
        null => new Exception("uh oh"),
        _ => new Exception($"Staus code {statusCode}")
    };
});

app.MapControllers();

app.Run();

static void CustomizeProblemDetails(ProblemDetailsOptions options) =>
    options.CustomizeProblemDetails = ctx =>
        ctx.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName);

static async Task CustomExceptionHandler(HttpContext context)
{
    var problemDetails = context.RequestServices.GetRequiredService<IProblemDetailsService>();
    // Do something custom here


    await problemDetails.WriteAsync(new() { HttpContext = context });
}

record HelloMessage(string Message);

[ApiController]
[Route("/[controller]")]
public class GreetingsController : Controller
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        if (DateTime.Now.Second % 2 == 1) throw new InvalidOperationException("Odd to see you here");

        return $"Greetings from {nameof(GreetingsController)}";
    }
}
