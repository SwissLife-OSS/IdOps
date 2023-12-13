using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdOps.Server.Storage.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.GraphQL.Tests;

public sealed class TestDataBuilder
{
    private readonly List<Func<Task>> _tasks = new();

    public TestDataBuilder(IServiceProvider services)
    {
        Services = services;
        DbContext = services.GetRequiredService<IIdOpsDbContext>();
    }

    public IIdOpsDbContext DbContext { get; }

    public IServiceProvider Services { get; }

    public TestDataBuilder Enqueue(Func<Task> task)
    {
        _tasks.Add(task);
        return this;
    }

    public async Task ExecuteAsync()
    {
        foreach (Func<Task> task in _tasks)
        {
            await task();
        }
    }

    public static TestDataBuilder New(IServiceProvider services)
    {
        return new(services);
    }
}
