// Don't forget to set the DOTNET_STARTUP_HOOKS env variable!

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
