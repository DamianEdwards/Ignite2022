using System.Diagnostics;
using Microsoft.Extensions.Hosting;

public class StartupHook
{
    public static void Initialize()
    {
        DiagnosticListener.AllListeners.Subscribe(new HostingListener());
    }

    private class HostingListener : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "Microsoft.Extensions.Hosting")
            {
                value.Subscribe(this);
            }
        }

        public void OnNext(KeyValuePair<string, object?> value)
        {
            var (k, v) = value;

            if (k == "HostBuilding" && v is IHostBuilder builder)
            {
                builder.ConfigureServices((c, services) =>
                {
                    // Everyone gets the "Magic" environment
                    c.HostingEnvironment.EnvironmentName = "Magic";
                });
            }
        }
    }
}
