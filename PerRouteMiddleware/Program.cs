using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/foo", (string q) => q).Use(sub =>
{
    sub.Use((context, next) =>
    {
        context.Request.QueryString = QueryString.Create("q", "this is a query string value");

        return next(context);
    });
});

app.MapGet("/bar", ([FromHeader(Name = "X-Path")]string s) => s).Use(sub =>
{
    sub.Use((context, next) =>
    {
        context.Request.Headers["X-Path"] = "this is a header value";

        return next(context);
    });
});

app.Run();
