using IdOps.Model;

namespace IdOps.Data.Errors;

public record NoEncryptedSecretError() : Error("The provided client has no encrypted secret saved.")
{
}
