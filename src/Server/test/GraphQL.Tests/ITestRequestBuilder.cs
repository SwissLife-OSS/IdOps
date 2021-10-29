using System.Threading.Tasks;
using HotChocolate.Execution;

namespace IdOps.GraphQL.Tests
{
    public interface ITestRequestBuilder
    {
        ITestRequestBuilder AddScope(string scope);
        ITestRequestBuilder AddVariableValue(string name, object? value);
        ITestRequestBuilder AddExecutor(IRequestExecutor requestExecutor);
        ITestRequestBuilder AddRequestFromFile(string fileName);
        Task<IExecutionResult> ExecuteAsync();
    }
}
