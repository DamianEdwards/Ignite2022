var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Using JSON.NET with minimal APIs");

app.MapPost("/", (JsonNet<Person> p) => Results.Extensions.JsonNet(p.Item));

app.Run();

class Person
{
    public string Name { get; set; } = default!;
}
