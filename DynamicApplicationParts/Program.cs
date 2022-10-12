using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var mvcBuilder = builder.Services.AddControllers();

// Clear the current parts so we don't discover any controllers
mvcBuilder.PartManager.ApplicationParts.Clear();

var dynamicApplicationPart = mvcBuilder.AddDynamicApplicationPart();

var app = builder.Build();

app.MapControllers();

app.Start();

app.Logger.LogInformation("Dynamically adding the controller");
await Task.Delay(5000);

await dynamicApplicationPart.AddModuleAsync("MyModule", typeof(MyController));

app.Logger.LogInformation("The routes for the controller should be ready");

app.WaitForShutdown();


public class MyController
{
    [HttpGet("/")]
    public string Get() => "This is a dynamic controller part";
}
