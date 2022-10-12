var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOutputCache();

var app = builder.Build();

app.UseResponseCaching();
app.UseOutputCache();

app.MapGet("/", (HttpContext context) =>
    {
        const string type = "monsterid";
        const int size = 200;
        var hash = Guid.NewGuid().ToString("n");

        context.Response.GetTypedHeaders().CacheControl = new()
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(5)
        };

        var html = $"""
                    <img src="https://www.gravatar.com/avatar/{hash}?s={size}&d={type}" width="{size}" height="{size}" />
                    <pre>Generated at {DateTime.Now:O}</pre>
                    <a href="/">Load</a>
                    """;

        return Results.Text(html, "text/html");
    })
    //.CacheOutput()
    ;

app.Run();
