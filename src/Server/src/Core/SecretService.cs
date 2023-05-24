using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdOps.Model;
using IdentityModel;
using System.Linq;
using System.Threading;
using IdOps.Data.Errors;

namespace IdOps;

public class SecretService : ISecretService
{
    private readonly IEnumerable<ISharedSecretGenerator> _sharedSecretGenerators;
    private readonly IEncryptionService _encryptionService;

    public SecretService(IEnumerable<ISharedSecretGenerator> sharedSecretGenerators,
        IEncryptionService encryptionService)
    {
        _sharedSecretGenerators = sharedSecretGenerators;
        _encryptionService = encryptionService;
    }

    public async Task<(Secret, string)> CreateSecretAsync(AddSecretRequest request)
    {
        var secret = new Secret
        {
            Id = Guid.NewGuid(), Type = "SharedSecret", Description = request.Name
        };

        string secretValue = request.Value!;

        if (string.IsNullOrEmpty(secretValue))
        {
            ISharedSecretGenerator generator =
                _sharedSecretGenerators.Single(x => x.Name == request.Generator);

            secretValue = generator.CreateSecret();
        }

        secret.Value = secretValue.ToSha256();


        if (request.SaveValue.GetValueOrDefault())
        {
            secret.EncryptedSecret = await _encryptionService
                .EncryptAsync(secretValue, CancellationToken.None);

            secret.EncryptionKeyId = await _encryptionService
                .GetEncryptionKeyNameBase64Async();
        }

        return (secret, secretValue);
    }

    public async Task<string> GetDecryptedSecretAsync(
        Secret secret,
        CancellationToken cancellationToken)
    {
        var encryptedValue = secret.EncryptedSecret;
        if (encryptedValue == null)
        {
            ErrorException.Throw(new NoEncryptedSecretError());
        }

        return await _encryptionService.DecryptAsync(encryptedValue, cancellationToken);
    }
}
