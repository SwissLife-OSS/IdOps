using System;

namespace IdOps.Exceptions;

/// <summary>
/// A error message that occurs when the `encryptedSecret` field of a client is null
/// </summary>
public class NoEncryptedSecretException : Exception
{
    public NoEncryptedSecretException(string? message) : base(message) { }
}
