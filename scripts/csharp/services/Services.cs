using System;
using System.Collections.Generic;

namespace Code.Service;

public static class Services
{
    private static readonly Dictionary<Type, object> _services = new();

    /// <summary>
    /// Registers a service of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the service to register.</typeparam>
    /// <param name="service">The instance of the service.</param>
    public static void Add<T>(T service) where T : class, IDisposable
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            throw new InvalidOperationException($"Service of type {type} is already registered.");
        }

        _services[type] = service;
    }

    /// <summary>
    /// Resolves a service of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    public static T Get<T>() where T : class
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        throw new InvalidOperationException($"Service of type {type} is not registered.");
    }

    /// <summary>
    /// Dispose all registered services.
    /// </summary>
    public static void Dispose()
    {
        foreach (var service in _services.Values)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _services.Clear();
    }
}