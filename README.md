# BRK203H Microsoft Ignite – Pubble: Hidden gems and live coding with .NET 7

45 mins total session time
35-40 mins content

## New stuff

- Mainstream
  - Rate limiting
  - Output caching
  - Problem Details
    - Common service for writing problem details to responses
    - Hooks up to developer exception page, new default exception handler, status code pages & ApiController by default
    - Can be hooked up to problem `IResult` returned from endpoints via an endpoint filter
  - Full request/response logging
  - `ActivityTrackingOptions`
  - New LINQ APIs: `Order`
  - Static abstract members on interfaces `IParsable<T>`
  - Counters & events in VS Performance Profiler
- Esoteric
  - `System.Diagnostics.Metrics`
  - QUIC transport
  - HTTP/2 WebSockets

## Interesting samples & patterns

- Startup hooks
- [Wrapper BindAsync type for protobuf](https://gist.github.com/davidfowl/41bcbccc7d8408af57ec1253ca558775)
- [Composite server example](https://gist.github.com/davidfowl/2ae62e7c34c27a58faacf8b0463b1586)
- [Deferred ApplicationPart loading](https://gist.github.com/davidfowl/d16c352c19b89acc2a20fe4c1061cad9)
- [WebApplication plugins pattern](https://github.com/davidfowl/WebApplicationPlugins)
- [Small proxy](https://gist.github.com/davidfowl/cf68de7da1f6cb4d7dcbedd6e1a9c6a4)
- [Metadata-only endpoints](https://github.com/DamianEdwards/AspNetCorePathAuthorization)

## Don't try this at home

- Trimming, AOT, and the smallest possible .NET app
  - ASP.NET Core considerations
  - https://github.com/DamianEdwards/TrimmedTodo
