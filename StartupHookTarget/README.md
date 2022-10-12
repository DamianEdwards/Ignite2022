## Startup hooks

This sample shows how you can inject a startup hook into the process that hooks the creation of any `IHostBuilder`.
Change `Properties\launchSetings.json` to specify the `DOTNET_STARTUP_HOOKS` environement variable to the full physical path
of `MyStartupHook.dll` (copy the path after building the solution).

This should change the environment name to "Magic" (this is just a demo). With access to the `IHostBuilder`, it's possible to change
configuration, services, and more.
