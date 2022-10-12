using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

public struct JsonNet<T> : IResult
{
    public JsonNet(T item)
    {
        Item = item;
    }

    public T Item { get; }

    // Input
    public static async ValueTask<JsonNet<T>> BindAsync(HttpContext httpContext)
    {
        // TODO: Check the content type?

        // Unforutnately this serializer doesn't support async IO, we can buffer or we can allow sync IO
        // this can cause performance problems for bigger payloads to buffering might be more appropriate.
        // context.Features.Get<IHttpBodyControlFeature>()!.AllowSynchronousIO = true;

        httpContext.Request.EnableBuffering();
        await httpContext.Request.Body.DrainAsync(httpContext.RequestAborted);
        httpContext.Request.Body.Position = 0;

        using var reader = new JsonTextReader(new StreamReader(httpContext.Request.Body));
        var serializer = new JsonSerializer();
        var item = serializer.Deserialize<T>(reader);

        return new(item);
    }

    // Output
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";

        // Unforutnately this serializer doesn't support async IO, we can buffer or we can allow sync IO
        // this can cause performance problems for bigger payloads to buffering might be more appropriate.

        httpContext.Features.Get<IHttpBodyControlFeature>()!.AllowSynchronousIO = true;
        using var writer = new JsonTextWriter(new StreamWriter(httpContext.Response.Body));
        var serializer = new JsonSerializer();
        serializer.Serialize(writer, Item);

        // Serializer.Serialize(httpContext.Response.Body, _item);
        return Task.CompletedTask;
    }
}

public static partial class SerializerResultExtensions
{
    public static JsonNet<T> JsonNet<T>(this IResultExtensions _, T obj)
    {
        return new JsonNet<T>(obj);
    }
}
