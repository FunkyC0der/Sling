using System;
using System.Collections.Generic;
using Playtika.Controllers;

public sealed class ControllerFactory : IControllerFactory
{
    private readonly Dictionary<Type, Func<IController>> _builders = new();

    public void Register<T>(Func<T> builder) where T : class, IController => 
        _builders[typeof(T)] = builder;

    public IController Create<T>() where T : class, IController
    {
        if (!_builders.TryGetValue(typeof(T), out var builder))
            throw new InvalidOperationException($"Controller {typeof(T).Name} not registered");
        return builder();
    }
}
