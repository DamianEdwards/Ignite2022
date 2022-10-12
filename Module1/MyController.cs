using Microsoft.AspNetCore.Mvc;

namespace Module1;

public class MyController
{
    [HttpGet("/module1")]
    public string Get() => "This is controller was loaded dynamically";
}
