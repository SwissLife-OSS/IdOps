using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotChocolate.Execution;
using IdOps.Security;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter;
using Snapshooter.Xunit;

namespace IdOps.GraphQL.Tests
{
    public class TestRequestBuilder
        : ITestRequestBuilder
    {
        private readonly IQueryRequestBuilder _requestBuilder = QueryRequestBuilder.New();
        private readonly List<Claim> _claims = new();
        private IRequestExecutor? _executor;

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

        public async Task<IExecutionResult> ExecuteAsync()
        {
            if (_executor is null)
            {
                throw new InvalidOperationException("Execute was not set");
            }

            if (_claims.Any())
            {
                ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(_claims, "Test"));

                IUserContextAccessor accessor =
                    _executor.Schema.Services!.GetRequiredService<IUserContextAccessor>();

                accessor.Context = _executor.Schema.Services!
                    .GetRequiredService<IUserContextFactory>()
                    .Create(claimsPrincipal);

                _requestBuilder.TryAddProperty(nameof(ClaimsPrincipal), claimsPrincipal);
            }

            return await _executor.ExecuteAsync(_requestBuilder.Create());
        }

        public static ITestRequestBuilder New() => new TestRequestBuilder();
    }
}
