# BRK203H Microsoft Ignite – Pubble: Hidden gems and live coding with .NET 7

45 mins total session time
35-40 mins content

## New stuff

- Rate limiting
- Full request/response logging
- Output caching
- Counters & events in VS Performance Profiler
- Problem Details
    - Common service for writing problem details to responses
    - Hooks up to developer exception page, new default exception handler, status code pages & ApiController by default
    - Can be hooked up to problem `IResult` returned from endpoints via an endpoint filter
- `ActivityTrackingOptions`

## Interesting samples & patterns

- [Startup hooks for good](StartupHookTarget)
- [Serializer pattern in Minimal APIs](CustomSerializeMinimalApis)
- [Deferred ApplicationPart loading](DynamicApplicationParts)
- [WebApplication plugins pattern](https://github.com/davidfowl/WebApplicationPlugins)
- [Small proxy](YarpProxy)
- [Metadata-only endpoints](https://github.com/DamianEdwards/AspNetCorePathAuthorization)

## Early days but I like where this is going..

- Trimming, AOT, and the smallest possible .NET app
  - ASP.NET Core considerations
  - https://github.com/DamianEdwards/TrimmedTodo
