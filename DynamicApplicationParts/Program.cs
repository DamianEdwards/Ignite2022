using System.Runtime.Loader;

var builder = WebApplication.CreateBuilder(args);

var mvcBuilder = builder.Services.AddControllers();

// Clear the current parts so we don't discover any controllers
mvcBuilder.PartManager.ApplicationParts.Clear();

var dynamicApplicationPart = mvcBuilder.AddDynamicApplicationPart();

var app = builder.Build();

app.MapControllers();

app.MapGet("/load/{module}", async (string module) =>
{
    var modulePath = Path.Combine(Directory.GetCurrentDirectory(), "..", module, "bin", "Debug", "net7.0", $"{module}.dll");

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
