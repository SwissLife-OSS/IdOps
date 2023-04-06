namespace IdOps.Abstractions;

public interface IFactory <TResult, in TInput>
{
    Task<TResult> Create(TInput input, CancellationToken cancellationToken);
}
