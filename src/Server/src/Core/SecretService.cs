using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdOps.Model;
using IdentityModel;
using System.Linq;
using IdOps.Controller;

namespace IdOps
{
    public class SecretService : ISecretService
    {
        private readonly IEnumerable<ISharedSecretGenerator> _sharedSecretGenerators;
        private readonly IKeyVaultController _keyVaultController;

        public SecretService(IEnumerable<ISharedSecretGenerator> sharedSecretGenerators, IKeyVaultController keyVaultController)
        {
            _sharedSecretGenerators = sharedSecretGenerators;
            _keyVaultController = keyVaultController;
        }

        public async Task<(Secret, string)> CreateSecretAsync(AddSecretRequest request)
        {
            var secret = new Secret
            {
                Id = Guid.NewGuid(),
                Type = "SharedSecret",
                Description = request.Name
            };

            string secretValue = request.Value!;

            if (string.IsNullOrEmpty(secretValue))
            {
                ISharedSecretGenerator generator = _sharedSecretGenerators
                    .Single(x => x.Name == request.Generator);

                secretValue = generator.CreateSecret();
            }

            secret.Value = secretValue.ToSha256();


            if (request.SaveValue.GetValueOrDefault())
            {
                secret.EncryptedSecret = await _keyVaultController.Encrypt(secretValue);
                secret.EncryptionKeyId = _keyVaultController.GetEncryptionKeyNameBase64();
            }

            return (secret, secretValue);
        }

    }
}
