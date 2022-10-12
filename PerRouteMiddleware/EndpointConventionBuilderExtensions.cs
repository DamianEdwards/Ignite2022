public static class EndpointConventionBuilderExtensions
{
    public static IEndpointConventionBuilder Use(this IEndpointConventionBuilder builder, Action<IApplicationBuilder> configure)
    {
        builder.Add(e =>
        {
            var app = new ApplicationBuilder(e.ApplicationServices);
            configure(app);
            if (e.RequestDelegate is not null)
            {
                app.Run(e.RequestDelegate);
            }

            e.RequestDelegate = app.Build();
        });

        return builder;
    }
}
