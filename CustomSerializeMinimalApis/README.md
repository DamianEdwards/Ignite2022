# Custom Serialization with Minimal APIs

This sample shows a pattern for building custom serialization for minimal APIs. The pattern is as following:

```C#
using Microsoft.AspNetCore.Http.Features;

public class Serialized<T> : IResult, IBindableFromHttpContext<Serialized<T>>
{
    public Serialized(T? item)
    {
        Item = item;
    }

    public T? Item { get; }

    // Input
    public static async ValueTask<Serialized<T>?> BindAsync(HttpContext httpContext, ParameterInfo parameterInfo)
    {
        // Write code here to de-serailize T from the request body
    }

    // Output
    public Task ExecuteAsync(HttpContext httpContext)
    {
        // Write code here to serialize T to the response body
    }
}

// Add an extension method to hang off Results.Extensions to make the result type more discoverable.
public static partial class SerializerResultExtensions
{
    public static Serialized<T> Serialize<T>(this IResultExtensions _, T obj)
    {
        return new Serialized<T>(obj);
    }
}
```

This pattern is implemented using JSON.NET in this application.
