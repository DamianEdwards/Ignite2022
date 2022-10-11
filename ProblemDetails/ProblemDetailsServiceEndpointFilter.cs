using Microsoft.AspNetCore.Http.HttpResults;
using Mvc = Microsoft.AspNetCore.Mvc;

namespace ProblemDetails;

public class ProblemDetailsServiceEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        => await next(context) switch
        {
            ProblemHttpResult problemHttpResult => new ProblemDetailsServiceAwareResult(problemHttpResult.StatusCode, problemHttpResult.ProblemDetails),
            Mvc.ProblemDetails problemDetails => new ProblemDetailsServiceAwareResult(null, problemDetails),
            { } result => result,
            null => null
        };

    private class ProblemDetailsServiceAwareResult : IResult, IValueHttpResult, IValueHttpResult<Mvc.ProblemDetails>
    {
        private readonly int? _statusCode;

        public ProblemDetailsServiceAwareResult(int? statusCode, Mvc.ProblemDetails problemDetails)
        {
            _statusCode = statusCode ?? problemDetails.Status;
            Value = problemDetails;
        }

        public Mvc.ProblemDetails Value { get; }

        object? IValueHttpResult.Value => Value;

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            if (httpContext.RequestServices.GetService<IProblemDetailsService>() is IProblemDetailsService problemDetailsService)
            {
                if (_statusCode is { } statusCode)
                {
                    httpContext.Response.StatusCode = statusCode;
                }
                await problemDetailsService.WriteAsync(new()
                {
                    HttpContext = httpContext,
                    ProblemDetails = Value
                });
            }
        }
    }
}
