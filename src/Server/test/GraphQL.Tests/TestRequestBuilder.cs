using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Security;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter;
using Snapshooter.Xunit;
using Path = System.IO.Path;

namespace IdOps.GraphQL.Tests;

public class TestRequestBuilder
    : ITestRequestBuilder
{
    private readonly QueryRequestBuilder _requestBuilder = QueryRequestBuilder.New();
    private readonly List<Claim> _claims = new();
    private IRequestExecutor? _executor;
    private bool _isAuthenticated = true;

    public ITestRequestBuilder AddScope(string scope)
    {
        _claims.Add(new Claim("scope", scope));
        return this;
    }

    public ITestRequestBuilder AddVariableValue(string name, object? value)
    {
        _requestBuilder.AddVariableValue(name, value);
        return this;
    }

    public ITestRequestBuilder AddExecutor(IRequestExecutor requestExecutor)
    {
        _executor = requestExecutor;
        return this;
    }

    /// <inheritdoc />
    public ITestRequestBuilder AddClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
        return this;
    }

    public ITestRequestBuilder SetAuthenticated(bool isAuthenticated)
    {
        _isAuthenticated = isAuthenticated;
        return this;
    }

    public ITestRequestBuilder AddRequestFromFile(string fileName)
    {
        SnapshotFullName name = new XunitSnapshotFullNameReader().ReadSnapshotFullName();
        var filePath = Path.Combine(
            name.FolderPath.Replace("/__snapshots__", ""),
            "__operations__",
            $"{fileName}.graphql");

        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"Could not find file {filePath}", nameof(fileName));
        }

        var operation = File.ReadAllText(filePath);

        _requestBuilder.SetQuery(operation);

        return this;
    }

    public Task<IExecutionResult> ExecuteAsync()
    {
        if (_executor is null)
        {
            throw new InvalidOperationException("Execute was not set");
        }

        if (_isAuthenticated)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(_claims, "Test"));
            _requestBuilder.SetUser(claimsPrincipal);
        }

        return _executor.ExecuteAsync(_requestBuilder.Create());
    }

    public static ITestRequestBuilder New() => new TestRequestBuilder();
}
