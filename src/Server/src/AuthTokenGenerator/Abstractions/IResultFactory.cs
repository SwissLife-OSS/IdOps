namespace IdOps.Abstractions;

public interface IResultFactory<TResult, in TInput>
{
    Task<TResult> Create(TInput input, CancellationToken cancellationToken);
}
