using Microsoft.AspNetCore.Mvc;

namespace Module1;

public class MyController
{
    [HttpGet("/")]
    public string Get() => "This is a dynamic controller part";
}
