using System.Threading.Tasks;
using HotChocolate.Execution;

namespace IdOps.GraphQL.Tests;

public interface ITestRequestBuilder
{
    ITestRequestBuilder AddVariableValue(string name, object? value);

    ITestRequestBuilder AddExecutor(IRequestExecutor requestExecutor);

    ITestRequestBuilder AddRequestFromFile(string fileName);

    ITestRequestBuilder AddClaim(string type, string value);

    ITestRequestBuilder SetAuthenticated(bool isAuthenticated);

    Task<IExecutionResult> ExecuteAsync();
}