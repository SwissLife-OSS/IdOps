namespace IdOps
{
    /// <summary>
    /// A generic error
    /// </summary>
    public abstract record Error(string Message) : IError;
}
