using System.Runtime.Loader;

var builder = WebApplication.CreateBuilder(args);

var moduleDirPath = Path.Combine(builder.Environment.ContentRootPath, "Modules");

var mvcBuilder = builder.Services.AddControllers();

// Clear the current parts so we don't discover any controllers
mvcBuilder.PartManager.ApplicationParts.Clear();

var dynamicApplicationPart = mvcBuilder.AddDynamicApplicationPart();

var app = builder.Build();

app.MapControllers();

static string Link(string? url) => $"""<div><a href="{url}">{url}</a></div>""";

app.MapGet("/", (EndpointDataSource ds) => Results.Text(
    string.Join("\r\n", ds.Endpoints.OfType<RouteEndpoint>().Select(e => Link(e.RoutePattern.RawText))), "text/html"));

app.MapGet("/load/{module}", async (string module) =>
{
    var modulePath = Path.Combine(moduleDirPath, $"{module}.dll");

    if (!File.Exists(modulePath))
    {
        return Results.Problem($"""Unable to locate module "{module}" """, statusCode: StatusCodes.Status404NotFound);
    }

    await dynamicApplicationPart.AddModuleAsync(module, () =>
    {
        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(modulePath);
        return assembly.GetExportedTypes();
    });

    return Results.Text($"{module} loaded");
});

app.Run();
