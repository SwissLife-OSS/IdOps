using System;

namespace IdOps.Exceptions;

public class NoEncryptedSecretException : Exception
{
    public NoEncryptedSecretException(string? message) : base(message) { }
}
