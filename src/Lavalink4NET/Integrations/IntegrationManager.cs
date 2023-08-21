namespace Lavalink4NET.Integrations;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal sealed class IntegrationManager : IIntegrationManager
{
    private Dictionary<Type, ILavalinkIntegration>? _integrations;

    /// <inheritdoc/>
    public ILavalinkIntegration? this[Type key]
    {
        get
        {
            if (_integrations is null)
            {
                return null;
            }

            return _integrations.TryGetValue(key, out var instance) ? instance : null;
        }

        set
        {
            _integrations ??= new Dictionary<Type, ILavalinkIntegration>();
            _integrations[key] = value!;
        }
    }

    /// <inheritdoc/>
    public T? Get<T>()
    {
        return (T?)this[typeof(T)];
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, ILavalinkIntegration>> GetEnumerator()
    {
        return _integrations is null
            ? Enumerable.Empty<KeyValuePair<Type, ILavalinkIntegration>>().GetEnumerator()
            : _integrations.GetEnumerator();
    }

    /// <inheritdoc/>
    public void Set<T>(T instance) where T : ILavalinkIntegration
    {
        this[typeof(T)] = instance;
    }

    public void Set<TIntegration, TImplementation>(TImplementation instance)
        where TImplementation : ILavalinkIntegration, TIntegration
    {
        this[typeof(TIntegration)] = instance;
    }
}
