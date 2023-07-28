namespace IdOps.Abstractions;

public interface IResultFactory<TResult, in TInput>
{
    Task<TResult> CreateRequestAsync(TInput input, CancellationToken cancellationToken);
}
