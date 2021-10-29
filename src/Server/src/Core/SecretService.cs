using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdOps.Model;
using IdentityModel;
using System.Linq;

namespace IdOps
{
    public class SecretService : ISecretService
    {
        private readonly IEnumerable<ISharedSecretGenerator> _sharedSecretGenerators;

        public SecretService(IEnumerable<ISharedSecretGenerator> sharedSecretGenerators)
        {
            _sharedSecretGenerators = sharedSecretGenerators;
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
                // Encrypt and save to KV
                await Task.Delay(0);
            }

            return (secret, secretValue);
        }
    }
}
