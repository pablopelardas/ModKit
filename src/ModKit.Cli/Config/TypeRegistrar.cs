﻿using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace ModKit.Cli.Config;

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder = new ServiceCollection();

    public void Register(Type service, Type implementation)
        => _builder.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation)
        => _builder.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
        => _builder.AddSingleton(service, _ => factory());

    public ITypeResolver Build() => new TypeResolver(_builder.BuildServiceProvider());
}

public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly ServiceProvider _provider;

    public TypeResolver(ServiceProvider provider) => _provider = provider;

    public object? Resolve(Type type) => _provider.GetService(type);

    public void Dispose() => _provider.Dispose();
}