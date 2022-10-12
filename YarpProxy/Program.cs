using System.Net;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpForwarder();

var app = builder.Build();

var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
{
    UseProxy = false,
    UseCookies = false
});

app.Map("{**path}", async (IHttpForwarder forwarder, HttpContext httpContext) =>
{
    var error = await forwarder.SendAsync(httpContext, "https://localhost:7188", httpClient);

    if (error != ForwarderError.None)
    {
        var errorFeature = httpContext.GetForwarderErrorFeature();
        var exception = errorFeature?.Exception;
    }
});

app.Run();

record Upstrem(string Scheme, string Host);
