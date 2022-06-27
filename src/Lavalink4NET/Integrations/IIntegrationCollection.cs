namespace Lavalink4NET.Integrations;

using System;
using System.Collections.Generic;

public interface IIntegrationCollection : IEnumerable<KeyValuePair<Type, ILavalinkIntegration>>
{
    ILavalinkIntegration? this[Type key] { get; set; }

    T? Get<T>();

    void Set<T>(T instance) where T : ILavalinkIntegration;

    void Set<TIntegration, TImplementation>(TImplementation instance)
        where TImplementation : TIntegration, ILavalinkIntegration;
}
