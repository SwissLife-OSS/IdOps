namespace IdOps
{
    /// <summary>
    /// This error is returned when the HashAlgorithm was not found
    /// </summary>
    public record HashAlgorithmNotFound(string hashAlgorithm)
        : Error("The provided value for `algorithm` was invalid. " +
              $"{hashAlgorithm} could not be resolver"),
          ICreatePersonalAccessTokenError;
}
