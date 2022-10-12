using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

/// <summary>
/// A custom application part that supports loading assemblies dynamically. AddModuleAsync should complete
/// when the module has been successfully loaded by routing and MVC. This allows requests to queue up waiting
/// for a module to be loaded.
/// 
/// There's no way to precise way to know when MVC has updated routing's knowledge about new controller routes so we use
/// a call to IChangeToken.RegisterChangeCallback as an approximation.
/// </summary>
public class DynamicApplicationPart : ApplicationPart, IApplicationPartTypeProvider, IActionDescriptorChangeProvider
{
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ConcurrentDictionary<string, ModuleEntry> _cache = new ConcurrentDictionary<string, ModuleEntry>();

    // This list represents the list of unloaded modules that mvc observed (the ones it enumerated).
    // We can use this to determine which modules to mark completed once MVC finishes loading them.
    // It doesn't need a lock because MVC applies updates sequentially.
    private readonly List<ModuleEntry> _observed = new List<ModuleEntry>();

    public Task AddModuleAsync(string moduleName, params Type[] types)
    {
        return AddModuleAsync(moduleName, () => types);
    }

    public Task AddModuleAsync(string moduleName, Func<Type[]> types)
    {
        while (true)
        {
            // Try to get the current module loading entry
            if (_cache.TryGetValue(moduleName, out var entry))
            {
                // Return the task from the first request
                return entry.Task;
            }

            // Make a new task that all other requests will wait on if they try to
            // execute while loading
            var newEntry = new ModuleEntry(moduleName, types);
            if (!_cache.TryAdd(moduleName, newEntry))
            {
                // We failed to add the entry, that means another thread won the race
                // start over and try to get the cached entry
                continue;
            }

            // Tell MVC we have a new module
            Interlocked.Exchange(ref _cts, new CancellationTokenSource())?.Cancel();

            return newEntry.Task;
        }
    }

    public IChangeToken GetChangeToken()
    {
        return new ModuleChangeToken(this);
    }

    public IEnumerable<TypeInfo> Types
    {
        get
        {
            _observed.Clear();

            foreach (var (key, entry) in _cache)
            {
                var types = entry.Types;

                // Add the list of entries that MVC read on this update round
                // we only care about modules that haven't loaded so we can mark them for completion
                if (!entry.Task.IsCompletedSuccessfully)
                {
                    _observed.Add(entry);
                }

                foreach (var item in types)
                {
                    yield return item.GetTypeInfo();
                }
            }
        }
    }

    public override string Name => "Dynamic Module Loader Part";

    internal void UpdateModules()
    {
        // MVC as loaded these entries and told routing the change has applied
        foreach (var entry in _observed)
        {
            entry.SetLoadingComplete();
        }
    }

    /// <summary>
    /// A change token implementation that triggers module loader updates on call to RegisterChangeCallback.
    /// </summary>
    private class ModuleChangeToken : IChangeToken
    {
        private readonly DynamicApplicationPart _moduleLoader;
        private readonly CancellationToken _token;

        public ModuleChangeToken(DynamicApplicationPart moduleLoader)
        {
            _moduleLoader = moduleLoader;
            _token = moduleLoader._cts.Token;
        }

        public bool ActiveChangeCallbacks => true;

        public bool HasChanged => _token.IsCancellationRequested;

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            // Notify the module loader
            _moduleLoader.UpdateModules();

            return _token.UnsafeRegister(callback, state);
        }
    }

    private class ModuleEntry
    {
        private Type[]? _types;
        private readonly Func<Type[]> _factory;
        private readonly TaskCompletionSource _tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private object _sync = new();

        public string ModuleName { get; }

        public Task Task => _tcs.Task;

        public Type[] Types
        {
            get => LazyInitializer.EnsureInitialized(ref _types, ref _sync, _factory);
        }

        public ModuleEntry(string moduleName, Func<Type[]> factory)
        {
            ModuleName = moduleName;
            _factory = factory;
        }

        public void SetLoadingComplete() => _tcs.TrySetResult();
    }
}
