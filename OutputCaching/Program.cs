var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", (int? size, HttpContext context) =>
    {
        const string type = "monsterid";
        size ??= 200;
        var hash = Guid.NewGuid().ToString("n");

        var html = $"""
                    <img src="https://www.gravatar.com/avatar/{hash}?s={size}&d={type}" width="{size}" height="{size}" />
                    <pre>Generated at {DateTime.Now:O}</pre>
                    <a href="/?size={size}">Load</a>
                    """;

        return Results.Text(html, "text/html");
    });

app.Run();

#region codez
//if (context.Features.Get<IResponseCachingFeature>() is { } responseCaching)
//{
//    responseCaching.VaryByQueryKeys = new[] { "size" };
//    context.Response.GetTypedHeaders().CacheControl = new()
//    {
//        Public = true,
//        MaxAge = TimeSpan.FromSeconds(5)
//    };
//}
#endregion
